using System.Threading.Tasks;
using Common.Enums;

namespace Common.Services.NoSql.Api.Interfaces;

public interface IAuthService
{
    Task SetTokensAsync(string userId, UserRole userRole);
    Task UnsetTokensAsync(string userId);
}
