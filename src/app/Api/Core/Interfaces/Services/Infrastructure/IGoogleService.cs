using Api.Core.Services.Infrastructure.Models;

namespace Api.Core.Interfaces.Services.Infrastructure
{
    public interface IGoogleService
    {
        string GetOAuthUrl();
        GoogleAuthModel ExchangeCodeForToken(string code);
    }
}
