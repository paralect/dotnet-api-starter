using System.Threading.Tasks;
using Common.DalSql;
using Common.DalSql.Entities;
using Common.DalSql.Filters;
using Common.DalSql.Interfaces;
using Common.ServicesSql.Domain.Interfaces;
using Common.ServicesSql.Domain.Models;

namespace Common.ServicesSql.Domain;

public class TokenService : BaseEntityService<Token, TokenFilter>, ITokenService
{
    private readonly ITokenRepository _tokenRepository;

    public TokenService(ITokenRepository tokenRepository) : base(tokenRepository)
    {
        _tokenRepository = tokenRepository;
    }

    public async Task<UserTokenModel> FindUserTokenByValueAsync(string value)
    {
        return await _tokenRepository.FindUserTokenByValueAsync(value);
    }
}
