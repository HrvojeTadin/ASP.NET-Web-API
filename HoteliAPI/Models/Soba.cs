using System;

namespace HoteliAPI.Models
{
    public partial class Soba
    {
        public Guid Id { get; set; }
        public int BrojSobe { get; set; }
        public decimal CijenaPoNocenju { get; set; }
    }
}
