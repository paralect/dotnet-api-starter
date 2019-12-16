using Api.Core.Interfaces.Services.App;
using Api.Core.Services.Infrastructure.Models;
using Microsoft.Extensions.Logging;

namespace Api.Core.Services.Infrastructure
{
    public class EmailService : IEmailService
    {
        private readonly ILogger _logger;

        public EmailService(ILogger<EmailService> logger)
        {
            _logger = logger;
        }

        public void SendSignupWelcome(SignupWelcomeModel model)
        {
            SendEmail("signup-welcome", model);
        }

        public void SendForgotPassword(ForgotPasswordModel model)
        {
            SendEmail("forgot-password", model);
        }

        private void SendEmail(string template, object data)
        {
            _logger.LogDebug($"Sending email {template}. The data is {data}");
        }
    }
}
