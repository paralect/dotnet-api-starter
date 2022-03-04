using Api.Services.Infrastructure.Models;

namespace Api.Services.Infrastructure
{
    public interface IEmailService
    {
        void SendSignUpWelcome(SignUpWelcomeModel model);

        void SendForgotPassword(ForgotPasswordModel model);
    }
}
