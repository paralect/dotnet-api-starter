using System;
using System.Threading.Tasks;

namespace Common.DALSql.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IUserSqlRepository Users { get; }
        ITokenSqlRepository Tokens { get; }
        Task Complete();
    }
}