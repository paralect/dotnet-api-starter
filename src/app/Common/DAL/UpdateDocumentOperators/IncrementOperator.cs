using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Common.DAL.Documents;
using MongoDB.Driver;

namespace Common.DAL.UpdateDocumentOperators
{
    public class IncrementOperator<TDocument, TItem>: IUpdateOperator<TDocument> where TDocument: BaseDocument
    {
        private static readonly HashSet<Type> _allowedTypes;
        private readonly Expression<Func<TDocument, TItem>> _field;
        private readonly TItem _incrementByValue;

        static IncrementOperator()
        {
            var types = new[]
            {
                typeof(byte), typeof(sbyte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long),
                typeof(ulong), typeof(float), typeof(double), typeof(decimal),
            };
            
            _allowedTypes =
                new HashSet<Type>(types.Concat(types.Select(t => typeof(Nullable<>).MakeGenericType(t))));
        }

        public IncrementOperator(Expression<Func<TDocument, TItem>> field, TItem incrementByValue)
        {
            if (!_allowedTypes.Contains(field.ReturnType) || !_allowedTypes.Contains(incrementByValue.GetType()))
            {
                throw new ArgumentException($"Only {string.Join(", ", _allowedTypes)} types is allowed");
            }
            
            _field = field;
            _incrementByValue = incrementByValue;
        }
        public UpdateDefinition<TDocument> ToUpdateDefinition()
        {
            return Builders<TDocument>.Update.Inc(_field, _incrementByValue);
        }
    }
}