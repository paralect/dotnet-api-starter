using Api.Core.Services.Infrastructure.Models;

namespace Api.Core.Services.Interfaces.Infrastructure
{
    public interface IEmailService
    {
        void SendSignUpWelcome(SignUpWelcomeModel model);

        void SendForgotPassword(ForgotPasswordModel model);
    }
}
