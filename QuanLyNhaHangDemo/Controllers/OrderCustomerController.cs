using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHangDemo.Models;
using QuanLyNhaHangDemo.Repository;
using System.Security.Claims;

namespace QuanLyNhaHangDemo.Controllers
{
    public class OrderCustomerController : Controller
    {
        private readonly DataContext _context;
        public OrderCustomerController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> ViewOrder(string ordercode)
        {
            if (string.IsNullOrEmpty(ordercode)) return NotFound();

            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (userEmail == null)
                return RedirectToAction("Login", "Account");

            // Chỉ cho xem đơn của chính mình
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.OrderCode == ordercode &&
                                          o.UserName == userEmail);
            if (order == null) return NotFound();

            var details = await _context.OrderDetails
                .Include(d => d.Product)
                .Where(d => d.OrderCode == ordercode)
                .ToListAsync();

            decimal total = details.Sum(d => d.Price * d.Quantity);

            ViewBag.Order = order;
            ViewBag.Total = total;
            ViewBag.Shipping = order.ShippingCost;
            ViewBag.GrandTotal = total + order.ShippingCost;

            return View(details); // Views/Order/ViewOrder.cshtml (frontend)
        }
    }
}
