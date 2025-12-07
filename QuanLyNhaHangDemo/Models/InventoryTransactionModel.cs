using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyNhaHangDemo.Models
{
    public class InventoryTransactionModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int MaterialId { get; set; }
        public MaterialModel Material { get; set; }

        public DateTime DateCreated { get; set; }

        public decimal Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal TotalPrice { get; set; }

        public string Type { get; set; }  
        public string? Note { get; set; }

        public int? OrderId { get; set; }
        public OrderModel? Order { get; set; }
    }
}
