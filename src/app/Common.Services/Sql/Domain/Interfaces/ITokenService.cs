using Common.DalSql.Entities;
using Common.DalSql.Filters;

namespace Common.Services.Sql.Domain.Interfaces;

public interface ITokenService : IEntityService<Token, TokenFilter>
{
}
