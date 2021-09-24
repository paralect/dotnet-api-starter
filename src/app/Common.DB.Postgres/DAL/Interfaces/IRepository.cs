using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.DB.Postgres.DAL.Documents;
using LinqToDB.Data;
using LinqToDB.Linq;

namespace Common.DB.Postgres.DAL.Interfaces
{
    public interface IRepository<TEntity>
        where TEntity : BasePostgresEntity
    {
        IQueryable<TEntity> GetQuery();
        IUpdatable<TEntity> GetUpdateQuery(string entityId);
        Task<BulkCopyRowsCopied> InsertRangeAsync(IEnumerable<TEntity> entities);
        Task<int> DeleteAsync(string entityId);
        Task<int> InsertAsync(TEntity entity);
    }
}
