using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Core.Services.Domain.Models;
using Api.Core.Services.Infrastructure.Models;
using Common.DALSql.Entities;

namespace Api.Core.Services.Interfaces.Domain
{
    public interface IUserSqlService
    {
        Task<User> CreateUserAccountAsync(CreateUserModel model);
        Task VerifyEmail(long userId);
        Task SignIn(long userId);

        Task UpdateResetPasswordTokenAsync(long id, string token);
        Task UpdatePasswordAsync(long id, string newPassword);
        // Task UpdateInfoAsync(long id, string email, string firstName, string lastName);
        // Task MarkEmailAsVerifiedAsync(long id);
        //
        // Task<bool> IsEmailInUseAsync(long userIdToExclude, string email);
        Task<string> SetResetPasswordToken(long userId);

        public IList<Token> GenerateTokens(long userId);
        Task SignInGoogleWithCodeAsync(GooglePayloadModel payload);
    }
}