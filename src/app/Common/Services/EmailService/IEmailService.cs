namespace Common.Services.EmailService
{
    public interface IEmailService
    {
        void SendSignUpWelcome(SignUpWelcomeModel model);

        void SendForgotPassword(ForgotPasswordModel model);
    }
}
