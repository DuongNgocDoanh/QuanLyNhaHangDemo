namespace QuanLyNhaHangDemo.Models
{
    public enum TableStatus
    {
        Empty = 0,       // Bàn trống
        Serving = 1,     // Đang sử dụng
        Reserved = 2     // Đã đặt trước
    }

    public class TableModel
    {
        public int Id { get; set; }
        public string TableName { get; set; }
        public TableStatus Status { get; set; } = TableStatus.Empty;

        public string? CurrentOrderCode { get; set; } // order đang gắn với bàn
    }
}
