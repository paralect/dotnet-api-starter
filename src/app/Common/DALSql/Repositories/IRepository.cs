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
        Task<T> FindOneByQueryAsNoTracking(DbQuery<T> queryParams);
        Task<IEnumerable<T>> FindByQueryAsNoTracking(DbQuery<T> queryParams);
        
        void Add(T entity);
        void AddRange(IEnumerable<T> entities);
        
        void Update(T entity);
        void UpdateRange(IEnumerable<T> entities);
        
        void Delete(T entity);
        void DeleteRange(IEnumerable<T> entities);

        //Task ExecuteQuery(string sql, object[] sqlParametersObjects);
    }
}