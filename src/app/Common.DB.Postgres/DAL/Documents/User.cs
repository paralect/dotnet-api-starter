﻿using System;
using Common.Enums;
using Common.Models;
using LinqToDB.Mapping;

namespace Common.DB.Postgres.DAL.Documents
{
    [Table(Name = "Users")]
    public class User : BasePostgresEntity, IUser
    {
        [Column, Nullable] public string FirstName { get; set; }
        [Column, Nullable] public string LastName { get; set; }
        [Column, Nullable] public string PasswordHash { get; set; }
        [Column, Nullable] public string Email { get; set; }
        [Column, NotNull] public bool IsEmailVerified { get; set; }
        [Column, NotNull] public UserRoleEnum Role { get; set; }
        [Column, Nullable] public string SignupToken { get; set; }
        [Column, NotNull] public DateTime LastRequest { get; set; }
        [Column, NotNull] public bool OAuthGoogle { get; set; }
        [Column, Nullable] public string ResetPasswordToken { get; set; }
    }
}