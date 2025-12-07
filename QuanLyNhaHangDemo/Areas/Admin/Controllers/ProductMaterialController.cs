using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHangDemo.Models;
using QuanLyNhaHangDemo.Repository;

namespace QuanLyNhaHangDemo.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductMaterialController : Controller
    {
        private readonly DataContext _dataContext;

        public ProductMaterialController(DataContext context)
        {
            _dataContext = context;
        }

        // 1. Xem định mức theo món
        public async Task<IActionResult> Index(int productId)
        {
            var product = await _dataContext.Products.FindAsync(productId);
            if (product == null) return NotFound();

            var recipe = await _dataContext.ProductMaterials
                .Include(pm => pm.Material)
                .Where(pm => pm.ProductId == productId)
                .ToListAsync();

            ViewBag.Product = product;
            return View(recipe);
        }

        // 2. Thêm định mức
        [HttpGet]
        public async Task<IActionResult> Create(int productId)
        {
            var product = await _dataContext.Products.FindAsync(productId);
            if (product == null) return NotFound();

            ViewBag.Product = product;
            ViewBag.Materials = new SelectList(
                await _dataContext.Materials.OrderBy(m => m.Name).ToListAsync(),
                "Id", "Name"
            );

            var model = new ProductMaterialModel
            {
                ProductId = productId
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductMaterialModel model)
        {
            var product = await _dataContext.Products.FindAsync(model.ProductId);
            if (product == null) return NotFound();

            if (model.QuantityPerProduct <= 0)
                ModelState.AddModelError("QuantityPerProduct", "Định mức phải > 0");

            if (!ModelState.IsValid)
            {
                ViewBag.Product = product;
                ViewBag.Materials = new SelectList(
                    await _dataContext.Materials.OrderBy(m => m.Name).ToListAsync(),
                    "Id", "Name", model.MaterialId
                );
                return View(model);
            }

            _dataContext.ProductMaterials.Add(model);
            await _dataContext.SaveChangesAsync();

            TempData["success"] = "Thêm định mức nguyên liệu thành công";
            return RedirectToAction(nameof(Index), new { productId = model.ProductId });
        }

        // 3. Sửa định mức
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var pm = await _dataContext.ProductMaterials
                .Include(x => x.Product)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (pm == null) return NotFound();

            ViewBag.Product = pm.Product;
            ViewBag.Materials = new SelectList(
                await _dataContext.Materials.OrderBy(m => m.Name).ToListAsync(),
                "Id", "Name", pm.MaterialId
            );

            return View(pm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductMaterialModel model)
        {
            var pm = await _dataContext.ProductMaterials.FindAsync(model.Id);
            if (pm == null) return NotFound();

            if (model.QuantityPerProduct <= 0)
                ModelState.AddModelError("QuantityPerProduct", "Định mức phải > 0");

            if (!ModelState.IsValid)
            {
                var product = await _dataContext.Products.FindAsync(model.ProductId);
                ViewBag.Product = product;
                ViewBag.Materials = new SelectList(
                    await _dataContext.Materials.OrderBy(m => m.Name).ToListAsync(),
                    "Id", "Name", model.MaterialId
                );
                return View(model);
            }

            pm.MaterialId = model.MaterialId;
            pm.QuantityPerProduct = model.QuantityPerProduct;
            pm.Note = model.Note;

            _dataContext.ProductMaterials.Update(pm);
            await _dataContext.SaveChangesAsync();

            TempData["success"] = "Cập nhật định mức thành công";
            return RedirectToAction(nameof(Index), new { productId = pm.ProductId });
        }

        // 4. Xoá định mức
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var pm = await _dataContext.ProductMaterials.FindAsync(id);
            if (pm == null) return NotFound();

            int productId = pm.ProductId;

            _dataContext.ProductMaterials.Remove(pm);
            await _dataContext.SaveChangesAsync();

            TempData["success"] = "Xóa định mức thành công";
            return RedirectToAction(nameof(Index), new { productId });
        }
    }
}
