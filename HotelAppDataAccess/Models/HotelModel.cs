using HotelAppDataAccess.Models;
using System.ComponentModel.DataAnnotations;

namespace HotelAppLibrary;

public class HotelModel
{
    [Key]
    public int HotelId { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public ICollection<RoomModel> Rooms { get; set; }
}