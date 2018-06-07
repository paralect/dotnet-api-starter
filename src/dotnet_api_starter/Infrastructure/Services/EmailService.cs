using dotnet_api_starter.Infrastructure.Abstract;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_api_starter.Infrastructure.Services
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
