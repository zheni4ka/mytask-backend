using Ardalis.Specification;
using Core.DTOs;
using Core.DTOs.User;
using Core.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace mytask_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase 
    {
        private readonly IAuthService _authService;
        private readonly IValidator<RegisterModel> _registerValidator;

        public AuthController(IAuthService authService, IValidator<RegisterModel> registerValidator)
        {
            _authService = authService;
            _registerValidator = registerValidator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var validationResult = await _registerValidator.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var result = await _authService.RegisterAsync(model);

            if (!result.IsAuthenticated)
            {
                return BadRequest(new { message = result.ErrorMessage });
            }

            return Ok(new { token = result.Token, refreshToken = result.RefreshToken, message = "Registration successful!" });
        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            if (string.IsNullOrEmpty(request.IdToken))
            {
                return BadRequest(new AuthResponse
                {
                    IsAuthenticated = false,
                    ErrorMessage = "Token not found"
                });
            }

            var result = await _authService.LoginWithGoogleAsync(request);

            if (!result.IsAuthenticated)
            {
                return Unauthorized(result); 
            }

            return Ok(result); 
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var result = await _authService.LoginAsync(model);

            if (!result.IsAuthenticated)
            {
                return Unauthorized(new { message = result.ErrorMessage });
            }

            return Ok(new { token = result.Token, refreshToken = result.RefreshToken });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenModel tokenModel)
        {
            if (tokenModel == null)
            {
                return BadRequest("Invalid client request");
            }

            var result = await _authService.RefreshTokenAsync(tokenModel);

            if (!result.IsAuthenticated)
            {
                return Unauthorized(new { message = result.ErrorMessage });
            }

            return Ok(new { token = result.Token, refreshToken = result.RefreshToken });
        }

    }
}