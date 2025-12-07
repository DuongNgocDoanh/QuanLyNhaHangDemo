using System.ComponentModel.DataAnnotations;

namespace QuanLyNhaHangDemo.Models
{
    public class AdminCreateUserViewModel
    {
        [Required]
        [Display(Name = "Tên đăng nhập")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Nhập lại mật khẩu")]
        [Compare("Password", ErrorMessage = "Mật khẩu nhập lại không khớp.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Quyền")]
        public string RoleId { get; set; }
    }
}
