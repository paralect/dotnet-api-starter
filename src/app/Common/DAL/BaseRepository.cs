using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.DAL.Documents;
using Common.DAL.Interfaces;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.Linq;

namespace Common.DAL
{
    public abstract class BaseRepository<TEntity> : IRepository<TEntity>
        where TEntity : BaseEntity
    {
        protected readonly IDbContext context;

        public BaseRepository(IDbContext context)
        {
            this.context = context;
        }

        public IQueryable<TEntity> GetQuery()
        {
            return context.GetTable<TEntity>();
        }

        public IUpdatable<TEntity> GetUpdateQuery(long entityID)
        {
            return context.GetTable<TEntity>()
                .Where(x => x.Id == entityID)
                .AsUpdatable();
        }

        public Task<BulkCopyRowsCopied> InsertRangeAsync(IEnumerable<TEntity> entities)
        {
            return context.GetTable<TEntity>().BulkCopyAsync(entities);
        }

        public Task<int> DeleteAsync(long entityID)
        {
            return context.GetTable<TEntity>().Where(x => x.Id == entityID).DeleteAsync();
        }

        public Task<int> InsertAsync(TEntity entity)
        {
            return context.InsertAsync(entity);
        }
    }
}
