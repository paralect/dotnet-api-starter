using System.Threading.Tasks;
using Api.Core.DAL.Views.User;
using Api.Core.Services.App.Models;

namespace Api.Core.Interfaces.Services.App
{
    public interface IUserService
    {
        Task<User> CreateUserAccountAsync(CreateUserModel model);
        Task<User> CreateUserAccountAsync(CreateUserGoogleModel model);

        Task<User> FindByIdAsync(string id);
        Task<User> FindByEmailAsync(string email);
        Task<User> FindBySignupTokenAsync(string signupToken);
        Task<User> FindByResetPasswordTokenAsync(string resetPasswordToken);

        Task UpdateLastRequestAsync(string id);
        Task UpdateResetPasswordTokenAsync(string id, string token);
        Task UpdatePasswordAsync(string id, string newPassword);
        Task UpdateInfoAsync(string id, string email, string firstName, string lastName);
        Task MarkEmailAsVerifiedAsync(string id);
        Task EnableGoogleAuthAsync(string id);

        Task<bool> IsEmailInUseAsync(string userIdToExclude, string email);
    }
}
