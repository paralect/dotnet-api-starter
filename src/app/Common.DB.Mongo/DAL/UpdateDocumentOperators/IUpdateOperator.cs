using Common.DB.Mongo.DAL.Documents;
using MongoDB.Driver;

namespace Common.DB.Mongo.DAL.UpdateDocumentOperators
{
    public interface IUpdateOperator<TDocument> where TDocument: BaseMongoDocument
    {
        UpdateDefinition<TDocument> ToUpdateDefinition();
    }
}