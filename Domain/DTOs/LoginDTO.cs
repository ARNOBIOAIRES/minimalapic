using System;

namespace minimalApi.Domain.DTOs
{
    public class LoginDTO
    {
        public string email { get; set; }  = string.Empty;
        public string password { get; set; } = string.Empty;
    }
}