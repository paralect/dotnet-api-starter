using System.Threading.Tasks;
using Api.Core.Services.Document.Models;
using Common.DAL.Documents.User;
using Common.DAL.Repositories;
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
        Task UpdateInfoAsync(string id, string email, string firstName, string lastName);
        Task MarkEmailAsVerifiedAsync(string id);
        Task EnableGoogleAuthAsync(string id);

        Task<bool> IsEmailInUseAsync(string userIdToExclude, string email);
    }
}
