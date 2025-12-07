using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using QuanLyNhaHangDemo.Models;
using QuanLyNhaHangDemo.Models.ViewModels;
using QuanLyNhaHangDemo.Repository;

namespace QuanLyNhaHangDemo.Controllers
{
    public class CartController : Controller
    {
        private readonly DataContext _dataContext;
        public CartController(DataContext _context)
        {
            _dataContext = _context;
        }
        public IActionResult Index()
        {
            List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            var shippingPriceCookie = Request.Cookies["ShippingPrice"];
            decimal shippingPrice = 0;
            if (shippingPriceCookie!=null)
            {
                var shippingPriceJson = shippingPriceCookie;
                shippingPrice = JsonConvert.DeserializeObject<decimal>(shippingPriceJson);
            }
            CartItemViewModel cartVM = new()
            {
                CartItems = cartItems,
                GrandTotal = cartItems.Sum(x => x.Quantity * x.Price),
                ShippingCost = shippingPrice
            };
            return View(cartVM);

        }
        public IActionResult Checkout()
        {
            return View("~/Views/Checkout/Index.cshtml");
        }
        [HttpPost]
        public IActionResult Add(int Id, int quantity = 1)
        {
            if (quantity <= 0) quantity = 1;

            // Lấy cart từ session
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart")
                                      ?? new List<CartItemModel>();

            // Tìm sản phẩm
            var product = _dataContext.Products.FirstOrDefault(p => p.Id == Id);
            if (product == null)
            {
                return Json(new { success = false, message = "Không tìm thấy sản phẩm." });
            }

            var item = cart.FirstOrDefault(c => c.ProductId == Id);
            if (item == null)
            {
                cart.Add(new CartItemModel
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = product.Price,
                    Quantity = quantity
                });
            }
            else
            {
                item.Quantity += quantity;
            }

            // Lưu lại session
            HttpContext.Session.SetJson("Cart", cart);

            int totalItems = cart.Sum(c => c.Quantity);

            return Json(new
            {
                success = true,
                message = $"Đã thêm {quantity} x {product.Name} vào giỏ hàng.",
                totalItems = totalItems
            });
        }


        public async Task<IActionResult> Decrease(int Id)
        {
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
            CartItemModel cartItem = cart.Where(c=>c.ProductId == Id).FirstOrDefault();
            if (cartItem.Quantity > 1)
            {
                --cartItem.Quantity;
            }
            else
            {
                cart.RemoveAll(p=>p.ProductId == Id);
            }
            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.SetJson("Cart", cart);
            }
            TempData["success"] = "Decrease Item to cart Successfully";
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Increase(int Id)
        {
            ProductModel product = await _dataContext.Products.Where(p=>p.Id==Id).FirstOrDefaultAsync();
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
            CartItemModel cartItem = cart.Where(c => c.ProductId == Id).FirstOrDefault();
            if (cartItem.Quantity>=1&&product.Quantity>cartItem.Quantity)
            {
                ++cartItem.Quantity;
                TempData["success"] = "Increase Item to cart Successfully";
            }
            else
            {
                cartItem.Quantity = product.Quantity;
                TempData["success"] = "Max";
            }
            HttpContext.Session.SetJson("Cart", cart);
            TempData["success"] = "Increase Item to cart Successfully";
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Remove(int Id)
        {
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
            cart.RemoveAll(p=>p.ProductId == Id);
            if(cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.SetJson("Cart",cart);
            }
            TempData["success"] = "Remove Item to cart Successfully";
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Clear()
        {
            HttpContext.Session.Remove("Cart");
            TempData["success"] = "Clear Item to cart Successfully";
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> GetShipping(ShippingModel shipping,string tinh,string quan)
        {
            var existingShipping = await _dataContext.Shippings
                .FirstOrDefaultAsync(s => s.City == tinh && s.District == quan);
            decimal shippingPrice = 0;
            if (existingShipping!=null)
            {
                shippingPrice = existingShipping.Price;
            }
            else
            {
                shippingPrice = 350000;
            }
            var shippingPriceJson = JsonConvert.SerializeObject(shippingPrice);
            try
            {
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(30),
                    Secure = true,
                };
                Response.Cookies.Append("ShippingPrice", shippingPriceJson, cookieOptions);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error setting cookie: " + ex.Message);
            }
            return Json(new { shippingPrice });
        }
    }
}
