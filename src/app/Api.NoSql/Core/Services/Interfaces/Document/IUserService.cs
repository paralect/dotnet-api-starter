using System.Threading.Tasks;
using Api.Core.Services.Document.Models;
using Common.Dal.Documents.User;
using Common.Dal.Repositories;
using Common.Services.Interfaces;

namespace Api.Core.Services.Interfaces.Document
{
    public interface IUserService : IDocumentService<User, UserFilter>
    {
        Task<User> CreateUserAccountAsync(CreateUserModel model);
        Task<User> CreateUserAccountAsync(CreateUserGoogleModel model);

        Task<User> FindByEmailAsync(string email);

        Task UpdateLastRequestAsync(string id);
        Task UpdateResetPasswordTokenAsync(string id, string token);
        Task UpdatePasswordAsync(string id, string newPassword);
        Task MarkEmailAsVerifiedAsync(string id);
        Task EnableGoogleAuthAsync(string id);

        Task<bool> IsEmailInUseAsync(string userIdToExclude, string email);
    }
}
