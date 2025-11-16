using System;

namespace FreshDrink.Web.ViewModels
{
    public class CartItemVm
    {
        public int    DrinkId    { get; set; }
        public string Name       { get; set; } = "";
        public string? ImageUrl  { get; set; }
        public decimal UnitPrice { get; set; }
        public int    Quantity   { get; set; }

        public decimal LineTotal => UnitPrice * Quantity;
    }
}
