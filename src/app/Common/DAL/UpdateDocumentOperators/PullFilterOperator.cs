using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Common.DAL.Documents;
using MongoDB.Driver;

namespace Common.DAL.UpdateDocumentOperators
{
    public class PullFilterOperator<TDocument, TItem>: IUpdateOperator<TDocument> where TDocument: BaseDocument
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