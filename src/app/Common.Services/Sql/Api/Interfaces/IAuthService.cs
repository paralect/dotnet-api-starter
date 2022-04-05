namespace Common.Services.Sql.Api.Interfaces;

public interface IAuthService
{
    Task SetTokensAsync(long userId);
    Task UnsetTokensAsync(long userId);
}
