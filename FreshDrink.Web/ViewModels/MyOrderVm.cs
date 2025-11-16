namespace FreshDrink.Web.ViewModels
{
    public class MyOrderVm
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public int ItemsCount { get; set; }
    }
}
