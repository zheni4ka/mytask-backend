using Core.Entities;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Core.Interfaces
{
    public interface IJwtService
    {
        string GenerateRefreshToken();
        string GenerateJwtToken(User user);
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token);
    }
}
