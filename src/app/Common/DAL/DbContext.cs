using Common.DAL.Documents;
using Common.DAL.Interfaces;
using LinqToDB;
using LinqToDB.Configuration;

namespace Common.DAL
{
    public partial class DbContext : LinqToDB.Data.DataConnection, IDbContext
    {
        //public ITable<User> Users { get { return this.GetTable<User>(); } }
        //public ITable<Token> Tokens { get { return this.GetTable<Token>(); } }

        //public IMongoCollection<User> Users { get; }
        //public IMongoCollection<Token> Tokens { get; }

        public DbContext()
        {
            InitDataContext();
            InitMappingSchema();
        }

        public DbContext(string configuration)
            : base(configuration)
        {
            InitDataContext();
            InitMappingSchema();
        }

        public DbContext(LinqToDbConnectionOptions options)
            : base(options)
        {
            InitDataContext();
            InitMappingSchema();
        }

        partial void InitDataContext();
        partial void InitMappingSchema();
    }
}
