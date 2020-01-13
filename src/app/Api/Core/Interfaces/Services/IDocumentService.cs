using System.Threading.Tasks;
using Api.Core.DAL;
using Api.Core.DAL.Documents;

namespace Api.Core.Interfaces.Services
{
    public interface IDocumentService<TDocument, in TFilter>
        where TDocument : BaseDocument
        where TFilter : BaseFilter
    {
        Task<TDocument> FindByIdAsync(string id);
        Task<TDocument> FindOneAsync(TFilter filter);
    }
}
