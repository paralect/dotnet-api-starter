using System;
using Common.DalSql.Entities;
using Microsoft.EntityFrameworkCore;

namespace Common.DalSql;

public class ShipDbContext : DbContext
{
    public ShipDbContext(DbContextOptions<ShipDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Token> Tokens { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.LogTo(Console.WriteLine);
}