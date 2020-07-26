using System;

namespace HoteliAPI.Models
{
    public partial class Usluga
    {
        public Guid Id { get; set; }
        public string Naziv { get; set; }
        public decimal JedinicnaCijena { get; set; }
        public string Sifra { get; set; }
    }
}
