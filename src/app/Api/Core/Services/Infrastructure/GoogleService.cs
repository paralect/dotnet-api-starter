using Api.Core.Interfaces.Services.Infrastructure;
using Api.Core.Services.Infrastructure.Models;

namespace Api.Core.Services.Infrastructure
{
    public class GoogleService : IGoogleService
    {
        public string GetOAuthUrl()
        {
            throw new System.NotImplementedException();
        }

        public GoogleAuthModel ExchangeCodeForToken(string code)
        {
            throw new System.NotImplementedException();
        }
    }
}
