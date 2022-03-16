using System.Threading.Tasks;
using Api.Sql.Services.Models;

namespace Api.Sql.Services.Interfaces
{
    public interface IGoogleService
    {
        string GetOAuthUrl();
        Task<GooglePayloadModel> ExchangeCodeForTokenAsync(string code);
    }
}
