using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Api.Core.DAL.Views;
using Api.Core.Interfaces.DAL;
using Api.Core.Utils;
using MongoDB.Driver;

namespace Api.Core.DAL
{
    public abstract class BaseRepository<TModel, TFilter> : IRepository<TModel, TFilter> 
        where TModel : BaseView
        where TFilter : BaseFilter, new()
    {
        protected readonly IDbContext DbContext;
        protected readonly IIdGenerator IdGenerator;
        protected readonly IMongoCollection<TModel> Collection;

        protected BaseRepository(IDbContext dbContext, IIdGenerator idGenerator, Func<IDbContext, IMongoCollection<TModel>> collectionProvider)
        {
            DbContext = dbContext;
            IdGenerator = idGenerator;
            Collection = collectionProvider(DbContext);
        }

        public async Task InsertAsync(TModel model)
        {
            if (model.Id.HasNoValue())
            {
                model.Id = IdGenerator.Generate();
            }

            await Collection.InsertOneAsync(model);
        }

        public async Task InsertManyAsync(IEnumerable<TModel> models)
        {
            var modelsToInsert = models.Select(m =>
            {
                if (m.Id.HasNoValue())
                {
                    m.Id = IdGenerator.Generate();
                }

                return m;
            }).ToList();

            await Collection.InsertManyAsync(modelsToInsert);
        }

        public async Task<TModel> FindOneAsync(TFilter filter)
        {
            var result = await Collection.FindAsync(BuildFilterQuery(filter));
            return result.SingleOrDefault();
        }

        public async Task<TModel> FindByIdAsync(string id)
        {
            var filter = new TFilter { Id = id };
            return await FindOneAsync(filter);
        }

        public async Task<bool> UpdateAsync(string id, Expression<Func<TModel, TModel>> updateExpression)
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

        public async Task DeleteManyAsync(Expression<Func<TModel, bool>> deleteExpression)
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

        protected virtual IEnumerable<FilterDefinition<TModel>> GetFilterQueries(TFilter filter)
        {
            return new List<FilterDefinition<TModel>>();
        }

        private FilterDefinition<TModel> BuildFilterQuery(TFilter filter)
        {
            var filterQueries = GetFilterQueries(filter);
            return Builders<TModel>.Filter.And(filterQueries);
        }
    }
}
