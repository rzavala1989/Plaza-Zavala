using HotelAppDataAccess.Models;

namespace HotelAppAPI.Interfaces;

public interface IBookingService
{
    Task<bool> IsRoomAvailable(int roomId, DateTime startDate, DateTime endDate);
    Task<BookingModel> CreateBooking(BookingModel booking);
}
