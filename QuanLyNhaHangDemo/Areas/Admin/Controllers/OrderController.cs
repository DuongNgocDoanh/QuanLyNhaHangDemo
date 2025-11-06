using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHangDemo.Repository;


namespace QuanLyNhaHangDemo.Areas.Admin.Controllers
{
    [Area("Admin")]
    
    public class OrderController : Controller
    {
        private readonly DataContext _dataContext;
        public OrderController(DataContext context)
        {
            _dataContext = context;
        }
        [HttpGet]
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            return View(await _dataContext.Orders.OrderByDescending(p => p.Id).ToListAsync());
        }
        [HttpGet]
        [Route("ViewOrder")]
        public IActionResult ViewOrder(string orderCode)
        {
            var orderDetails = _dataContext.OrderDetails
                                           .Where(o => o.OrderCode == orderCode)
                                           .Include(o => o.Product)
                                           .ToList();

            ViewBag.Status = _dataContext.Orders
                                         .FirstOrDefault(o => o.OrderCode == orderCode).Status;

            ViewBag.StatusList = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "Đơn hàng mới" },
                new SelectListItem { Value = "2", Text = "Đã giao hàng" },
                new SelectListItem { Value = "3", Text = "Đang xử lý" },
                new SelectListItem { Value = "4", Text = "Đã hủy" }
            };

            return View(orderDetails);
        }

        [HttpPost]
        [Route("UpdateOrder")]
        public async Task<IActionResult> UpdateOrder(string ordercode, int status)
        {
            var order = await _dataContext.Orders.FirstOrDefaultAsync(o => o.OrderCode == ordercode);

            if (order == null)
            {
                return NotFound();
            }

            order.Status = status;

            try
            {
                await _dataContext.SaveChangesAsync();
                return Ok(new { success = true, message = "Order status updated successfully" });
            }
            catch (Exception)
            {


                return StatusCode(500, "An error occurred while updating the order status.");
            }
        }
        [HttpGet]
        [Route("Delete")]
        public async Task<IActionResult> Delete(string ordercode)
        {
            var order = await _dataContext.Orders.FirstOrDefaultAsync(o => o.OrderCode == ordercode);

            if (order == null)
            {
                return NotFound();
            }
            try
            {

                //delete order
                _dataContext.Orders.Remove(order);


                await _dataContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            catch (Exception)
            {

                return StatusCode(500, "An error occurred while deleting the order.");
            }
        }

    }
}