using System.ComponentModel.DataAnnotations.Schema;

namespace FreshDrink.Data.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; } = default!;

        public int DrinkId { get; set; }      // ID sản phẩm gốc
        public Drink? Drink { get; set; }
        public string Name { get; set; } = "";
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal LineTotal { get; set; }
    }
}
