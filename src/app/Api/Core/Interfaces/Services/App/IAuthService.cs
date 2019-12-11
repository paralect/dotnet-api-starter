using System.Threading.Tasks;

namespace Api.Core.Interfaces.Services.App
{
    public interface IAuthService
    {
        Task SetTokens(string userId);
        Task UnsetTokens(string userId);
    }
}
