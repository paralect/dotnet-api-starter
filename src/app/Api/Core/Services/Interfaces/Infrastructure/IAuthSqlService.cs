using System.Collections.Generic;
using System.Threading.Tasks;
using Common.DALSql.Entities;

namespace Api.Core.Services.Interfaces.Infrastructure
{
    public interface IAuthSqlService
    {
        Task SetTokensAsync(long userId);
        void SetTokens(IEnumerable<Token> tokens);
        Task UnsetTokensAsync(long userId);
        IList<Token> GenerateTokens(long userId);
    }
}