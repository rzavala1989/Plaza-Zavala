using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelAppDataAccess.Models
{
    public class ReviewModel
    {
        [Key]
        public int ReviewId { get; set; }
        public string Content { get; set; }
        public int Rating { get; set; }

        [ForeignKey("BookingId")]
        public int BookingId { get; set; }
        public BookingModel Booking { get; set; }
    }
}