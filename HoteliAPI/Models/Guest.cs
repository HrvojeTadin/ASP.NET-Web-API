using System;
using System.ComponentModel.DataAnnotations;

namespace HotelAPI.Models
{
    public partial class Guest
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        [StringLength(11)]
        public string PIN { get; set; }
    }
}
