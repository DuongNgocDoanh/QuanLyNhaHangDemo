using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHangDemo.Repository;

namespace QuanLyNhaHangDemo.Controllers
{
    public class BrandController : Controller
    {
        private readonly DataContext _dataContext;
        public BrandController(DataContext context)
        {
            _dataContext = context;
        }
        public async Task<IActionResult> Index(string Slug = "")
        {
            // Tìm danh mục theo slug
            var brand = await _dataContext.Brands
                .FirstOrDefaultAsync(c => c.Slug == Slug);

            // Nếu không tồn tại, quay về trang chủ
            if (brand == null)
                return RedirectToAction("Index");

            // Lấy danh sách sản phẩm theo danh mục
            var productsByBrand = await _dataContext.Products
                .Where(p => p.BrandId == brand.Id)
                .OrderByDescending(p => p.Id)
                .ToListAsync();

            // Gửi danh sách sản phẩm sang View
            return View(productsByBrand);
        }
    }
}
 


  
            
     
