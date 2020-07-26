using System;

namespace HotelAPI.Models
{
    public partial class Room
    {
        public Guid Id { get; set; }
        public int RoomNumber { get; set; }
        public decimal PricePerNight { get; set; }
    }
}
