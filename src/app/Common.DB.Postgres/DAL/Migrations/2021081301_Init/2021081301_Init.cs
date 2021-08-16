using System;
using FluentMigrator;

namespace Common.DB.Postgres.DAL.Migrations
{
    [Migration(2021081301)]
    public class Init : Migration
    {
        public override void Up()
        {
            Execute.Script(AppDomain.CurrentDomain.BaseDirectory + @"DAL\Migrations\2021081301_Init\2021081301_Init_Up.sql");
        }

        public override void Down()
        {
            Delete.Table("Users");
            Delete.Table("Tokens");
        }
    }
}
