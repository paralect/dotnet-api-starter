using System.Collections.Generic;
using System.Threading.Tasks;
using Common.DALSql.Entities;

namespace Common.DALSql.Repositories
{
    public interface IRepository<T> where T : BaseEntity
    {
        IAsyncEnumerable<T> GetAll();
        IAsyncEnumerable<T> GetAllIgnoreQueryFilters();
        
        Task<T> Find(long id);
        Task<T> FindAsNoTracking(long id);
        Task<T> FindIgnoreQueryFilters(long id);
        
        Task Add(T entity, bool persist = true);
        Task AddRange(IEnumerable<T> entities, bool persist = true);
        
        Task Update(T entity, bool persist = true);
        Task UpdateRange(IEnumerable<T> entities, bool persist = true);
        
        Task Delete(T entity, bool persist = true);
        Task DeleteRange(IEnumerable<T> entities, bool persist = true);

        Task ExecuteQuery(string sql, object[] sqlParametersObjects);
        Task SaveChanges();
    }
}