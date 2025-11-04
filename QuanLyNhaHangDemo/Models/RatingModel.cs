using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyNhaHangDemo.Models
{
    public class RatingModel
    {
        [Key]
        public int Id { get; set; }
        public int ProductId { get; set; }
        [Required(ErrorMessage = "Nhap ten")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Nhap email")]
        public string Email { get; set; }
        [Required(ErrorMessage ="Nhap danh gia")]
        public string Comment { get; set; }

        public string Stars { get; set; }

        [ForeignKey("ProductId")]
        public ProductModel Product { get; set; }
    }
}
