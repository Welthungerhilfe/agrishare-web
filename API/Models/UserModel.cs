using Agrishare.Core.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace Agrishare.API.Models
{
    public class UserModel
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Telephone { get; set; }

        public string PIN { get; set; }

        public string EmailAddress { get; set; }
        public Gender GenderId { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Language? LanguageId { get; set; }

    }
}