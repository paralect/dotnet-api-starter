using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Common.DB.Mongo.DAL.Documents;
using MongoDB.Driver;

namespace Common.DB.Mongo.DAL.UpdateDocumentOperators
{
    public class PullFilterOperator<TDocument, TItem>: IUpdateOperator<TDocument> where TDocument: BaseMongoDocument
    {
        private readonly Expression<Func<TDocument, IEnumerable<TItem>>> _field;
        private readonly Expression<Func<TItem, bool>> _condition;
        
        public PullFilterOperator(Expression<Func<TDocument, IEnumerable<TItem>>> field, Expression<Func<TItem, bool>> condition)
        {
            _field = field;
            _condition = condition;
        }

        public UpdateDefinition<TDocument> ToUpdateDefinition()
        {
            return Builders<TDocument>.Update.PullFilter(_field, _condition);
        }
    }
}