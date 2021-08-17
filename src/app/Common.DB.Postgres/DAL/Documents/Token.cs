using System;
using Common.Enums;
using Common.Models;
using LinqToDB.Mapping;

namespace Common.DB.Postgres.DAL.Documents
{
    [Table(Name = "Tokens")]
    public class Token : BasePostgresEntity, IToken
    {
        [Column, NotNull] public TokenTypeEnum Type { get; set; }
        [Column, Nullable] public string? Value { get; set; }
        [Column, NotNull] public DateTime ExpireAt { get; set; }
        [Column, NotNull] public string UserId { get; set; }
    }
}
