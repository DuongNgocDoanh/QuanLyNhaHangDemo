using System.ComponentModel.DataAnnotations;

namespace QuanLyNhaHangDemo.Models
{
    public class ProductMaterialModel
    {
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }
        public ProductModel Product { get; set; }

        [Required]
        public int MaterialId { get; set; }
        public MaterialModel Material { get; set; }

        [Required]
        [Display(Name = "Định mức / 1 phần món")]
        public decimal QuantityPerProduct { get; set; }

        public string? Note { get; set; }
    }
}
