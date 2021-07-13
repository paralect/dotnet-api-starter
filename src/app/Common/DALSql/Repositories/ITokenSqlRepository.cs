using System.Collections.Generic;
using System.Threading.Tasks;
using Common.DALSql.Entities;

namespace Common.DALSql.Repositories
{
    public interface ITokenSqlRepository : IRepository<Token>
    {
        Task<Token> FindByValue(string value);
        Task<IEnumerable<Token>> FindByUserId(long userId);
    }
}