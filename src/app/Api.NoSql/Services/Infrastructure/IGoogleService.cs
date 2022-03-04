using System.Threading.Tasks;
using Api.Services.Infrastructure.Models;

namespace Api.Services.Infrastructure
{
    public interface IGoogleService
    {
        string GetOAuthUrl();
        Task<GooglePayloadModel> ExchangeCodeForTokenAsync(string code);
    }
}
