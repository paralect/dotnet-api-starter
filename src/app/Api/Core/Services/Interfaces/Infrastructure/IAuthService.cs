using System.Threading.Tasks;

namespace Api.Core.Services.Interfaces.Infrastructure
{
    public interface IAuthService
    {
        Task SetTokensAsync(long userId);
        Task UnsetTokensAsync(long userId);
    }
}
