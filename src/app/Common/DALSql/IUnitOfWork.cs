using System;
using System.Threading.Tasks;
using Common.DALSql.Entities;
using Microsoft.EntityFrameworkCore;

namespace Common.DALSql
{
    public interface IUnitOfWork : IDisposable
    {
        DbSet<User> Users { get; }
        DbSet<Token> Tokens { get; }
        
        void Attach(BaseEntity entity);
        Task Perform(Func<Task> action);
        Task Perform(Action action);
    }
}