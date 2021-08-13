using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.DAL.Documents;
using LinqToDB.Data;
using LinqToDB.Linq;

namespace Common.DAL.Interfaces
{
    public interface IRepository<TEntity>
        where TEntity : BaseEntity
    {
        IQueryable<TEntity> GetQuery();
        IUpdatable<TEntity> GetUpdateQuery(long entityID);
        Task<BulkCopyRowsCopied> InsertRangeAsync(IEnumerable<TEntity> entities);
        Task<int> DeleteAsync(long entityID);
        Task<int> InsertAsync(TEntity entity);
    }
}
