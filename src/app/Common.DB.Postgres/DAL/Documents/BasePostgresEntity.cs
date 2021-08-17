using System;
using Common.Models;
using LinqToDB.Mapping;

namespace Common.DB.Postgres.DAL.Documents
{
    public class BasePostgresEntity : IEntity
    {
        public BasePostgresEntity()
        {
            Id = Guid.NewGuid().ToString();
        }

        public BasePostgresEntity(Guid id)
        {
            Id = id.ToString();
        }

        [Column(Name = "Id"), PrimaryKey]
        public string Id { get; private set; }
    }
}
