using System.Threading.Tasks;
using Api.Core.Services.Domain.Models;
using Api.Core.Services.Infrastructure.Models;
using Common.DALSql.Entities;

namespace Api.Core.Services.Interfaces.Domain
{
    public interface IUserSqlService
    {
        User CreateUserAccount(CreateUserModel model);
        Task VerifyEmailAsync(long userId);
        Task SignInAsync(long userId);
        Task SignInGoogleWithCodeAsync(GooglePayloadModel payload);
        Task UpdateResetPasswordTokenAsync(long id, string token);
        Task UpdatePasswordAsync(long id, string newPassword);
        Task UpdateInfoAsync(long id, string email, string firstName, string lastName);
        Task<bool> IsEmailInUseAsync(long userIdToExclude, string email);
        Task<string> SetResetPasswordTokenAsync(long userId);
    }
}