using System.Threading.Tasks;
using Common.DalSql.Entities;
using Common.DalSql.Filters;
using Common.DalSql.Repositories;

namespace Api.Services.Domain
{
    public class TokenService : ITokenService
    {
        private readonly ITokenRepository _tokenRepository;

        public TokenService(ITokenRepository tokenRepository)
        {
            _tokenRepository = tokenRepository;
        }

        public async Task<Token> FindByValueAsync(string value)
        {
            return await _tokenRepository.FindOneAsync(new TokenFilter
            {
                Value = value
            });
        }
    }
}
