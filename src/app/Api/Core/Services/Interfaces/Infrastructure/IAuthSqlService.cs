using System.Threading.Tasks;

namespace Api.Core.Services.Interfaces.Infrastructure
{
    public interface IAuthSqlService
    {
        Task SetTokensAsync(long userId);
        Task UnsetTokensAsync(long userId);
    }
}