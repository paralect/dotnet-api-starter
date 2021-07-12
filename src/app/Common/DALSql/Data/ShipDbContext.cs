using Common.DALSql.Entities;
using Microsoft.EntityFrameworkCore;

namespace Common.DALSql.Data
{
    public class ShipDbContext : DbContext
    {
        public ShipDbContext(DbContextOptions<ShipDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Token> Tokens { get; set; }
    }
}
