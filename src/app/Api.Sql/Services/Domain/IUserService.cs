using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Api.Services.Infrastructure.Models;
using Api.Models.User;
using Common.DALSql;
using Common.DALSql.Entities;
using Common.DALSql.Filters;
using Api.Services.Domain.Models;

namespace Api.Services.Domain
{
    public interface IUserService
    {
        Task<User> FindByIdAsync(long id);
        Task<User> FindByEmailAsync(string email);
        Task<User> FindBySignupTokenAsync(string token);
        Task<User> FindByResetPasswordTokenAsync(string token);
        Task<Page<UserViewModel>> FindPageAsync(
            UserFilter filter,
            ICollection<SortField> sortFields,
            int page,
            int pageSize,
            Expression<Func<User, UserViewModel>> map
        );
        Task<bool> IsEmailInUseAsync(long idToExclude, string email);


        Task<User> CreateUserAccountAsync(CreateUserModel model);
        Task VerifyEmailAsync(long id);
        Task SignInAsync(long id);
        Task SignInGoogleWithCodeAsync(GooglePayloadModel payload);
        Task UpdateResetPasswordTokenAsync(long id, string token);
        Task UpdatePasswordAsync(long id, string newPassword);
        Task UpdateInfoAsync(long id, string email, string firstName, string lastName);
        Task<string> SetResetPasswordTokenAsync(long id);
    }
}