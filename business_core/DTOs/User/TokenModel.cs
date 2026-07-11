using System;
using System.Collections.Generic;
using System.Text;

namespace Core.DTOs.User
{
    public class TokenModel
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
