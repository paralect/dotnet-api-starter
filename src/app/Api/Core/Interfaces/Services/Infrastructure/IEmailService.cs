using Api.Core.Services.Infrastructure.Models;

namespace Api.Core.Interfaces.Services.Infrastructure
{
    public interface IEmailService
    {
        void SendSignupWelcome(SignupWelcomeModel model);

        void SendForgotPassword(ForgotPasswordModel model);
    }
}
