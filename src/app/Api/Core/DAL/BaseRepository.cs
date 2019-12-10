﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Api.Core.DbViews;
using Api.Core.Interfaces.DAL;
using Api.Core.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Api.Core.DAL
{
    public abstract class BaseRepository<TModel> : IRepository<TModel> 
        where TModel : BaseView
    {
        protected readonly DbContext Context;
        protected readonly IMongoCollection<TModel> Collection;

        protected BaseRepository(IOptions<DbSettings> settings, Func<DbContext, IMongoCollection<TModel>> collectionProvider)
        {
            Context = new DbContext(settings);
            Collection = collectionProvider(Context);
        }

        public async Task Insert(TModel model)
        {
            try
            {
                await Collection.InsertOneAsync(model);
            }
            catch (Exception ex)
            {
                // TODO: log
                throw ex;
            }
        }

        public async Task InsertMany(IEnumerable<TModel> models)
        {
            try
            {
                await Collection.InsertManyAsync(models);
            }
            catch (Exception ex)
            {
                // TODO: log
                throw ex;
            }
        }

        public TModel FindOne(Func<TModel, bool> predicate)
        {
            try
            {
                return Collection.AsQueryable().Where(predicate).SingleOrDefault();
            }
            catch (Exception ex)
            {
                // TODO: log
                throw ex;
            }
        }

        public TModel FindById(string id)
        {
            return FindOne(x => x.Id == id);
        }

        public async Task<bool> Update(string id, Expression<Func<TModel, TModel>> updateExpression)
        {
            var memberInitExpression = updateExpression.Body as MemberInitExpression;
            if (memberInitExpression == null)
                throw new ArgumentException("The update expression must be of type MemberInitExpression.", "updateExpression");

            UpdateDefinition<TModel> update = null;

            foreach (MemberBinding binding in memberInitExpression.Bindings)
            {
                string propertyName = binding.Member.Name;

                var memberAssignment = binding as MemberAssignment;
                if (memberAssignment == null)
                    throw new ArgumentException("The update expression MemberBinding must be of type MemberAssignment.", "updateExpression");

                Expression memberExpression = memberAssignment.Expression;

                object value;

                if (memberExpression.NodeType == ExpressionType.Constant)
                {
                    var constantExpression = memberExpression as ConstantExpression;
                    if (constantExpression == null)
                        throw new ArgumentException("The MemberAssignment expression is not a ConstantExpression.", "updateExpression");

                    value = constantExpression.Value;
                }
                else
                {
                    LambdaExpression lambda = Expression.Lambda(memberExpression, null);
                    value = lambda.Compile().DynamicInvoke();
                }

                if (update == null)
                    update = Builders<TModel>.Update.Set(propertyName, value);
                else
                    update = update.Set(propertyName, value);
            }

            var filter = Builders<TModel>.Filter.Eq(x => x.Id, id);

            try
            {
                UpdateResult actionResult = await Collection.UpdateOneAsync(filter, update);
                return actionResult.IsAcknowledged && actionResult.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                // TODO: log
                throw ex;
            }
        }

        public async Task DeleteMany(Expression<Func<TModel, bool>> deleteExpression)
        {
            try
            {
                await Collection.DeleteManyAsync(deleteExpression);
            }
            catch (Exception ex)
            {
                // TODO: log
                throw ex;
            }
        }
    }
}
