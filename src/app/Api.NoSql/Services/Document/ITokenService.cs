using System.Threading.Tasks;
using Common.DAL.Documents.Token;
using Common.DAL.Repositories;

namespace Api.Services.Document
{
    public interface ITokenService : IDocumentService<Token, TokenFilter>
    {
        Task<Token> FindByValueAsync(string value);
    }
}