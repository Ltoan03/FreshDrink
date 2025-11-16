using System;
using System.ComponentModel.DataAnnotations;

namespace FreshDrink.Data.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Required]
        public int DrinkId { get; set; }
        public Drink? Drink { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Range(1,5)]
        public int Rating { get; set; }

        [StringLength(500)]
        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
