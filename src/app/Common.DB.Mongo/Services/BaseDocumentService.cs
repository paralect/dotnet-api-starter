using System.Threading.Tasks;
using Common.DB.Mongo.DAL;
using Common.DB.Mongo.DAL.Documents;
using Common.DB.Mongo.DAL.Interfaces;
using Common.DB.Mongo.Services.Interfaces;

namespace Common.DB.Mongo.Services
{
    public class BaseDocumentService<TDocument, TFilter> : IDocumentService<TDocument, TFilter>
        where TDocument : BaseMongoDocument
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
