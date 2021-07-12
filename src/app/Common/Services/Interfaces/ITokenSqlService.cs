using System.Collections.Generic;
using System.Threading.Tasks;
using Common.DALSql.Entities;

namespace Common.Services.Interfaces
{
    public interface ITokenSqlService
    {
        Task<Token> FindByIdAsync(long id);
        //Task<TDocument> FindOneAsync(TFilter filter);
        Task<List<Token>> CreateAuthTokensAsync(long userId);
        Task<Token> FindByValueAsync(string tokenValue);
        Task DeleteUserTokensAsync(long userId);
    }
}