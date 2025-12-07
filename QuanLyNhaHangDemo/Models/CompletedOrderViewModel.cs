namespace QuanLyNhaHangDemo.Models
{
    public class CompletedOrderViewModel
    {
        public string OrderCode { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedDate { get; set; }
        public decimal ShippingCost { get; set; }

        // Doanh thu từ sản phẩm (không tính ship)
        public decimal OrderRevenue { get; set; }

        // Nếu muốn hiển thị luôn tổng gồm cả ship:
        public decimal TotalWithShipping => OrderRevenue + ShippingCost;
    }
}
