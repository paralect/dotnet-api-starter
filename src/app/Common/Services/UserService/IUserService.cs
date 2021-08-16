using System;
using System.Threading.Tasks;
using Common.Models;

namespace Common.Services.UserService
{
    public interface IUserService : IDocumentService<IUser>
    {
        Task<IUser> CreateUserAccountAsync(CreateUserModel model);
        Task<IUser> CreateUserAccountAsync(CreateUserGoogleModel model);

        Task<IUser?> FindByEmailAsync(string email);

        Task UpdateLastRequestAsync(Guid id);
        Task UpdateResetPasswordTokenAsync(Guid id, string token);
        Task UpdatePasswordAsync(Guid id, string newPassword);
        Task UpdateInfoAsync(Guid id, string email, string firstName, string lastName);
        Task MarkEmailAsVerifiedAsync(Guid id);
        Task EnableGoogleAuthAsync(Guid id);
        Task<Guid?> FindUserIDByResetPasswordTokenAsync(string resetPasswordToken);
        Task<Guid?> FindUserIDBySignUpTokenAsync(string signUpToken);

        Task<bool> IsEmailInUseAsync(Guid? userIdToExclude, string email);
    }
}
