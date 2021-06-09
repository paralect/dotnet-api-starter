using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Common.DAL.Documents;
using MongoDB.Driver;

namespace Common.DAL.UpdateDocumentOperators
{
    public class PushManyOperator<TDocument, TItem>: IUpdateOperator<TDocument> where TDocument: BaseDocument
    {
        private readonly Expression<Func<TDocument, IEnumerable<TItem>>> _field;
        private readonly IEnumerable<TItem> _newItems;

        public PushManyOperator(Expression<Func<TDocument, IEnumerable<TItem>>> field, IEnumerable<TItem> newItems)
        {
            _field = field;
            _newItems = newItems;
        }
        
        public UpdateDefinition<TDocument> ToUpdateDefinition()
        {
            return Builders<TDocument>.Update.PushEach(_field, _newItems);
        }
    }
}