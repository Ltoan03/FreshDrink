namespace FreshDrink.Web.ViewModels
{
    public class DrinkDetailVm
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? CategoryName { get; set; }
        public string? ImageUrl { get; set; }
        public string? Description { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsActive { get; set; }
    }
}
