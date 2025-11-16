namespace FreshDrink.Web.ViewModels
{
    public class OrderSummaryVm
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = "Pending";
        public decimal Total { get; set; }
        public int ItemsCount { get; set; }
         public string? UserName { get; set; }
    }
}
