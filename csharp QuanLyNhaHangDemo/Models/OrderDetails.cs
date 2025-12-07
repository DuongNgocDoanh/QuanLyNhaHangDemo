using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyNhaHangDemo.Models
{
    public class OrderDetails
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string OrderCode { get; set; }

        // add OrderId FK (optional)
        public int? OrderId { get; set; }
        [ForeignKey("OrderId")]
        public OrderModel Order { get; set; }

        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public int Status { get; set; }
        public decimal Price { get; set; }
        [ForeignKey("ProductId")]
        public ProductModel Product { get; set; }
    }
}