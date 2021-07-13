using System;
using System.Threading.Tasks;
using Common.DALSql.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Common.DALSql.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ShipDbContext _context;
        private bool _isDisposed;
        
        public IUserSqlRepository Users { get; }
        public ITokenSqlRepository Tokens { get; }
        
        public UnitOfWork(ShipDbContext context)
        {
            _context = context;
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