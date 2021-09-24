using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Common.DB.Mongo.DAL.Documents;
using MongoDB.Driver;

namespace Common.DB.Mongo.DAL.UpdateDocumentOperators
{
    public class PullOperator<TDocument, TItem>: IUpdateOperator<TDocument> where TDocument: BaseMongoDocument
    {
        private readonly Expression<Func<TDocument, IEnumerable<TItem>>> _field;
        private readonly TItem _value;
        
        public PullOperator(Expression<Func<TDocument, IEnumerable<TItem>>> field,  TItem value)
        {
            _field = field;
            _value = value;
        }
        
        public UpdateDefinition<TDocument> ToUpdateDefinition()
        {
            return Builders<TDocument>.Update.Pull(_field, _value);
        }
    }
}