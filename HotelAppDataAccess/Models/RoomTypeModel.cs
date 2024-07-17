using HotelAppDataAccess.Models;
using System.ComponentModel.DataAnnotations;

namespace HotelAppLibrary;

public class RoomTypeModel
{
    [Key]
    public int RoomTypeId { get; set; }
    public string TypeName { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; }
    public ICollection<RoomModel> Rooms { get; set; }
}