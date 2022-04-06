using Api.Views.Models.Infrastructure.Email;

namespace Common.Services.Infrastructure.Interfaces;

public interface IEmailService
{
    Task SendSignUpWelcomeAsync(SignUpModel model);
    Task SendForgotPasswordAsync(ForgotPasswordModel model);
}
