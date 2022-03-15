using Common.Services.Infrastructure.Email.Models;

namespace Common.Services.Infrastructure.Interfaces;

public interface IEmailService
{
    void SendSignUpWelcome(SignUpWelcomeModel model);

    void SendForgotPassword(ForgotPasswordModel model);
}
