using System.Threading.Tasks;
using Common.DalSql.Entities;

namespace Api.Services.Infrastructure
{
    public interface IAuthService
    {
        Task SetTokensAsync(long userId);
        void SetTokens(User user);
        Task UnsetTokensAsync(long userId);
    }
}