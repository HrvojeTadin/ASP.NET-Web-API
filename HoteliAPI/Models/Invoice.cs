using System;
using System.Collections.Generic;

namespace HotelAPI.Models
{
    public partial class Invoice
    {
        public Invoice()
        {
            InvoiceItem = new List<InvoiceItem>();
        }

        public Guid Id { get; set; }
        public decimal TotalWithoutDiscounts { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal InTotal { get; set; }
        public bool Paid { get; set; }
        public string PIN { get; set; }

        public Guid ReservationId { get; set; }
        public virtual ICollection<InvoiceItem> InvoiceItem { get; set; }
    }
}
