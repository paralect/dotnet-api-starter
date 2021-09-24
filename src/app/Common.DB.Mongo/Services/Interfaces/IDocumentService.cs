using System;
using System.Threading.Tasks;
using Common.DB.Mongo.DAL;
using Common.DB.Mongo.DAL.Documents;

namespace Common.DB.Mongo.Services.Interfaces
{
    public interface IDocumentService<TDocument, in TFilter>
        where TDocument : BaseMongoDocument
        where TFilter : BaseFilter
    {
        Task<TDocument> FindByIdAsync(string id);
        Task<TDocument> FindOneAsync(TFilter filter);
    }
}
