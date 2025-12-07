using System.ComponentModel.DataAnnotations;

namespace QuanLyNhaHangDemo.Models
{
    public class MaterialModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Tên nguyên liệu")]
        public string Name { get; set; }

        [Display(Name = "Đơn vị tính")]
        public string Unit { get; set; }          // kg, gói, chai, ...

        [Display(Name = "Số lượng tồn")]
        public decimal CurrentQuantity { get; set; }

        [Display(Name = "Ngưỡng cảnh báo")]
        public decimal ReorderLevel { get; set; }

        [Display(Name = "Trạng thái")]
        public int Status { get; set; } = 1;      // 1 = đang dùng, 0 = ẩn

        public ICollection<InventoryTransactionModel> InventoryTransactions { get; set; }
    }
}
