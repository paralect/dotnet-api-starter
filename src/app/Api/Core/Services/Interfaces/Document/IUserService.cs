using System.Threading.Tasks;
using Api.Core.Services.Document.Models;
using Common.DAL.Documents;
using Common.Services.Interfaces;

namespace Api.Core.Services.Interfaces.Document
{
    public interface IUserService : IDocumentService<User>
    {
        Task<User> CreateUserAccountAsync(CreateUserModel model);
        Task<User> CreateUserAccountAsync(CreateUserGoogleModel model);

        Task<User> FindByEmailAsync(string email);

        Task UpdateLastRequestAsync(long id);
        Task UpdateResetPasswordTokenAsync(long id, string token);
        Task UpdatePasswordAsync(long id, string newPassword);
        Task UpdateInfoAsync(long id, string email, string firstName, string lastName);
        Task MarkEmailAsVerifiedAsync(long id);
        Task EnableGoogleAuthAsync(long id);
        Task<long?> FindUserIDByResetPasswordTokenAsync(string resetPasswordToken);
        Task<long?> FindUserIDBySignUpTokenAsync(string signUpToken);

        Task<bool> IsEmailInUseAsync(long? userIdToExclude, string email);
    }
}
