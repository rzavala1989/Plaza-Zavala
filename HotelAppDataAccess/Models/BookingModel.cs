using HotelAppLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelAppDataAccess.Models
{
    public class BookingModel
    {
        [Key]
        public int BookingId { get; set; }

        [ForeignKey("RoomTypeId")]
        public int RoomTypeId { get; set; }
        public RoomTypeModel RoomType { get; set; } // Add this navigation property if needed

        [ForeignKey("RoomId")]
        public int RoomId { get; set; }
        public RoomModel Room { get; set; } // Correct navigation property

        [ForeignKey("GuestId")]
        public int GuestId { get; set; }
        public GuestModel Guest { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public ICollection<ReviewModel> Reviews { get; set; } // Ensure this property exists
    }
}