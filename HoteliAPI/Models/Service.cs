using System;

namespace HotelAPI.Models
{
    public partial class Service
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal PricePerItem { get; set; }
        public string PIN { get; set; }
    }
}
