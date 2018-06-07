using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_api_starter.Models.Account
{
    public class ResetPasswordModel
    {
        [Required(ErrorMessage = "Password is required.")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Password must be 6-20 characters.")]
        public string Password { get; set; }

        public string Token { get; set; }
    }
}
