using Common.DalSql.Entities;
using Common.DalSql.Filters;

namespace Common.Services.ServicesSql.Domain.Interfaces;

public interface ITokenService : IEntityService<Token, TokenFilter>
{
}
