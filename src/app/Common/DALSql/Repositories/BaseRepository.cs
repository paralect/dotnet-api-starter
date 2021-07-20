using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.DALSql.Data;
using Common.DALSql.Entities;
using Microsoft.EntityFrameworkCore;

namespace Common.DALSql.Repositories
{
    public class BaseRepository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly ShipDbContext _dbContext;
        protected DbSet<T> Table { get; }
        
        protected BaseRepository(ShipDbContext context)
        {
            _dbContext = context; 
            Table = context.Set<T>();
        }
        
        protected BaseRepository(DbContextOptions<ShipDbContext> options) : this(new ShipDbContext(options))
        {
        }
        
        public async Task<T> FindOne(long id)
        {
            return await Table.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }
        
        public async Task<T> FindOneByQuery(DbQuery<T> queryParams)
        {
            return await ConstructQuery(queryParams).FirstOrDefaultAsync();
        }
        
        public async Task<IEnumerable<T>> FindByQuery(DbQuery<T> queryParams)
        {
            return await ConstructQuery(queryParams).ToListAsync();
        }

        public async Task<T> FindOneIgnoreQueryFilters(long id)
        {
            return await Table.IgnoreQueryFilters().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public void Add(T entity)
        {
            _dbContext.Attach(entity);
            Table.Add(entity);
        }

        public void AddRange(IEnumerable<T> entities)
        {
            //var entitiesToAdd = entities.ToList();
            //_dbContext.Attach(entitiesToAdd);
            Table.AddRange(entities);
        }

        public void Update(T entity)
        {
            Table.Update(entity);
        }

        public void UpdateRange(IEnumerable<T> entities)
        {
            var entitiesToUpdate = entities.ToList();
            _dbContext.Attach(entitiesToUpdate);
            Table.UpdateRange(entitiesToUpdate);
        }

        public void Delete(T entity)
        {
            _dbContext.Attach(entity);
            Table.Remove(entity);
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            var entitiesToDelete = entities.ToList();
            _dbContext.Attach(entitiesToDelete);
            Table.RemoveRange(entitiesToDelete);
        }

        private IQueryable<T> ConstructQuery(DbQuery<T> queryParams)
        {
            var query = Table.AsNoTracking();
            if (queryParams.Predicates.Any())
            {
                queryParams.Predicates.ForEach(predicate => query = query.Where(predicate));
            }

            return query;
        }
    }
}