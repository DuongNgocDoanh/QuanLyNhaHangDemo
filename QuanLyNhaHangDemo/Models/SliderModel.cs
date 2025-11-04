using QuanLyNhaHangDemo.Repository.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyNhaHangDemo.Models
{
    public class SliderModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Yeu cau nhap ten slider")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Yeu cau nhap mo ta slider")]
        public string Description { get; set; }
        public int? Status { get; set; }
        public string ImageUrl { get; set; }
        [NotMapped]
        [FileExtension]
        public IFormFile ImageFile { get; set; }

    }
}
