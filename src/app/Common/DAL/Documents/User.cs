﻿using System;
using LinqToDB.Mapping;

namespace Common.DAL.Documents
{
    [Table(Name = "Users")]
    public class User : BaseEntity
    {
        //public User()
        //{
        //    OAuth = new OAuthSettings();
        //}
        [Column, NotNull] public string FirstName { get; set; }
        [Column, Nullable] public string? LastName { get; set; }
        [Column, Nullable] public string? PasswordHash { get; set; }
        [Column, Nullable] public string? Email { get; set; }
        [Column, NotNull] public bool IsEmailVerified { get; set; }
        [Column, Nullable] public string? SignupToken { get; set; }
        [Column, NotNull] public DateTime LastRequest { get; set; }
        //public OAuthSettings OAuth { get; set; }
        [Column, NotNull] public bool OAuthGoogle { get; set; }
        [Column, Nullable] public string? ResetPasswordToken { get; set; }

        //public class OAuthSettings
        //{
        //    public bool Google { get; set; }
        //}
    }
}