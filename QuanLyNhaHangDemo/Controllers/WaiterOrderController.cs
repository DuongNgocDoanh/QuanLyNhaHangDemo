using Microsoft.AspNetCore.Mvc;

namespace QuanLyNhaHangDemo.Controllers
{
    public class WaiterOrderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
