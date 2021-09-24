using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Common.DB.Mongo.DAL.Documents;
using MongoDB.Driver;

namespace Common.DB.Mongo.DAL.UpdateDocumentOperators
{
    public class IncrementOperator<TDocument, TItem>: IUpdateOperator<TDocument> where TDocument: BaseMongoDocument
    {
        private readonly Expression<Func<TDocument, TItem>> _field;
        private readonly TItem _incrementByValue;
        
        public IncrementOperator(Expression<Func<TDocument, TItem>> field, TItem incrementByValue)
        {
            if (!IncrementOperatorValidator.IsValidFieldType(typeof(TItem)))
            {
                throw new ArgumentException("Only numeric types are allowed");
            }

            if (incrementByValue == null)
            {
                throw new ArgumentNullException(nameof(incrementByValue), "Incrementing by null is not allowed");
            }
            
            _field = field;
            _incrementByValue = incrementByValue;
        }
        public UpdateDefinition<TDocument> ToUpdateDefinition()
        {
            return Builders<TDocument>.Update.Inc(_field, _incrementByValue);
        }
    }

    public static class IncrementOperatorValidator
    {
        private static readonly HashSet<Type> _allowedTypes;
        
        static IncrementOperatorValidator()
        {
            var types = new[]
            {
                typeof(byte), typeof(sbyte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long),
                typeof(ulong), typeof(float), typeof(double), typeof(decimal),
            };

            var nullableTypes = types.Select(t => typeof(Nullable<>).MakeGenericType(t));

            _allowedTypes = new HashSet<Type>(types.Concat(nullableTypes));
        }

        public static bool IsValidFieldType(Type type)
        {
            return _allowedTypes.Contains(type);
        }
    }
}