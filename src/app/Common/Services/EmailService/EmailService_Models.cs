using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Services.EmailService
{
    public class SignUpWelcomeModel
    {
        public string Email { get; set; }
        public string SignUpToken { get; set; }
    }

    public class ForgotPasswordModel
    {
        public string Email { get; set; }
        public string ResetPasswordUrl { get; set; }
        public string FirstName { get; set; }
    }
}
