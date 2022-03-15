﻿using Common.Services.Infrastructure.Email.Models;
using Common.Services.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;

namespace Common.Services.Infrastructure.Email;

public class EmailService : IEmailService
{
    private readonly ILogger _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public void SendSignUpWelcome(SignUpWelcomeModel model)
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
