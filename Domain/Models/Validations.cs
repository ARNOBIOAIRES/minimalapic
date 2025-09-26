using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace minimalApi.Domain.Models
{
    public struct Validations

    {
         public List<string> Mensagens { get; set; } 
        // public static bool ValidateEmail(string email)
        // {
        //     // Simple email validation logic
        //     return email.Contains("@") && email.Contains(".");
        // }

        // public static bool ValidatePhoneNumber(string phoneNumber)
        // {
        //     // Simple phone number validation logic
        //     return phoneNumber.All(char.IsDigit) && phoneNumber.Length == 10;
        // }
    }
}