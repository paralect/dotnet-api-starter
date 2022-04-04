using Api.Views.Models.Infrastructure.Email;
using Common.Services.Infrastructure.Interfaces;
using Common.Settings;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace Common.Services.Infrastructure.Email;

public class EmailService : IEmailService
{
    private readonly ILogger _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public async Task SendSignUpWelcomeAsync(SignUpWelcomeModel model)
    {
        await SendEmailAsync(
            model.Email,
            model.FirstName,
            "Sign Up",
            $"<a href=http://localhost:3001/account/verify-email?token={model.SignUpToken}>Link</a> to verify email." // TODO replace host
        );
    }

    public async Task SendForgotPasswordAsync(ForgotPasswordModel model)
    {
        await SendEmailAsync(
            model.Email,
            model.FirstName,
            "Forgot Password",
            $"<a href={model.ResetPasswordUrl}>Link</a> to reset password."
        );
    }

    private async Task SendEmailAsync(string toEmail, string toName, string subject, string body)
    {
        var message = new MimeMessage
        {
            Subject = subject,
            Body = new TextPart("html")
            {
                Text = body
            }
        };
        message.From.Add(new MailboxAddress("Paralect", "ship@paralect.com"));
        message.To.Add(new MailboxAddress(toName, toEmail));

        using (var client = new SmtpClient())
        {
            await client.ConnectAsync("localhost", 25, false);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

        _logger.LogDebug(
            $"Sending email to {toName} ({toEmail}). Subject: {subject}. Body: {body}"
        );
    }
}
