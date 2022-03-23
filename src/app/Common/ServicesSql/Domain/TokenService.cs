using Common.DalSql.Entities;
using Common.DalSql.Filters;
using Common.DalSql.Interfaces;
using Common.ServicesSql.Domain.Interfaces;

namespace Common.ServicesSql.Domain;

public class TokenService : BaseEntityService<Token, TokenFilter>, ITokenService
{
    public TokenService(ITokenRepository tokenRepository) : base(tokenRepository)
    {
    }
}
