using System;
using System.Linq;
using System.Threading.Tasks;
using Common.DB.Mongo.DAL.Documents.User;
using Common.DB.Mongo.DAL.Interfaces;
using Common.DB.Mongo.DAL.Repositories;
using Common.DB.Mongo.DAL.UpdateDocumentOperators;
using Common.Models;
using Common.Services;
using Common.Services.EmailService;
using Common.Services.UserService;
using Common.Utils;
using LinqToDB;

namespace Common.DB.Mongo.Services
{
    public class UserService : BaseDocumentService<User, UserFilter>, IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;

        public UserService(IUserRepository userRepository,
            IEmailService emailService) : base(userRepository)
        {
            _userRepository = userRepository;
            _emailService = emailService;
        }

        public async Task<IUser?> FindByEmailAsync(string email)
        {
            return await FindOneAsync(new UserFilter { Email = email });
        }

        public async Task MarkEmailAsVerifiedAsync(Guid id)
        {
            await _userRepository.UpdateOneAsync(id, u => u.IsEmailVerified, true);
        }

        public async Task UpdateLastRequestAsync(Guid id)
        {
            await _userRepository.UpdateOneAsync(id, u => u.LastRequest, DateTime.UtcNow);
        }

        public async Task UpdateResetPasswordTokenAsync(Guid id, string token)
        {
            await _userRepository.UpdateOneAsync(id, u => u.ResetPasswordToken, token);
        }

        public async Task UpdatePasswordAsync(Guid id, string newPassword)
        {
            await _userRepository.UpdateOneAsync(id, new IUpdateOperator<User>[]
            {
                new SetOperator<User, string>(user => user.PasswordHash, newPassword.GetHash()),
                new SetOperator<User, string>(user => user.ResetPasswordToken, string.Empty)
            });
        }

        public async Task UpdateInfoAsync(Guid id, string email, string firstName, string lastName)
        {
            await _userRepository.UpdateOneAsync(id, new IUpdateOperator<User>[]
            {
                new SetOperator<User, string>(user => user.Email, email),
                new SetOperator<User, string>(user => user.FirstName, firstName),
                new SetOperator<User, string>(user => user.LastName, lastName)
            });
        }

        public async Task<IUser> CreateUserAccountAsync(CreateUserModel model)
        {
            var signUpToken = SecurityUtils.GenerateSecureToken();

            var newUser = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                PasswordHash = model.Password.GetHash(),
                Email = model.Email,
                IsEmailVerified = false,
                SignupToken = signUpToken
            };

            await _userRepository.InsertAsync(newUser);

            _emailService.SendSignUpWelcome(new SignUpWelcomeModel
            {
                Email = model.Email,
                SignUpToken = signUpToken
            });

            return newUser;
        }

        public async Task<IUser> CreateUserAccountAsync(CreateUserGoogleModel model)
        {
            var newUser = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                IsEmailVerified = true,
                OAuthGoogle = true
            };

            await _userRepository.InsertAsync(newUser);

            return newUser;
        }

        public async Task EnableGoogleAuthAsync(Guid id)
        {
            await _userRepository.UpdateOneAsync(id, u => u.OAuthGoogle, true);
        }

        public async Task<bool> IsEmailInUseAsync(Guid? userIdToExclude, string email)
        {
            var user = await _userRepository
                .FindOneAsync(new UserFilter { UserIdToExclude = userIdToExclude, Email = email });

            return user != null;
        }

        public async Task<Guid?> FindUserIDByResetPasswordTokenAsync(string resetPasswordToken)
        {
            return (await FindOneAsync(new UserFilter { ResetPasswordToken = resetPasswordToken }))?.Id;
        }

        public async Task<Guid?> FindUserIDBySignUpTokenAsync(string signUpToken)
        {
            return (await FindOneAsync(new UserFilter { SignUpToken = signUpToken }))?.Id;
        }

        async Task<IUser?> IDocumentService<IUser>.FindByIdAsync(Guid id)
        {
            return await FindByIdAsync(id);
        }
    }
}