using HotelAppAPI.Interfaces;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;


namespace HotelAppDataAccess.Services
{
    public class EmailService : IEmailService
    {
        public async Task SendBookingConfirmationEmail(string to, string subject, string body)
        {
            var smtpClient = new SmtpClient("smtp.example.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("your-email@example.com", "your-email-password"),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("your-email@example.com"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(to);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}