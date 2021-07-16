using System;
using System.Threading.Tasks;
using Common.DALSql.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace Common.DALSql.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ShipDbContext _context;
        private readonly ILogger<UnitOfWork> _logger;
        private bool _isDisposed;

        public IUserSqlRepository Users { get; }
        public ITokenSqlRepository Tokens { get; }
        
        public UnitOfWork(ShipDbContext context, ILogger<UnitOfWork> logger)
        {
            _context = context;
            _logger = logger;
            Users = new UserSqlRepository(_context);
            Tokens = new TokenSqlRepository(_context);
        }
        
        public async Task Complete()
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