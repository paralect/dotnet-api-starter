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
        private bool _isDisposed;
        private readonly bool _disposeContext;
        public DbSet<T> Table { get; }
        public ShipDbContext Context { get; }
        
        protected BaseRepository(ShipDbContext context)
        {
            Context = context;
            Table = Context.Set<T>();
        }
        
        protected BaseRepository(DbContextOptions<ShipDbContext> options) : this(new ShipDbContext(options))
        {
            _disposeContext = true;
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

        public async Task Add(T entity, bool persist = true)
        {
            await Table.AddAsync(entity);
            if (persist)
            {
                await SaveChanges();
            }
        }

        public async Task AddRange(IEnumerable<T> entities, bool persist = true)
        {
            await Table.AddRangeAsync(entities);
            if (persist)
            {
                await SaveChanges();
            }
        }

        public async Task Update(T entity, bool persist = true)
        {
            Table.Update(entity);
            if (persist)
            {
                await SaveChanges();
            }
        }

        public async Task UpdateRange(IEnumerable<T> entities, bool persist = true)
        {
            Table.UpdateRange(entities);
            if (persist)
            {
                await SaveChanges();
            }
        }

        public async Task Delete(T entity, bool persist = true)
        {
            Table.Remove(entity);
            if (persist)
            {
                await SaveChanges();
            }
        }

        public async Task DeleteRange(IEnumerable<T> entities, bool persist = true)
        {
            Table.RemoveRange(entities);
            if (persist)
            {
                await SaveChanges();
            }
        }

        public async Task ExecuteQuery(string sql, object[] sqlParametersObjects)
        {
            await Context.Database.ExecuteSqlRawAsync(sql, sqlParametersObjects);
        }

        public async Task SaveChanges()
        {
            try
            {
                await Context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                //A concurrency error occurred
                //Should log and handle intelligently
                throw;
            }
            catch (RetryLimitExceededException ex)
            {
                //DbResiliency retry limit exceeded
                //Should log and handle intelligently
                throw;
            }
            catch (DbUpdateException ex)
            {
                //Should log and handle intelligently
                throw;
            }
            catch (Exception ex)
            {
                //Should log and handle intelligently
                throw;
            }
        }
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed)
            {
                return;
            }
            if (disposing)
            {
                if (_disposeContext)
                {
                    Context.Dispose();
                }
            }
            _isDisposed = true;
        }
        
        ~BaseRepository()
        {
            Dispose(false);
        }
    }
}