using System.Threading.Tasks;
using Common.DalSql.Entities;
using Common.DalSql.Filters;
using Common.ServicesSql.Domain.Models;

namespace Common.ServicesSql.Domain.Interfaces;

public interface IUserService : IEntityService<User, UserFilter>
{
    Task<User> FindByEmailAsync(string email);
    Task<User> FindBySignupTokenAsync(string token);
    Task<User> FindByResetPasswordTokenAsync(string token);
    Task<bool> IsEmailInUseAsync(long idToExclude, string email);

    Task<User> CreateUserAccountAsync(CreateUserModel model);
    Task<User> CreateUserAccountAsync(CreateUserGoogleModel model);

    Task VerifyEmailAsync(long id);
    Task EnableGoogleAuthAsync(User user);
    Task SignInAsync(long id);
    Task UpdateResetPasswordTokenAsync(long id, string token);
    Task UpdatePasswordAsync(long id, string newPassword);
    Task UpdateInfoAsync(long id, string email, string firstName, string lastName);
    Task<string> SetResetPasswordTokenAsync(long id);
}
