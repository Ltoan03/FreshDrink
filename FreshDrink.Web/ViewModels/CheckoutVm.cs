using System.ComponentModel.DataAnnotations;

namespace FreshDrink.Web.ViewModels;

public class CheckoutVm
{
    [Required] public string FullName { get; set; } = string.Empty;
    [Required] public string Address  { get; set; } = string.Empty;
    [Phone]   public string? Phone    { get; set; }

    // View dùng PaymentProvider
    [Required] public string PaymentProvider { get; set; } = "COD"; // COD / Momo / VNPay...
}
