using Api.Core.Interfaces.Services.App;
using Microsoft.Extensions.Logging;

namespace Api.Core.Services.App
{
    public class EmailService : IEmailService
    {
        private readonly ILogger _logger;

        public EmailService(ILogger<EmailService> logger)
        {
            _logger = logger;
        }

        public void SendForgotPassword(object data)
        {
            SendEmail("signup-welcome", data);
        }

        public void SendSignupWelcome(object data)
        {
            SendEmail("forgot-password", data);
        }

        private void SendEmail(string template, object data)
        {
            _logger.LogDebug($"Sending email {template}. The data is {data}");
        }
    }
}
