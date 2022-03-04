using System.Threading.Tasks;
using Common.Enums;

namespace Api.Services.Infrastructure
{
    public interface IAuthService
    {
        Task SetTokensAsync(string userId, UserRole userRole);
        Task UnsetTokensAsync(string userId);
    }
}
