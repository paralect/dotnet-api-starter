using Common.DalSql.Entities;
using Common.DalSql.Filters;
using Common.DalSql.Interfaces;
using Common.Services.ServicesSql.Domain.Interfaces;

namespace Common.Services.ServicesSql.Domain;

public class TokenService : BaseEntityService<Token, TokenFilter>, ITokenService
{
    public TokenService(ITokenRepository tokenRepository) : base(tokenRepository)
    {
    }
}
