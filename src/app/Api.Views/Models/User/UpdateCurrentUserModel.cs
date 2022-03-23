using System.ComponentModel.DataAnnotations;

namespace Api.Views.Models.User;

public class UpdateCurrentUserModel
{
    [Required(ErrorMessage = "Password is required.")]
    [StringLength(20, MinimumLength = 6, ErrorMessage = "Password must be 6-20 characters.")]
    public string Password { get; set; }
}
