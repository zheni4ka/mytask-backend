using Core.DTOs;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace business_logic.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ICategoryService _categoryService;

        public AuthService(UserManager<User> userManager, IConfiguration configuration, ICategoryService categoryService)
        {
            _userManager = userManager;
            _configuration = configuration;
            _categoryService = categoryService;
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

            var token = GenerateJwtToken(user);

            return new AuthResponse { IsAuthenticated = true, Token = token };
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

            var token = GenerateJwtToken(user);
            return new AuthResponse { IsAuthenticated = true, Token = token };

        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["Secret"];
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];


            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id), 
                new Claim(ClaimTypes.Email, user.Email!)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            const int TokenExpirationDays = 3;

            var tokenDescriptor = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddDays(TokenExpirationDays),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

    }
}
