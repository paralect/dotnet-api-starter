using System.Threading.Tasks;
using Common.Models;

namespace Common.Services.UserService
{
    public interface IUserService : IDocumentService<IUser>
    {
        Task<IUser> CreateUserAccountAsync(CreateUserModel model);
        Task<IUser> CreateUserAccountAsync(CreateUserGoogleModel model);

        Task<IUser> FindByEmailAsync(string email);

        Task UpdateLastRequestAsync(string id);
        Task UpdateResetPasswordTokenAsync(string id, string token);
        Task UpdatePasswordAsync(string id, string newPassword);
        Task UpdateInfoAsync(string id, string email, string firstName, string lastName);
        Task MarkEmailAsVerifiedAsync(string id);
        Task EnableGoogleAuthAsync(string id);
        Task<string> FindUserIdByResetPasswordTokenAsync(string resetPasswordToken);
        Task<string> FindUserIdBySignUpTokenAsync(string signUpToken);

        Task<bool> IsEmailInUseAsync(string userIdToExclude, string email);
    }
}
