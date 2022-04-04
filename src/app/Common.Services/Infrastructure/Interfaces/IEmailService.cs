using Api.Views.Models.Infrastructure.Email;

namespace Common.Services.Infrastructure.Interfaces;

public interface IEmailService
{
    Task SendSignUpWelcomeAsync(SignUpWelcomeModel model);
    Task SendForgotPasswordAsync(ForgotPasswordModel model);
}
