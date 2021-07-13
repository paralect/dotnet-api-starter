using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.DALSql.Data;
using Common.DALSql.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Common.DALSql.Repositories
{
    public class BaseRepository<T> : IRepository<T> where T : BaseEntity
    {
        protected DbSet<T> Table { get; }
        
        protected BaseRepository(ShipDbContext context)
        {
            Table = context.Set<T>();
        }
        
        protected BaseRepository(DbContextOptions<ShipDbContext> options) : this(new ShipDbContext(options))
        {
        }
        
        public IAsyncEnumerable<T> GetAll()
        {
            return Table;
        }

        public IAsyncEnumerable<T> GetAllIgnoreQueryFilters()
        {
            return Table.IgnoreQueryFilters().AsAsyncEnumerable();
        }

        public async Task<T> Find(long id)
        {
            return await Table.FindAsync(id);
        }

        public async Task<T> FindAsNoTracking(long id)
        {
            return await Table.Where(x => x.Id == id).AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<T> FindIgnoreQueryFilters(long id)
        {
            return await Table.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == id);
        }

        public void Add(T entity)
        {
            Table.Add(entity);
        }

        public void AddRange(IEnumerable<T> entities)
        {
            Table.AddRange(entities);
        }

        public void Update(T entity)
        {
            Table.Update(entity);
        }

        public void UpdateRange(IEnumerable<T> entities)
        {
            Table.UpdateRange(entities);
        }

        public void Delete(T entity)
        {
            Table.Remove(entity);
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            Table.RemoveRange(entities);
        }

        // public async Task ExecuteQuery(string sql, object[] sqlParametersObjects)
        // {
        //     await Context.Database.ExecuteSqlRawAsync(sql, sqlParametersObjects);
        // }
    }
}