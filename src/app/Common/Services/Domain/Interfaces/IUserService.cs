using System.Threading.Tasks;
using Common.Dal.Documents.User;
using Common.Dal.Repositories;
using Common.Services.Domain.Models;

namespace Common.Services.Domain.Interfaces;

public interface IUserService : IDocumentService<User, UserFilter>
{
    Task<User> FindByEmailAsync(string email);
    Task<bool> IsEmailInUseAsync(string userIdToExclude, string email);

    Task<User> CreateUserAccountAsync(CreateUserModel model);

    Task UpdateLastRequestAsync(string id);
    Task UpdateResetPasswordTokenAsync(string id, string token);
    Task UpdatePasswordAsync(string id, string newPassword);
    Task MarkEmailAsVerifiedAsync(string id);
}
