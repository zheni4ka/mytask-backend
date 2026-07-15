using Core.DTOs;
using Core.DTOs.User;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Google.Apis.Auth;

namespace business_logic.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly ICategoryService _categoryService;
        private readonly IConfiguration _configuration;
        private readonly IJwtService _jwtService;

        public AuthService(UserManager<User> userManager, IConfiguration configuration, ICategoryService categoryService, IJwtService jwtService)
        {
            _userManager = userManager;
            _categoryService = categoryService;
            _configuration = configuration;
            _jwtService = jwtService;
        }

        public async Task<AuthResponse> LoginWithGoogleAsync(GoogleLoginRequest request)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string> { _configuration["Google:ClientId"] }
            };

            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, settings);

                var user = await _userManager.FindByEmailAsync(payload.Email);
                if (user == null)
                {
                    user = new User
                    {
                        Email = payload.Email,
                        UserName = payload.Email
                    };

                    var result = await _userManager.CreateAsync(user);
                    if (!result.Succeeded)
                    {
                        return new AuthResponse
                        {
                            IsAuthenticated = false,
                            ErrorMessage = "Unable to find user in db"
                        };
                    }
                }

                var token = _jwtService.GenerateJwtToken(user);

                return new AuthResponse
                {
                    IsAuthenticated = true,
                    Token = token,
                    RefreshToken = null
                };
            }
            catch (InvalidJwtException)
            {
                return new AuthResponse
                {
                    IsAuthenticated = false,
                    ErrorMessage = "Invalid Google token"
                };
            }
        }


        public async Task<AuthResponse> LoginAsync(LoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return new AuthResponse { IsAuthenticated = false, ErrorMessage = "Invalid email or password." };
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!isPasswordValid)
            {
                return new AuthResponse { IsAuthenticated = false, ErrorMessage = "Invalid email or password." };
            }

            var token = _jwtService.GenerateJwtToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); 
            await _userManager.UpdateAsync(user);

            return new AuthResponse
            {
                IsAuthenticated = true,
                Token = token,
                RefreshToken = refreshToken
            };
        }

        public async Task<AuthResponse> RefreshTokenAsync(TokenModel tokenModel)
        {
            if (tokenModel is null)
                return new AuthResponse { IsAuthenticated = false, ErrorMessage = "Invalid client request" };

            string accessToken = tokenModel.AccessToken;
            string refreshToken = tokenModel.RefreshToken;

            var principal = _jwtService.GetPrincipalFromExpiredToken(accessToken);
            if (principal == null)
                return new AuthResponse { IsAuthenticated = false, ErrorMessage = "Invalid access token or refresh token" };

            var email = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var user = await _userManager.FindByEmailAsync(email!);

            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return new AuthResponse { IsAuthenticated = false, ErrorMessage = "Invalid access token or refresh token" };
            }

            var newAccessToken = _jwtService.GenerateJwtToken(user);
            var newRefreshToken = _jwtService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            await _userManager.UpdateAsync(user);

            return new AuthResponse
            {
                IsAuthenticated = true,
                Token = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }

        public async Task<AuthResponse> RegisterAsync(RegisterModel model)
        {
            if (!model.Password.Equals(model.ConfirmPassword))
            {
                return new AuthResponse { IsAuthenticated = false, ErrorMessage = "Passwords do not match." };
            }

            if(await _userManager.FindByEmailAsync(model.Email) != null)
            {
                return new AuthResponse { IsAuthenticated = false, ErrorMessage = "Email is already in use." };
            }

            var user = new User
            {
                UserName = model.FirstName, 
                Email = model.Email,
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return new AuthResponse { IsAuthenticated = false, ErrorMessage = errors };
            }
            try
            {
                await _categoryService.CreateDefaultCategoriesAsync(user.Id);
            }
            catch (Exception ex)
            {
                await _userManager.DeleteAsync(user);
                return new AuthResponse { IsAuthenticated = false, ErrorMessage = $"Failed to initialize user categories: {ex.Message}" };
            }

            var token = _jwtService.GenerateJwtToken(user);
            return new AuthResponse { IsAuthenticated = true, Token = token };

        }

        

    }
}
