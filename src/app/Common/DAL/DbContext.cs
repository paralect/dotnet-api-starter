using Common.DAL.Documents;
using Common.DAL.Interfaces;
using LinqToDB;

namespace Common.DAL
{
    public class DbContext :  LinqToDB.Data.DataConnection, IDbContext
    {
        //public ITable<User> Users { get { return this.GetTable<User>(); } }
        //public ITable<Token> Tokens { get { return this.GetTable<Token>(); } }

        //public IMongoCollection<User> Users { get; }
        //public IMongoCollection<Token> Tokens { get; }
    }
}
