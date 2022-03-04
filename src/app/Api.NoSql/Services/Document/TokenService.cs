using System.Threading.Tasks;
using Common.DAL.Documents.Token;
using Common.DAL.Interfaces;
using Common.DAL.Repositories;

namespace Api.Services.Document
{
    public class TokenService : BaseDocumentService<Token, TokenFilter>, ITokenService
    {
        private readonly ITokenRepository _tokenRepository;
        private readonly IUserRepository _userRepository;

        public TokenService(ITokenRepository tokenRepository, IUserRepository userRepository)
            : base(tokenRepository)
        {
            _tokenRepository = tokenRepository;
            _userRepository = userRepository;
        }

        public async Task<Token> FindByValueAsync(string value)
        {
            return await FindOneAsync(new TokenFilter { Value = value });
        }
    }
}
