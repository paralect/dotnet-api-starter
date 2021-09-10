using Common.DB.Postgres.DAL.Interfaces;
using LinqToDB.Configuration;

namespace Common.DB.Postgres.DAL
{
    public partial class PostgresDbContext : LinqToDB.Data.DataConnection, IPostgresDbContext
    {
        public PostgresDbContext()
        {
            InitDataContext();
            InitMappingSchema();
        }

        public PostgresDbContext(string configuration)
            : base(configuration)
        {
            InitDataContext();
            InitMappingSchema();
        }

        public PostgresDbContext(LinqToDbConnectionOptions options)
            : base(options)
        {
            InitDataContext();
            InitMappingSchema();
        }

        partial void InitDataContext();
        partial void InitMappingSchema();
    }
}
