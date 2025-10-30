using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // nhớ import để dùng ToListAsync()
using QuanLyNhaHangDemo.Models;
using QuanLyNhaHangDemo.Repository;

namespace QuanLyNhaHangDemo.Controllers
{
    public class CategoryController : Controller
    {
        private readonly DataContext _dataContext;

        public CategoryController(DataContext context)
        {
            _dataContext = context;
        }

        public async Task<IActionResult> Index(string Slug = "")
        {
            // Tìm danh mục theo slug
            var category = await _dataContext.Categories
                .FirstOrDefaultAsync(c => c.Slug == Slug);

            // Nếu không tồn tại, quay về trang chủ
            if (category == null)
                return RedirectToAction("Index", "Home");

            // Lấy danh sách sản phẩm theo danh mục
            var productsByCategory = await _dataContext.Products
                .Where(p => p.CategoryId == category.Id)
                .OrderByDescending(p => p.Id)
                .ToListAsync();

            // Gửi danh sách sản phẩm sang View
            return View(productsByCategory);
        }
    }
}
