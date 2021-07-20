using System;
using System.Threading.Tasks;
using Common.DALSql.Data;
using Common.DALSql.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace Common.DALSql
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ShipDbContext _context;
        private readonly ILogger<UnitOfWork> _logger;
        private bool _isDisposed;

        public DbSet<User> Users { get; }
        public DbSet<Token> Tokens { get; }
        
        public UnitOfWork(ShipDbContext context, ILogger<UnitOfWork> logger)
        {
            _context = context;
            Users = _context.Users;
            Tokens = _context.Tokens;
            
            _logger = logger;
        }

        public void Attach(BaseEntity entity)
        {
            _context.Attach(entity);
        }

        public async Task Perform(Func<Task> action)
        {
            await action();
            await Complete();
        }
        
        public async Task Perform(Action action)
        {
            action();
            await Complete();
        }
        
        private async Task Complete()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
            catch (RetryLimitExceededException ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
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
            if (!_isDisposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            
            _isDisposed = true;
        }
        
        ~UnitOfWork()
        {
            Dispose(false);
        }
    }
}