using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHangDemo.Repository;

namespace QuanLyNhaHangDemo.Controllers
{
    [Area("Admin")]
    public class KitchenController : Controller
    {
        private readonly DataContext _dataContext;

        public KitchenController(DataContext context)
        {
            _dataContext = context;
        }

        public async Task<IActionResult> Index()
        {
            var tasks = await _dataContext.OrderDetails
                .Where(od => od.Status != 2) // Chưa hoàn thành
                .Include(od => od.Product)
                .ThenInclude(p => p.Category) // Để lấy Category.Name
                .OrderBy(od => od.Product.Category.Priority) // Sắp xếp theo độ ưu tiên
                .ThenBy(od => od.Id)
                .ToListAsync();

            return View(tasks);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int orderDetailId, int status)
        {
            var orderDetail = await _dataContext.OrderDetails.FindAsync(orderDetailId);
            if (orderDetail != null)
            {
                orderDetail.Status = status;
                await _dataContext.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}
