using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_api_starter.Infrastructure.Abstract
{
    public interface IEmailService
    {
        void SendSignupWelcome(object data);
        void SendForgotPassword(object data);
    }
}
