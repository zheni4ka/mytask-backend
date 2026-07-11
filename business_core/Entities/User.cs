using Microsoft.AspNetCore.Identity;

namespace Core.Entities
{
    public class User : IdentityUser
    {
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }

        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Assignment> Assignments { get; set; }
    }
}
