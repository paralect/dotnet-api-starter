using Common.DB.Postgres.DAL.Documents;
using LinqToDB;

namespace Common.DB.Postgres.DAL.Interfaces
{
    public interface IPostgresDbContext: IDataContext
    {
        public ITable<T> GetTable<T>() where T : class;
    }
}
