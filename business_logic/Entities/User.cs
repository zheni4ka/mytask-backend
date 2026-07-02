using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace business_logic.Entities
{
    public class User : IdentityUser
    {
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Assignment> Assignments { get; set; }
    }
}
