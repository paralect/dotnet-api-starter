﻿namespace Api.Services.Infrastructure.Models
{
    public class ForgotPasswordModel
    {
        public string Email { get; set; }
        public string ResetPasswordUrl { get; set; }
        public string FirstName { get; set; }
    }
}
