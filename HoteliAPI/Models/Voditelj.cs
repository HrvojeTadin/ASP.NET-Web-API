using System;

namespace HoteliAPI.Models
{
    public partial class Voditelj
    {
        public Guid Id { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string Sifra { get; set; }
    }
}
