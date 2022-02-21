using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Common.DAL.Documents;
using MongoDB.Driver;

namespace Common.DAL.UpdateDocumentOperators
{
    public class PushOperator<TDocument, TItem>: IUpdateOperator<TDocument> where TDocument: BaseDocument
    {
        private readonly Expression<Func<TDocument, IEnumerable<TItem>>> _field;
        private readonly TItem _newItem;

        public PushOperator(Expression<Func<TDocument, IEnumerable<TItem>>> field, TItem newItem)
        {
            _field = field;
            _newItem = newItem;
        }
        
        public UpdateDefinition<TDocument> ToUpdateDefinition()
        {
            return Builders<TDocument>.Update.Push(_field, _newItem);
        }
    }
}