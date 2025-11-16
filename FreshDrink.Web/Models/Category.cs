using System.ComponentModel.DataAnnotations;

namespace FreshDrink.Data.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;
        

        public bool IsActive { get; set; } = true;

        public ICollection<Drink>? Drinks { get; set; }
    }
}
