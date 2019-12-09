using Api.Core.Models.User;
using System.Threading.Tasks;

namespace Api.Core.Abstract
{
    public interface IUserService
    {
        Task<bool> MarkEmailAsVerified(string id);

        Task<bool> UpdateResetPasswordToken(string id, string token);

        Task<bool> UpdatePassword(string id, string newPassword);

        Task<bool> UpdateInfo(string id, string email, string firstName, string lastName);

        Task<User> CreateUserAccount(string email, string firstName, string lastName, string password);
    }
}
