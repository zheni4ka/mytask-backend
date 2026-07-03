using Microsoft.AspNetCore.Identity;

namespace business_logic.Entities
{
    public class User : IdentityUser
    {
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Assignment> Assignments { get; set; }
    }
}
