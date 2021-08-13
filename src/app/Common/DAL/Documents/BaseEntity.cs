using LinqToDB.Mapping;

namespace Common.DAL.Documents
{
    public class BaseEntity
    {
        [Column(Name = "Id"), PrimaryKey, Identity]
        public long Id { get; set; }
    }
}
