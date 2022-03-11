using System.Threading.Tasks;
using Api.NoSql.Services.Models;

namespace Api.NoSql.Services.Interfaces
{
    public interface IGoogleService
    {
        string GetOAuthUrl();
        Task<GooglePayloadModel> ExchangeCodeForTokenAsync(string code);
    }
}
