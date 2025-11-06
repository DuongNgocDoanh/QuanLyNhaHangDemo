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
            var brand = await _dataContext.Brands
                .FirstOrDefaultAsync(c => c.Slug == Slug && c.Status == 1);

            // Nếu không tồn tại hoặc bị ẩn, quay về trang chủ
            if (brand == null)
                return RedirectToAction("Index", "Home");

            // Lấy danh sách sản phẩm theo brand
            var productsByBrand = await _dataContext.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Where(p => p.BrandId == brand.Id
                            && p.Category.Status == 1)
                .OrderByDescending(p => p.Id)
                .ToListAsync();

            ViewBag.BrandName = brand.Name;

            // Gửi danh sách sản phẩm sang View
            return View(productsByBrand);
        }

    }
}
 


  
            
     
