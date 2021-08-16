using System;
using System.Threading.Tasks;

namespace Api.Core.Services.Interfaces.Infrastructure
{
    public interface IAuthService
    {
        Task SetTokensAsync(Guid userId);
        Task UnsetTokensAsync(Guid userId);
    }
}
