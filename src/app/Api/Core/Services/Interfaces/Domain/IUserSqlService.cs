using System.Threading.Tasks;
using Api.Core.Services.Domain.Models;
using Common.DALSql.Models;

namespace Api.Core.Services.Interfaces.Domain
{
    public interface IUserSqlService
    {
        Task<User> FindByIdAsync(long id);
        Task<User> FindBySignupTokenAsync(string token);
        Task<User> FindByResetPasswordTokenAsync(string token);

        //Task<User> FindOneAsync(TFilter filter);

        Task<User> CreateUserAccountAsync(CreateUserModel model);
        Task<User> CreateUserAccountAsync(CreateUserGoogleModel model);

        Task<User> FindByEmailAsync(string email);

        Task UpdateLastRequestAsync(long id);
        Task UpdateResetPasswordTokenAsync(long id, string token);
        Task UpdatePasswordAsync(long id, string newPassword);
        Task UpdateInfoAsync(long id, string email, string firstName, string lastName);
        Task MarkEmailAsVerifiedAsync(long id);
        Task EnableGoogleAuthAsync(long id);

        Task<bool> IsEmailInUseAsync(long userIdToExclude, string email);
    }
}