using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHangDemo.Repository;

namespace QuanLyNhaHangDemo.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        private readonly DataContext _dataContext;

        public DashboardController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        // 1. Thống kê tổng quan
        public IActionResult Index()
        {
            var count_products = _dataContext.Products.Count();
            var count_orders = _dataContext.Orders.Count();
            var count_categories = _dataContext.Categories.Count();
            var count_users = _dataContext.Users.Count();
            ViewBag.CountProducts = count_products;
            ViewBag.CountOrders = count_orders;
            ViewBag.CountCategories = count_categories;
            ViewBag.CountUsers = count_users;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetChartData(string type, DateTime? startDate, DateTime? endDate)
        {
            var statsQuery = _dataContext.Statisticals.AsQueryable();
            var importsQuery = _dataContext.InventoryTransactions
                .Where(t => t.Type == "IN")
                .AsQueryable();

            var today = DateTime.Today;
            DateTime? from = null;
            DateTime? to = null;

            switch (type)
            {
                case "week":
                    from = today.AddDays(-6);
                    to = today;
                    break;

                case "month":
                    from = new DateTime(today.Year, today.Month, 1);
                    to = today;
                    break;

                case "year":
                    from = new DateTime(today.Year, 1, 1);
                    to = today;
                    break;

                case "custom":
                    if (startDate.HasValue && endDate.HasValue && startDate <= endDate)
                    {
                        from = startDate.Value.Date;
                        to = endDate.Value.Date;
                    }
                    break;

                case "all":
                default:
                    break;
            }

            // Lọc theo thời gian
            if (from.HasValue)
            {
                statsQuery = statsQuery.Where(s => s.DateCreated.Date >= from.Value.Date);
                importsQuery = importsQuery.Where(t => t.DateCreated.Date >= from.Value.Date);
            }

            if (to.HasValue)
            {
                statsQuery = statsQuery.Where(s => s.DateCreated.Date <= to.Value.Date);
                importsQuery = importsQuery.Where(t => t.DateCreated.Date <= to.Value.Date);
            }

            // ===== 1. BÁN HÀNG: gom theo ngày trong Statisticals =====
            var statsPerDay = await statsQuery
                .GroupBy(s => s.DateCreated.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Sold = g.Sum(x => x.Sold),
                    Quantity = g.Sum(x => x.Quantity),
                    Revenue = g.Sum(x => x.Revenue),
                    Profit = g.Sum(x => x.Profit)
                })
                .OrderBy(x => x.Date)
                .ToListAsync();

            var salesData = statsPerDay
                .Select(x => new
                {
                    date = x.Date.ToString("yyyy-MM-dd"),
                    sold = x.Sold,
                    quantity = x.Quantity,
                    revenue = x.Revenue,
                    profit = x.Profit
                })
                .ToList();

            // ===== 2. NHẬP KHO: gom theo ngày trong InventoryTransactions (IN) =====
            var importPerDay = await importsQuery
                .GroupBy(t => t.DateCreated.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    TotalImportCost = g.Sum(x => x.TotalPrice)
                })
                .OrderBy(x => x.Date)
                .ToListAsync();

            var importData = importPerDay
                .Select(x => new
                {
                    date = x.Date.ToString("yyyy-MM-dd"),
                    importCost = x.TotalImportCost
                })
                .ToList();

            // ===== 3. Tổng cộng =====
            var totalSold = statsPerDay.Sum(s => s.Sold);
            var totalQuantity = statsPerDay.Sum(s => s.Quantity);
            var totalRevenue = statsPerDay.Sum(s => s.Revenue);
            var totalProfit = statsPerDay.Sum(s => s.Profit);
            var totalImportCost = importPerDay.Sum(i => i.TotalImportCost);

            // ===== 4. TOP SẢN PHẨM: bán nhiều & lợi nhuận cao =====
            var orderDetailsQuery = _dataContext.OrderDetails
                .Include(od => od.Product)
                .Join(
                    _dataContext.Orders,
                    od => od.OrderCode,
                    o => o.OrderCode,
                    (od, o) => new { od, o }
                )
                .Where(x => x.o.Status == 2)   // 2 = Hoàn thành
                .AsQueryable();

            if (from.HasValue)
                orderDetailsQuery = orderDetailsQuery
                    .Where(x => x.o.CreatedDate.Date >= from.Value.Date);

            if (to.HasValue)
                orderDetailsQuery = orderDetailsQuery
                    .Where(x => x.o.CreatedDate.Date <= to.Value.Date);

            var orderDetailsList = await orderDetailsQuery
                .Select(x => new
                {
                    x.od.ProductId,
                    ProductName = x.od.Product.Name,
                    x.od.Quantity,
                    x.od.Price
                })
                .ToListAsync();

            var topByQuantity = new List<object>();
            var topByProfit = new List<object>();

            if (orderDetailsList.Any())
            {
                var productMaterials = await _dataContext.ProductMaterials
                    .Include(pm => pm.Material)
                    .Where(pm => pm.Material.Status == 1)   // chỉ lấy nguyên liệu đang hoạt động
                    .ToListAsync();

                var lastImports = await _dataContext.InventoryTransactions
                    .Where(t => t.Type == "IN")
                    .GroupBy(t => t.MaterialId)
                    .Select(g => g
                        .OrderByDescending(t => t.DateCreated)
                        .ThenByDescending(t => t.Id)
                        .FirstOrDefault())
                    .ToListAsync();

                var materialCostDict = lastImports
                    .Where(t => t != null)
                    .ToDictionary(t => t.MaterialId, t => t.UnitPrice);

                var productStats = new List<(int ProductId, string ProductName, decimal QuantitySold, decimal Revenue, decimal Profit)>();

                var grouped = orderDetailsList
                    .GroupBy(x => new { x.ProductId, x.ProductName });

                foreach (var g in grouped)
                {
                    var totalQty = g.Sum(i => i.Quantity);
                    var revenue = g.Sum(i => i.Quantity * i.Price);

                    decimal costPerUnit = 0m;
                    var mats = productMaterials.Where(pm => pm.ProductId == g.Key.ProductId);

                    foreach (var pm in mats)
                    {
                        if (materialCostDict.TryGetValue(pm.MaterialId, out var unitPrice))
                        {
                            costPerUnit += pm.QuantityPerProduct * unitPrice;
                        }
                    }

                    var totalCost = totalQty * costPerUnit;
                    var profit = revenue - totalCost;

                    productStats.Add((g.Key.ProductId, g.Key.ProductName, totalQty, revenue, profit));
                }

                topByQuantity = productStats
                    .OrderByDescending(p => p.QuantitySold)
                    .Take(5)
                    .Select(p => new
                    {
                        productId = p.ProductId,
                        productName = p.ProductName,
                        quantitySold = p.QuantitySold,
                        revenue = p.Revenue,
                        profit = p.Profit
                    })
                    .Cast<object>()
                    .ToList();

                topByProfit = productStats
                    .OrderByDescending(p => p.Profit)
                    .Take(5)
                    .Select(p => new
                    {
                        productId = p.ProductId,
                        productName = p.ProductName,
                        quantitySold = p.QuantitySold,
                        revenue = p.Revenue,
                        profit = p.Profit
                    })
                    .Cast<object>()
                    .ToList();
            }

            return Json(new
            {
                sales = salesData,      // doanh thu / lợi nhuận / số lượng bán theo ngày
                imports = importData,   // nhập kho theo ngày
                totalSold,
                totalQuantity,
                totalRevenue,
                totalProfit,
                totalImportCost,
                topByQuantity,
                topByProfit
            });
        }



    }
}
