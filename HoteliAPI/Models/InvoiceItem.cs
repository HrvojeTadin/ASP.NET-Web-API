using System;

namespace HotelAPI.Models
{
    public partial class InvoiceItem
    {
        public Guid Id { get; set; }
        public int Amount { get; set; }
        public decimal TotalPrice { get; set; }
        public Guid ServiceId { get; set; }
        public Guid InvoiceId { get; set; }
        public string PIN { get; set; }
    }
}
