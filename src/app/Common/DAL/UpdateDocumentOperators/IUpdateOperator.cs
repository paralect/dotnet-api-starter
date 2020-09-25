using Common.DAL.Documents;
using MongoDB.Driver;

namespace Common.DAL.UpdateDocumentOperators
{
    public interface IUpdateOperator<TDocument> where TDocument: BaseDocument
    {
        UpdateDefinition<TDocument> ToUpdateDefinition();
    }
}