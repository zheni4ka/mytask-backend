using Core.DTOs;
using Core.DTOs.User;

namespace Core.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(LoginModel model);
        Task<AuthResponse> LoginWithGoogleAsync(GoogleLoginRequest request);
        Task<AuthResponse> RegisterAsync(RegisterModel model);
        Task<AuthResponse> RefreshTokenAsync(TokenModel tokenModel);
    }
}
