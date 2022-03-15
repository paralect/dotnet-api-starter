using System.Threading.Tasks;
using Common.Dal.Documents.Token;
using Common.Dal.Repositories;

namespace Common.Services.Domain.Interfaces;

public interface ITokenService : IDocumentService<Token, TokenFilter>
{
    Task<Token> FindByValueAsync(string value);
}
