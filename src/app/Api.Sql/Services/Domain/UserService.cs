using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Api.Services.Infrastructure.Models;
using Api.Services.Infrastructure;
using Api.Models.User;
using Common.DalSql;
using Common.DalSql.Entities;
using Common.DalSql.Filters;
using Common.DalSql.Repositories;
using Common.Utils;
using Api.Services.Domain.Models;

namespace Api.Services.Domain
{
    public class UserService : IUserService
    {
        private readonly IEmailService _emailService;
        private readonly IAuthService _authService;
        private readonly IUserRepository _userRepository;

        public UserService(
            IEmailService emailService,
            IAuthService authSqlService,
            IUserRepository userRepository)
        {
            _emailService = emailService;
            _authService = authSqlService;
            _userRepository = userRepository;
        }

        public async Task<User> FindByIdAsync(long id)
        {
            return await _userRepository.FindOneAsync(new UserFilter
            {
                Id = id
            });
        }

        public async Task<User> FindByEmailAsync(string email)
        {
            return await _userRepository.FindOneAsync(new UserFilter
            {
                Email = email
            });
        }

        public async Task<User> FindBySignupTokenAsync(string token)
        {
            return await _userRepository.FindOneAsync(new UserFilter
            {
                SignupToken = token
            });
        }

        public async Task<User> FindByResetPasswordTokenAsync(string token)
        {
            return await _userRepository.FindOneAsync(new UserFilter
            {
                ResetPasswordToken = token
            });
        }

        public async Task<Page<UserViewModel>> FindPageAsync(
            UserFilter filter,
            ICollection<SortField> sortFields,
            int page,
            int pageSize,
            Expression<Func<User, UserViewModel>> map)
        {
            return await _userRepository.FindPageAsync(filter, sortFields, page, pageSize, map);
        }

        public async Task<User> CreateUserAccountAsync(CreateUserModel model)
        {
            var signUpToken = SecurityUtils.GenerateSecureToken();

            var user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                PasswordHash = model.Password.GetHash(),
                Email = model.Email,
                IsEmailVerified = false,
                SignupToken = signUpToken,
                CreatedOn = DateTime.UtcNow // TODO do in repository
            };

            await _userRepository.InsertAsync(user);

            _emailService.SendSignUpWelcome(new SignUpWelcomeModel
            {
                Email = model.Email,
                SignUpToken = signUpToken
            });

            return user;
        }

        public async Task VerifyEmailAsync(long id)
        {
            var user = await _userRepository.FindById(id);
            user.IsEmailVerified = true;
            user.LastRequest = DateTime.UtcNow;

            await _authService.SetTokensAsync(id);
        }

        public async Task SignInAsync(long id)
        {
            var user = await _userRepository.FindById(id);
            user.LastRequest = DateTime.UtcNow;

            await _authService.SetTokensAsync(id);
        }

        public async Task UpdateResetPasswordTokenAsync(long id, string token)
        {
            var user = await _userRepository.FindById(id);
            user.ResetPasswordToken = token;
        }

        public async Task<string> SetResetPasswordTokenAsync(long id)
        {
            var user = await _userRepository.FindById(id);
            if (user.ResetPasswordToken.HasNoValue())
            {
                user.ResetPasswordToken = SecurityUtils.GenerateSecureToken();
            }

            return user.ResetPasswordToken;
        }

        public async Task UpdatePasswordAsync(long id, string newPassword)
        {
            var user = await _userRepository.FindById(id);
            user.PasswordHash = newPassword.GetHash();
            user.ResetPasswordToken = string.Empty;
        }

        public async Task UpdateInfoAsync(long id, string email, string firstName, string lastName)
        {
            var user = await _userRepository.FindById(id);
            user.Email = email;
            user.FirstName = firstName;
            user.LastName = lastName;
        }

        public async Task<bool> IsEmailInUseAsync(long userIdToExclude, string email)
        {
            var user = await _userRepository.FindOneAsync(new UserFilter
            {
                IdToExclude = userIdToExclude,
                Email = email,
                AsNoTracking = true
            });

            return user != null;
        }

        public async Task SignInGoogleWithCodeAsync(GooglePayloadModel payload)
        {
            var user = await _userRepository.FindOneAsync(new UserFilter
            {
                Email = payload.Email,
                IncludeProperties = new List<Expression<Func<User, object>>>
                {
                    u => u.Tokens
                }
            });

            if (user == null)
            {
                user = new User
                {
                    FirstName = payload.GivenName,
                    LastName = payload.FamilyName,
                    Email = payload.Email,
                    IsEmailVerified = true,
                    OAuthGoogle = true
                };

                await _userRepository.InsertAsync(user);
            }
            else
            {
                if (!user.OAuthGoogle)
                {
                    user.OAuthGoogle = true;
                    user.LastRequest = DateTime.UtcNow;
                }
            }

            _authService.SetTokens(user);
        }
    }
}