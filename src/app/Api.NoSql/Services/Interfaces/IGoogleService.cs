using System.Threading.Tasks;
using Api.Services.Models;

namespace Api.Services.Interfaces
{
    public interface IGoogleService
    {
        string GetOAuthUrl();
        Task<GooglePayloadModel> ExchangeCodeForTokenAsync(string code);
    }
}
