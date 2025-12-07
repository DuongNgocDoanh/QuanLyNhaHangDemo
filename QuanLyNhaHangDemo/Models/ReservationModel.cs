using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyNhaHangDemo.Models
{
    public enum ReservationStatus
    {
        Pending = 0,   // Chờ duyệt
        Approved = 1,  // Đã duyệt
        Rejected = 2,  // Từ chối
        Cancelled = 3,  // Khách hủy
        Completed = 4   // Đã hoàn thành

    }

    public class ReservationModel
    {
        public int Id { get; set; }

        [Required]
        public int TableId { get; set; }
        public TableModel Table { get; set; }

        [Required, Display(Name = "Tên khách")]
        public string CustomerName { get; set; }

        [Required, Display(Name = "Số điện thoại")]
        public string Phone { get; set; }

        [Display(Name = "Số người")]
        public int PeopleCount { get; set; }

        [Required, Display(Name = "Thời gian đến (dự kiến)")]
        public DateTime ReserveTime { get; set; }

        [Display(Name = "Ghi chú")]
        public string? Note { get; set; }

        public ReservationStatus Status { get; set; } = ReservationStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string? CustomerEmail { get; set; }
    }
}
