using System;
using System.ComponentModel.DataAnnotations;

namespace HoteliAPI.Models
{
    public partial class Gost
    {
        public Guid Id { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        [StringLength(11)]
        public string Oib { get; set; }
    }
}
