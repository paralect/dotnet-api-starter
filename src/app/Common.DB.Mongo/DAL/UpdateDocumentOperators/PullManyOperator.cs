using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Common.DB.Mongo.DAL.Documents;
using MongoDB.Driver;

namespace Common.DB.Mongo.DAL.UpdateDocumentOperators
{
    public class PullManyOperator<TDocument, TItem>: IUpdateOperator<TDocument> where TDocument: BaseMongoDocument
    {
        private readonly Expression<Func<TDocument, IEnumerable<TItem>>> _field;
        private readonly IEnumerable<TItem> _value;
        
        public PullManyOperator(Expression<Func<TDocument, IEnumerable<TItem>>> field,  IEnumerable<TItem> value)
        {
            _field = field;
            _value = value;
        }
        
        public UpdateDefinition<TDocument> ToUpdateDefinition()
        {
            return Builders<TDocument>.Update.PullAll(_field, _value);
        }
    }
}