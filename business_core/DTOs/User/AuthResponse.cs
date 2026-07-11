namespace Core.DTOs
{
    public class AuthResponse
    {
        public bool IsAuthenticated { get; set; }
        public string? ErrorMessage { get; set; }
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
    }
}
