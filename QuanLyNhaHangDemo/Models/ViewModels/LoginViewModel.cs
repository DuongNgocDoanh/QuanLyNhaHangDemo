using System.ComponentModel.DataAnnotations;

namespace QuanLyNhaHangDemo.Models.ViewModels
{
    public class LoginViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Lam on nhap UserName")]
        public string Username { get; set; }
        
        [DataType(DataType.Password), Required(ErrorMessage = "Lam on nhap Password")]
        public string Password { get; set; }
        public string ReturnUrl { get; set; }
    }
}
