namespace FreshDrink.Web.ViewModels
{
    public class OrderDetailVm
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = "";
        public decimal Total { get; set; }

        // Thêm cho admin xem ai là người mua
        public string? UserName { get; set; }

        public List<OrderDetailItemVm> Items { get; set; } = new();
    }

    public class OrderDetailItemVm
    {
        public string Name { get; set; } = "";
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal LineTotal { get; set; }
    }
}
