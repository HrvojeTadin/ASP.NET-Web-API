using System;
using System.Collections.Generic;

namespace HoteliAPI.Models
{
    public partial class Racun
    {
        public Racun()
        {
            StavkaRacuna = new List<StavkaRacuna>();
        }

        public Guid Id { get; set; }
        public decimal UkupnoBezPopusta { get; set; }
        public decimal IznosPopusta { get; set; }
        public decimal Ukupno { get; set; }
        public bool Placeno { get; set; }
        public string Sifra { get; set; }

        public Guid RezervacijaId { get; set; }
        public virtual ICollection<StavkaRacuna> StavkaRacuna { get; set; }
    }
}
