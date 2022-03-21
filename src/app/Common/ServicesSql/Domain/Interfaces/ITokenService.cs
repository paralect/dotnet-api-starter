using System.Threading.Tasks;
using Common.DalSql.Entities;
using Common.DalSql.Filters;
using Common.ServicesSql.Domain.Models;

namespace Common.ServicesSql.Domain.Interfaces;

public interface ITokenService : IEntityService<Token, TokenFilter>
{
    Task<UserTokenModel> FindUserTokenByValueAsync(string value);
}
