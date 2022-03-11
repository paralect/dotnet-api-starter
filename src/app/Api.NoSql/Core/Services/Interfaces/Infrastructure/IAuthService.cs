using System.Threading.Tasks;

namespace Api.Core.Services.Interfaces.Infrastructure
{
    public interface IAuthService
    {
        Task SetTokensAsync(string userId);
        Task UnsetTokensAsync(string userId);
    }
}
