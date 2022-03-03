﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Common.DALSql.Entities
{
    public class User : BaseEntity
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        [Required]
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsEmailVerified { get; set; }
        [Required]
        public string SignupToken { get; set; }
        public DateTime LastRequest { get; set; }
        public string ResetPasswordToken { get; set; }
        public bool OAuthGoogle { get; set; }

        public ICollection<Token> Tokens { get; set; }
    }
}