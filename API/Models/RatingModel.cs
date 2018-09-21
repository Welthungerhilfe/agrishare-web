using Agrishare.Core.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace Agrishare.API.Models
{
    public class RatingModel
    {
        [Required]
        public int ListingId { get; set; }

        [Required]
        public int Rating { get; set; }

        [Required]
        public string Comments { get; set; }
    }
}