using System;
using Common.Models;
using LinqToDB.Mapping;

namespace Common.DB.Postgres.DAL.Documents
{
    public class BasePostgresEntity : IEntity
    {
        public BasePostgresEntity()
        {
            Id = Guid.NewGuid();
        }

        public BasePostgresEntity(Guid id)
        {
            Id = id;
        }

        [Column(Name = "Id"), PrimaryKey]
        public Guid Id { get; private set; }
    }
}
