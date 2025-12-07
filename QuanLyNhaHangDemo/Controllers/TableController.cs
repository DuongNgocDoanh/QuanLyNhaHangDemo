using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHangDemo.Models;
using QuanLyNhaHangDemo.Repository;
using Microsoft.AspNetCore.Http;

namespace QuanLyNhaHangDemo.Controllers
{
    public class TableController : Controller
    {
        private readonly DataContext _context;

        public TableController(DataContext context)
        {
            _context = context;
        }

        // /Table/Index – hiển thị sơ đồ bàn
        public async Task<IActionResult> Index()
        {
            var tables = await _context.tableModels
                                       .OrderBy(t => t.Id)
                                       .ToListAsync();
            return View(tables);
        }

        public async Task<IActionResult> Choose(int tableId)
        {
            var table = await _context.tableModels.FindAsync(tableId);
            if (table == null) return NotFound();

            HttpContext.Session.SetInt32("CurrentTableId", tableId);

            if (table.Status == TableStatus.Empty)
            {
                table.Status = TableStatus.Serving;
                _context.tableModels.Update(table);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public async Task<IActionResult> ViewOrderAtTable(int tableId)
        {
            var table = await _context.tableModels.FindAsync(tableId);
            if (table == null) return NotFound();

            // Ưu tiên CurrentOrderCode nếu đã set
            string? orderCode = table.CurrentOrderCode;

            if (string.IsNullOrEmpty(orderCode))
            {
                // Nếu chưa có, tìm đơn đang mở của bàn này
                var order = await _context.Orders
                    .Where(o => o.TableId == tableId &&
                                o.Status != 2 &&   // != Hoàn thành
                                o.Status != 5)     // != Hủy (nếu có)
                    .OrderByDescending(o => o.CreatedDate)
                    .FirstOrDefaultAsync();

                if (order == null)
                {
                    TempData["error"] = "Bàn này chưa có đơn hàng nào đang mở.";
                    return RedirectToAction("Index");
                }

                orderCode = order.OrderCode;
            }

            // 👉 Nhảy thẳng sang Admin/Order/ViewOrder (ở đó có nút In hóa đơn)
            return RedirectToAction(
                actionName: "ViewOrder",
                controllerName: "Order",
                routeValues: new { area = "Admin", ordercode = orderCode }
            );

        }
    }
}
