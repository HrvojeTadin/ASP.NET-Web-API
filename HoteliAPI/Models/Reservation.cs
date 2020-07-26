using System;
using System.ComponentModel.DataAnnotations;

namespace HotelAPI.Models
{
    public partial class Reservation
    {
        public Guid Id { get; set; }
        [Required, DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}")]
        public DateTime DateFrom { get; set; }
        [Required, DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}")]
        public DateTime DateTo { get; set; }
        public int IsActive { get; set; }
        public Guid GuestId { get; set; }
        public Guid HotelManagerId { get; set; }
        public Guid RoomId { get; set; }
        [StringLength(11)]
        public string PIN { get; set; }
    }
}
