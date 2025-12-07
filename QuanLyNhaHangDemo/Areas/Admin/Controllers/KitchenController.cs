using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHangDemo.Models; // nếu cần
using QuanLyNhaHangDemo.Repository;
using System.Linq;
using System.Threading.Tasks;

namespace QuanLyNhaHangDemo.Controllers
{
    [Area("Admin")]
    public class KitchenController : Controller
    {
        private readonly DataContext _dataContext;

        // Quy ước Priority cho từng khu
        private const int PRIORITY_KHAIVI = 0; // Món khai vị
        private const int PRIORITY_DOUONG = 1; // Đồ uống: bia, coca...
        private const int PRIORITY_MONCHINH = 3; // Món mặn nóng / món chính

        public KitchenController(DataContext context)
        {
            _dataContext = context;
        }

        // ========== KHU KHAI VỊ ==========

        public async Task<IActionResult> KhaiVi()
        {
            return await PriorityScreen(PRIORITY_KHAIVI, "Khu bếp khai vị");
        }

        // ========== KHU MÓN CHÍNH ==========

        public async Task<IActionResult> MonChinh()
        {
            return await PriorityScreen(PRIORITY_MONCHINH, "Khu bếp món chính");
        }

        // ========== KHU ĐỒ UỐNG ==========

        public async Task<IActionResult> DoUong()
        {
            return await PriorityScreen(PRIORITY_DOUONG, "Khu đồ uống");
        }

        // ========== HÀM DÙNG CHUNG LỌC THEO Category.Priority ==========

        private async Task<IActionResult> PriorityScreen(int priority, string title)
        {
            var tasks = await _dataContext.OrderDetails
        .Include(od => od.Product)
            .ThenInclude(p => p.Category)
        .Where(od => od.Product.Category.Priority == priority
                     && od.Status != 2    // không lấy món đã hoàn thành
                     && od.Status != 3)   // 🚫 không lấy món đã hủy
        .OrderBy(od => od.Id)
        .ToListAsync();

            ViewBag.ScreenTitle = title;
            return View("Index", tasks);
        }

        // ========== CẬP NHẬT TRẠNG THÁI ==========

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int orderDetailId, int status, string? returnAction)
        {
            // Lấy OrderDetail + Product
            var orderDetail = await _dataContext.OrderDetails
                .Include(od => od.Product)
                .FirstOrDefaultAsync(od => od.Id == orderDetailId);

            if (orderDetail == null)
            {
                return NotFound();
            }

            int oldStatus = orderDetail.Status;
            orderDetail.Status = status;

            // Chỉ trừ kho khi chuyển từ trạng thái KHÔNG HOÀN THÀNH -> HOÀN THÀNH (status = 2)
            bool isJustCompleted = (oldStatus != 2 && status == 2);

            // Tìm Order tương ứng (để ghi OrderId + OrderCode)
            var order = await _dataContext.Orders
                .FirstOrDefaultAsync(o => o.OrderCode == orderDetail.OrderCode);

            if (isJustCompleted)
            {
                // Lấy toàn bộ nguyên liệu dùng cho món này
                var usages = await _dataContext.ProductMaterials
                    .Include(pm => pm.Material)
                    .Where(pm => pm.ProductId == orderDetail.ProductId)
                    .ToListAsync();

                foreach (var usage in usages)
                {
                    var material = usage.Material;

                    // Số lượng nguyên liệu dùng = số món trong order * định mức / món
                    decimal usedQuantity = orderDetail.Quantity * usage.QuantityPerProduct;

                    // Trừ tồn kho (có thể âm nếu bán quá tồn)
                    material.CurrentQuantity -= usedQuantity;

                    // Cập nhật lại tồn kho mà không tạo bản ghi xuất kho
                    _dataContext.Materials.Update(material);
                }
            }

            // Lưu thay đổi: trạng thái món + kho
            await _dataContext.SaveChangesAsync();

            // Quay lại khu bếp đúng màn hình
            if (!string.IsNullOrEmpty(returnAction))
            {
                return RedirectToAction(returnAction);
            }

            return RedirectToAction(nameof(KhaiVi)); // Quay lại khu khai vị (hoặc khu bếp khác)
        }
    }
}
