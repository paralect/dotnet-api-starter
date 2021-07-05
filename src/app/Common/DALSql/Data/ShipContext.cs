using Common.DALSql.Models;
using Microsoft.EntityFrameworkCore;

namespace Common.DALSql.Data
{
    public class ShipContext : DbContext
    {
        public ShipContext(DbContextOptions<ShipContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Token> Tokens { get; set; }
    }
}
