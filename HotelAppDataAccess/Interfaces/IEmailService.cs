namespace HotelAppAPI.Interfaces;

public interface IEmailService
{
    Task SendBookingConfirmationEmail(string to, string subject, string body);
}