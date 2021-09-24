using System;
using System.Linq.Expressions;
using Common.DB.Mongo.DAL.Documents;
using MongoDB.Driver;

namespace Common.DB.Mongo.DAL.UpdateDocumentOperators
{
    public class SetOperator<TDocument, TField>: IUpdateOperator<TDocument> where TDocument: BaseMongoDocument
    {
        private readonly Expression<Func<TDocument, TField>> _field;
        private readonly TField _newValue;

        public SetOperator(Expression<Func<TDocument, TField>> field, TField newValue)
        {
            _field = field;
            _newValue = newValue;
        }

        public UpdateDefinition<TDocument> ToUpdateDefinition()
        {
            return Builders<TDocument>.Update.Set(_field, _newValue);
        }
    }
}