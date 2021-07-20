using System.Collections.Generic;
using System.Threading.Tasks;
using Common.DALSql.Entities;

namespace Common.DALSql.Repositories
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task<T> FindOne(long id);
        Task<T> FindOneByQuery(DbQuery<T> queryParams);
        Task<IEnumerable<T>> FindByQuery(DbQuery<T> queryParams);
        Task<T> FindOneIgnoreQueryFilters(long id);
        
        void Add(T entity);
        void AddRange(IEnumerable<T> entities);
        
        void Update(T entity);
        void UpdateRange(IEnumerable<T> entities);
        
        void Delete(T entity);
        void DeleteRange(IEnumerable<T> entities);
    }
}