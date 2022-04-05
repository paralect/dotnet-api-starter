using Common.Dal.Documents.Token;
using Common.Dal.Interfaces;
using Common.Dal.Repositories;
using Common.Services.NoSql.Domain.Interfaces;

namespace Common.Services.NoSql.Domain;

public class TokenService : BaseDocumentService<Token, TokenFilter>, ITokenService
{
    public TokenService(ITokenRepository tokenRepository)
        : base(tokenRepository)
    {
    }

    public async Task<Token> FindByValueAsync(string value)
    {
        return await FindOneAsync(new TokenFilter { Value = value });
    }
}
