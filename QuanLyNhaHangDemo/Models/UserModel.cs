using System.ComponentModel.DataAnnotations;

namespace QuanLyNhaHangDemo.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="Lam on nhap UserName")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Lam on nhap Email"),EmailAddress]
        public string Email { get; set; }
        [DataType(DataType.Password),Required(ErrorMessage ="Lam on nhap Password")]
        public string Password { get; set; }
    }
}
