using System.Collections.Generic;
using System.Threading.Tasks;
using Common.DALSql.Entities;

namespace Api.Core.Services.Interfaces.Infrastructure
{
    public interface IAuthSqlService
    {
        void SetTokens(long userId);
        Task SetTokens(User user);
        Task UnsetTokensAsync(long userId);
    }
}