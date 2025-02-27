using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HotelAppDataAccess.Models
{
    public class GuestModel
    {
        [Key]
        public int GuestModelId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public ICollection<BookingModel>? Bookings { get; set; } // Ensure this property exists
    }
}