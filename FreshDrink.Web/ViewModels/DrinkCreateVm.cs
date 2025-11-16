using FreshDrink.Data.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FreshDrink.Web.ViewModels
{
    public class DrinkCreateVm
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên không được để trống")]
        [StringLength(200, ErrorMessage = "Tên quá dài")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Giá không hợp lệ")]
        [Range(1000, 1000000, ErrorMessage = "Giá phải từ 1.000đ đến 1.000.000đ")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn danh mục")]
        public int CategoryId { get; set; }

        public string? Description { get; set; }

        /// <summary>
        /// Ảnh hiện tại (dùng để edit)
        /// </summary>
        public string? ImageUrl { get; set; }

        /// <summary>
        /// File upload mới
        /// </summary>
        public IFormFile? ImageFile { get; set; }

        public bool IsFeatured { get; set; } = false;
        public bool IsActive { get; set; } = true;

        public List<Category> Categories { get; set; } = new();

        /// <summary>
        /// Chỉ bắt buộc file ảnh khi tạo mới
        /// </summary>
        public bool RequireImage => Id == 0;
    }
}
