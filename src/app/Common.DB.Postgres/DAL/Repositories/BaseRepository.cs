using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.DB.Postgres.DAL.Documents;
using Common.DB.Postgres.DAL.Interfaces;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.Linq;

namespace Common.DB.Postgres.DAL.Repositories
{
    public abstract class BaseRepository<TEntity> : IRepository<TEntity>
       where TEntity : BasePostgresEntity
    {
        protected readonly IPostgresDbContext context;

        public BaseRepository(IPostgresDbContext context)
        {
            this.context = context;
        }

        public IQueryable<TEntity> GetQuery()
        {
            return context.GetTable<TEntity>();
        }

        public IUpdatable<TEntity> GetUpdateQuery(string entityId)
        {
            return context.GetTable<TEntity>()
                .Where(x => x.Id == entityId)
                .AsUpdatable();
        }

        public Task<BulkCopyRowsCopied> InsertRangeAsync(IEnumerable<TEntity> entities)
        {
            return context.GetTable<TEntity>().BulkCopyAsync(entities);
        }

        public Task<int> DeleteAsync(string entityId)
        {
            return context.GetTable<TEntity>().Where(x => x.Id == entityId).DeleteAsync();
        }

        public Task<int> InsertAsync(TEntity entity)
        {
            return context.InsertAsync(entity);
        }
    }
}
