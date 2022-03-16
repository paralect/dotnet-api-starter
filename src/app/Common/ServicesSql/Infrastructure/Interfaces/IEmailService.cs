using Common.ServicesSql.Infrastructure.Email.Models;

namespace Common.ServicesSql.Infrastructure.Interfaces
{
    public interface IEmailService
    {
        void SendSignUpWelcome(SignUpWelcomeModel model);
        void SendForgotPassword(ForgotPasswordModel model);
    }
}
