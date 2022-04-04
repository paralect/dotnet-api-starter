namespace Api.Views.Models.Infrastructure.Email;

public class SignUpWelcomeModel
{
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string SignUpToken { get; set; }
}
