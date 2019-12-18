using System.Threading.Tasks;

namespace Api.Core.Interfaces.Services.App
{
    public interface IAuthService
    {
        Task SetTokensAsync(string userId);
        Task UnsetTokensAsync(string userId);
    }
}
