using Common.DalSql.Entities;
using Common.DalSql.Filters;
using Common.DalSql.Interfaces;
using Common.Services.Sql.Domain.Interfaces;

namespace Common.Services.Sql.Domain;

public class TokenService : BaseEntityService<Token, TokenFilter>, ITokenService
{
    public TokenService(ITokenRepository tokenRepository) : base(tokenRepository)
    {
    }
}
