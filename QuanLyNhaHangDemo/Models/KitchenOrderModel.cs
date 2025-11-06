namespace QuanLyNhaHangDemo.Models
{
    public class KitchenOrderModel
    {
        public string OrderCode { get; set; }  // Mã đơn hàng
        public string ProductName { get; set; } // Tên món
        public int Quantity { get; set; } // Số lượng món
        public string CategoryName { get; set; } // Danh mục món ăn
        public int Status { get; set; } // 0: Chưa làm, 1: Đang nấu, 2: Hoàn thành, 3: Đã phục vụ
    }
}
