using System;
using Common.Models;
using LinqToDB.Mapping;

namespace Common.DB.Postgres.DAL.Documents
{
    public class BasePostgresEntity : IEntity
    {
        [Column(Name = "Id"), PrimaryKey]
        public string Id { get; set; } = Guid.NewGuid().ToString();
    }
}
