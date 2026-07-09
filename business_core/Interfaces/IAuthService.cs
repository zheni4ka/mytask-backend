using Core.DTOs;

namespace Core.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(LoginModel model);
        Task<AuthResponse> RegisterAsync(RegisterModel model);
    }
}
