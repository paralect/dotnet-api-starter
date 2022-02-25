﻿using System.ComponentModel.DataAnnotations;

namespace Api.Models.User
{
    public class UpdateCurrentModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [MinLength(2, ErrorMessage = "Your first name must be longer than 1 letter.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [MinLength(2, ErrorMessage = "Your last name must be longer than 1 letter.")]
        public string LastName { get; set; }
    }
}
