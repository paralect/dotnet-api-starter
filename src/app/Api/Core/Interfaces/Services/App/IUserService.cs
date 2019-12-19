using System.Threading.Tasks;
using Api.Core.DAL.Views.User;
using Api.Core.Services.App.Models;

namespace Api.Core.Interfaces.Services.App
{
    public interface IUserService
    {
        User FindByEmail(string email);
        User FindBySignupToken(string signupToken);
        User FindByResetPasswordToken(string resetPasswordToken);

        Task MarkEmailAsVerifiedAsync(string id);

        Task UpdateLastRequestAsync(string id);
        Task UpdateResetPasswordTokenAsync(string id, string token);
        Task UpdatePasswordAsync(string id, string newPassword);
        Task UpdateInfoAsync(string id, string email, string firstName, string lastName);

        Task<User> CreateUserAccountAsync(CreateUserModel model);
        Task<User> CreateUserAccountAsync(CreateUserGoogleModel model);

        Task EnableGoogleAuthAsync(string id);
    }
}
