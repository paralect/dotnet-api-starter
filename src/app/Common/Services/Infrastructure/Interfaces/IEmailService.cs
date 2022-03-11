using Common.Services.Infrastructure.Models;

namespace Common.Services.Infrastructure.Interfaces;

public interface IEmailService
{
    void SendSignUpWelcome(SignUpWelcomeModel model);

    void SendForgotPassword(ForgotPasswordModel model);
}
