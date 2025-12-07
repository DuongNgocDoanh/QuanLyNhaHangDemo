namespace QuanLyNhaHangDemo.Models
{
    public class OrderModel
    {
        public int Id { get; set; }
        public string OrderCode { get; set; }

        public decimal ShippingCost { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UserName { get; set; }
        public int Status { get; set; }

        // 🔹 Liên kết với bàn
        public int? TableId { get; set; }      // ăn tại quán thì có TableId, online thì null
        public TableModel? Table { get; set; }
    }
}
