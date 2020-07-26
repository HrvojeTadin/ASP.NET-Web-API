using System;

namespace HotelAPI.Models
{
    public partial class HotelManager
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string PIN { get; set; }
    }
}
