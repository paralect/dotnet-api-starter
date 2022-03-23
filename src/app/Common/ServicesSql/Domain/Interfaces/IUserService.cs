using System.Threading.Tasks;
using Common.DalSql.Entities;
using Common.DalSql.Filters;
using Common.ServicesSql.Domain.Models;

namespace Common.ServicesSql.Domain.Interfaces;

public interface IUserService : IEntityService<User, UserFilter>
{
    Task<User> CreateUserAccountAsync(CreateUserModel model);

    Task VerifyEmailAsync(long id);
    Task SignInAsync(long id);
    Task UpdatePasswordAsync(long id, string newPassword);
    Task<string> SetResetPasswordTokenAsync(long id);
}
