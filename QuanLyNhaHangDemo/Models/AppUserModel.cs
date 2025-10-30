using Microsoft.AspNetCore.Identity;

namespace QuanLyNhaHangDemo.Models
{
    public class AppUserModel :IdentityUser
    {
        public string Occupation { get; set; }
    }
}
