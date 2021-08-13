using System.Threading.Tasks;
using Common.DAL.Documents;
using Common.DAL.Interfaces;
using Common.Services.Interfaces;
using LinqToDB;

namespace Common.Services
{
    public class BaseDocumentService<TDocument> : IDocumentService<TDocument>
        where TDocument : BaseEntity
    {
        private readonly IRepository<TDocument> _repository;

        public BaseDocumentService(IRepository<TDocument> repository)
        {
            _repository = repository;
        }

        public Task<TDocument?> FindByIdAsync(long id)
        {
            return _repository.GetQuery().FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
