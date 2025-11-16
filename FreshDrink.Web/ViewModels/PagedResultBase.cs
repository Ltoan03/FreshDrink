namespace FreshDrink.Web.ViewModels;

public class PagedResultBase
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int TotalRecords { get; set; }
    public int TotalPages => PageSize == 0 ? 0 : (int)Math.Ceiling(TotalRecords / (double)PageSize);

    // Dùng cho _Pagination.cshtml để giữ query khi chuyển trang
    public string? QueryString { get; set; }
}
