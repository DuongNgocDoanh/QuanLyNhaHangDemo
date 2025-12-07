using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHangDemo.Areas.Admin.Repository;
using QuanLyNhaHangDemo.Models;
using QuanLyNhaHangDemo.Repository;

namespace QuanLyNhaHangDemo.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Reservations")]
    public class ReservationAdminController : Controller
    {
        private readonly DataContext _context;
        private readonly IEmailSender _emailSender;

        public ReservationAdminController(DataContext context, IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }

        // Danh sách đặt bàn
        [HttpGet]
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            var list = await _context.Reservations
                .Include(r => r.Table)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return View(list);
        }

        // DUYỆT
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Approve")]
        public async Task<IActionResult> Approve(int id)
        {
            var res = await _context.Reservations
                .Include(r => r.Table)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (res == null) return NotFound();

            res.Status = ReservationStatus.Approved;
            await _context.SaveChangesAsync();

            // Gửi email cho khách nếu có email
            if (!string.IsNullOrEmpty(res.CustomerEmail))
            {
                var subject = $"Xác nhận đặt bàn #{res.Id} – THÀNH CÔNG";
                var message = $@"
                    Xin chào {res.CustomerName},

                    Yêu cầu đặt bàn của bạn đã được XÁC NHẬN.

                    Bàn: {res.Table.TableName} (ID: {res.TableId})
                    Thời gian đến: {res.ReserveTime:dd/MM/yyyy HH:mm}
                    Số người: {res.PeopleCount}
                    Ghi chú: {res.Note}

                    Rất mong được đón tiếp bạn tại nhà hàng.
";

                await _emailSender.SendEmailAsync(res.CustomerEmail, subject, message);
            }

            TempData["success"] = "Đã duyệt đặt bàn.";
            return RedirectToAction("Index");
        }

        // TỪ CHỐI
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Reject")]
        public async Task<IActionResult> Reject(int id)
        {
            var res = await _context.Reservations
                .Include(r => r.Table)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (res == null) return NotFound();

            res.Status = ReservationStatus.Rejected;
            await _context.SaveChangesAsync();

            if (!string.IsNullOrEmpty(res.CustomerEmail))
            {
                var subject = $"Đặt bàn #{res.Id} – KHÔNG THÀNH CÔNG";
                var message = $@"
                    Xin chào {res.CustomerName},

                    Rất tiếc, yêu cầu đặt bàn của bạn KHÔNG được chấp nhận
                    vào thời gian: {res.ReserveTime:dd/MM/yyyy HH:mm}.

                    Bàn: {res.Table.TableName} (ID: {res.TableId})
                    Lý do có thể là: hết chỗ hoặc trùng giờ cao điểm.

                    Vui lòng thử lại với khung giờ khác hoặc liên hệ trực tiếp nhà hàng để được hỗ trợ.
                    ";

                await _emailSender.SendEmailAsync(res.CustomerEmail, subject, message);
            }

            TempData["success"] = "Đã từ chối đặt bàn.";
            return RedirectToAction("Index");
        }
    }
}
