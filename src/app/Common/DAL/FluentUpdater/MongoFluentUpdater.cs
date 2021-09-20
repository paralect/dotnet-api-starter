using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Common.DAL.Documents;
using Common.DAL.UpdateDocumentOperators;
using Common.Utils;
using MongoDB.Driver;

namespace Common.DAL.FluentUpdater
{
    public class MongoFluentUpdater<TDocument> : IUpdateOperator<TDocument> where TDocument : BaseDocument
    {
        private readonly List<UpdateDefinition<TDocument>> _updates;

        public MongoFluentUpdater()
        {
            _updates = new();
        }

        public MongoFluentUpdater<TDocument> Set<TField>(Expression<Func<TDocument, TField>> field, TField value)
        {
            _updates.Add(Builders<TDocument>.Update.Set(field, value));
            return this;
        }

        public MongoFluentUpdater<TDocument> Push<TItem>(Expression<Func<TDocument, IEnumerable<TItem>>> field, TItem item)
        {
            _updates.Add(Builders<TDocument>.Update.Push(field, item));
            return this;
        }

        public MongoFluentUpdater<TDocument> PushMany<TItem>(Expression<Func<TDocument, IEnumerable<TItem>>> field, IEnumerable<TItem> values)
        {
            _updates.Add(Builders<TDocument>.Update.PushEach(field, values));
            return this;
        }

        public MongoFluentUpdater<TDocument> Pull<TItem>(Expression<Func<TDocument, IEnumerable<TItem>>> field, TItem item)
        {
            _updates.Add(Builders<TDocument>.Update.Pull(field, item));
            return this;
        }

        public MongoFluentUpdater<TDocument> PullMany<TItem>(Expression<Func<TDocument, IEnumerable<TItem>>> field, IEnumerable<TItem> value)
        {
            _updates.Add(Builders<TDocument>.Update.PullAll(field, value));
            return this;
        }

        public MongoFluentUpdater<TDocument> PullWhere<TItem>(Expression<Func<TDocument, IEnumerable<TItem>>> field, Expression<Func<TItem, bool>> condition)
        {
            _updates.Add(Builders<TDocument>.Update.PullFilter(field, condition));
            return this;
        }

        public MongoFluentUpdater<TDocument> Increment<TItem>(Expression<Func<TDocument, TItem>> field, TItem incrementByValue)
        {
            _updates.Add(Builders<TDocument>.Update.Inc(field, incrementByValue));
            return this;
        }

        public UpdateDefinition<TDocument> ToUpdateDefinition()
        {
            return Builders<TDocument>.Update.Combine(_updates);
        }
    }
}