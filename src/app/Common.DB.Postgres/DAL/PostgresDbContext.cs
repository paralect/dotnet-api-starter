using Common.DB.Postgres.DAL.Interfaces;
using LinqToDB.Configuration;
using LinqToDB.Data;

namespace Common.DB.Postgres.DAL
{
    public class PostgresDbContext : DataConnection, IPostgresDbContext
    {
        public PostgresDbContext(LinqToDbConnectionOptions<PostgresDbContext> options) : base(options) { }
    }
}
