using System.Threading.Tasks;
using Common.DalSql.Entities;
using Common.DalSql.Filters;
using Common.ServicesSql.Domain.Models;

namespace Common.DalSql.Interfaces;

public interface ITokenRepository : IRepository<Token, TokenFilter>
{
    Task<UserTokenModel> FindUserTokenByValueAsync(string value);
}
