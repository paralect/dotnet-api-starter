using System.Threading.Tasks;
using Common.DAL;
using Common.DAL.Documents;
using Common.DAL.Interfaces;
using Common.Services.Interfaces;

namespace Common.Services
{
    public class BaseDocumentService<TDocument, TFilter> : IDocumentService<TDocument, TFilter>
        where TDocument : BaseDocument
        where TFilter : BaseFilter, new()
    {
        private readonly IRepository<TDocument, TFilter> _repository;

        public BaseDocumentService(IRepository<TDocument, TFilter> repository)
        {
            _repository = repository;
        }

        public async Task<TDocument> FindByIdAsync(string id)
        {
            return await FindOneAsync(new TFilter {Id = id});
        }

        public async Task<TDocument> FindOneAsync(TFilter filter)
        {
            return await _repository.FindOneAsync(filter);
        }
    }
}
