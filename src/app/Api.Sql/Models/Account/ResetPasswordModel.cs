using System.ComponentModel.DataAnnotations;

namespace Api.Models.Account
{
    public class ResetPasswordModel
    {
        [Required(ErrorMessage = "Password is required.")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Password must be 6-20 characters.")]
        public string Password { get; set; }

        public string Token { get; set; }
    }
}
