using System;
using Common.Enums;
using LinqToDB.Mapping;

namespace Common.DAL.Documents
{
    [Table(Name = "Tokens")]
    public class Token : BaseEntity
    {
        [Column, NotNull] public TokenTypeEnum Type { get; set; }
        [Column, Nullable] public string? Value { get; set; }
        [Column, NotNull] public DateTime ExpireAt { get; set; }
        [Column, NotNull] public long UserId { get; set; }
    }
}
