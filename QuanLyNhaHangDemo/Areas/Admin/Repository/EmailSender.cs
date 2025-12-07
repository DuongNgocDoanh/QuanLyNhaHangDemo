using System.Net;
using System.Net.Mail;

namespace QuanLyNhaHangDemo.Areas.Admin.Repository
{
    public class EmailSender:IEmailSender
    {
        public Task SendEmailAsync(string email,string subject, string message)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("doanhdkdk@gmail.com", "gwydtoqepdyqzlrj")
            };
            var mail = new MailMessage(from: "doanhdkdk@gmail.com", to: email, subject, message)
            {
                IsBodyHtml = true   // 🔴 để hiển thị nút đẹp
            };

            return client.SendMailAsync(mail);
        }
    }
}
