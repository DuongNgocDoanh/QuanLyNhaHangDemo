using System.ComponentModel.DataAnnotations;

namespace QuanLyNhaHangDemo.Models
{
    public class ProductQuantityModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="Không được để trống số lượng")]
        public int Quantity { get; set; }
        public int ProductId { get; set; }
        public DateTime DateCreated { get; set; }

    }
}
