using HotelAppLibrary;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelAppDataAccess.Models
{
    public class RoomModel
    {
        [Key]
        public int RoomId { get; set; }
        
        public string RoomNumber { get; set; }

        [ForeignKey("RoomTypeId")]
        public int RoomTypeId { get; set; }
        
        public RoomTypeModel RoomType { get; set; }

        [ForeignKey("HotelId")]
        public int HotelId { get; set; }
        
        public HotelModel Hotel { get; set; }

        public ICollection<BookingModel> Bookings { get; set; }
    }
}