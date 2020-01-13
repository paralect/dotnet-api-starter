using System.Threading.Tasks;
using Api.Core.Services.Infrastructure.Models;

namespace Api.Core.Interfaces.Services.Infrastructure
{
    public interface IGoogleService
    {
        string GetOAuthUrl();
        Task<GooglePayloadModel> ExchangeCodeForTokenAsync(string code);
    }
}
