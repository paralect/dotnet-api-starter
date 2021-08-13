using Common.DAL.Documents;
using LinqToDB;

namespace Common.DAL.Interfaces
{
    public interface IDbContext: IDataContext
    {
        public ITable<T> GetTable<T>() where T : class;
    }
}
