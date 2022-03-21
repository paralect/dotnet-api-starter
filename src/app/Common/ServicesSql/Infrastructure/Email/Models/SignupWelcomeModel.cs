namespace Common.ServicesSql.Infrastructure.Email.Models;

public class SignUpWelcomeModel
{
    public string Email { get; set; }
    public string SignUpToken { get; set; }
}
