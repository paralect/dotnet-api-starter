using System.Threading.Tasks;
using Api.Core.DbViews.User;
using Api.Core.Services.App.Models;

namespace Api.Core.Interfaces.Services.App
{
    public interface IUserService
    {
        User FindByEmail(string email);
        User FindBySignupToken(string signupToken);
        User FindByResetPasswordToken(string resetPasswordToken);

        Task MarkEmailAsVerified(string id);

        Task UpdateLastRequest(string id);

        Task UpdateResetPasswordToken(string id, string token);

        Task UpdatePassword(string id, string newPassword);

        Task UpdateInfo(string id, string email, string firstName, string lastName);

        Task<User> CreateUserAccount(CreateUserModel model);
        Task<User> CreateUserAccount(CreateUserGoogleModel model);
    }
}
