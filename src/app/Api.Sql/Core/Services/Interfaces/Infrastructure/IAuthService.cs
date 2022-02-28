using System.Threading.Tasks;
using Common.DALSql.Entities;

namespace Api.Core.Services.Interfaces.Infrastructure
{
    public interface IAuthService
    {
        void SetTokens(long userId);
        void SetTokens(User user);
        Task UnsetTokensAsync(long userId);
    }
}