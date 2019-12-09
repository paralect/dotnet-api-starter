namespace Api.Core.Interfaces.Services.App
{
    public interface IEmailService
    {
        void SendSignupWelcome(object data);

        void SendForgotPassword(object data);
    }
}
