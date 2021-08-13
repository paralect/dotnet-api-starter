using System.Threading.Tasks;
using Common.DAL.Documents;

namespace Common.Services.Interfaces
{
    public interface IDocumentService<TDocument>
        where TDocument : BaseEntity
    {
        Task<TDocument?> FindByIdAsync(long id);
    }
}
