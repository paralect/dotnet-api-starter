using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Services.Document.Models;
using Common.DAL;
using Common.DAL.Documents.User;
using Common.DAL.Repositories;

namespace Api.Services.Document
{
    public interface IUserService : IDocumentService<User, UserFilter>
    {
        Task<User> FindByEmailAsync(string email);
        Task<Page<User>> FindPageAsync(
            UserFilter filter,
            IList<(string, SortDirection)> sortFields,
            int page,
            int pageSize
        );

        Task<User> CreateUserAccountAsync(CreateUserModel model);
        Task<User> CreateUserAccountAsync(CreateUserGoogleModel model);

        Task UpdateLastRequestAsync(string id);
        Task UpdateResetPasswordTokenAsync(string id, string token);
        Task UpdatePasswordAsync(string id, string newPassword);
        Task MarkEmailAsVerifiedAsync(string id);
        Task EnableGoogleAuthAsync(string id);

        Task<bool> IsEmailInUseAsync(string userIdToExclude, string email);
    }
}
