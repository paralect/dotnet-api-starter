using System.ComponentModel.DataAnnotations;

namespace Api.Models.Account
{
    public class SigninModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Incorrect email or password.")]
        public string Password { get; set; }
    }
}
