﻿using System.ComponentModel.DataAnnotations;

namespace Api.Views.Models.View.Account;

public class ResendVerificationModel
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    public string Email { get; set; }
}