using System;

namespace HoteliAPI.Models
{
    public partial class StavkaRacuna
    {
        public Guid Id { get; set; }
        public int Kolicina { get; set; }
        public decimal UkupnaCijena { get; set; }
        public Guid UslugaId { get; set; }
        public Guid RacunId { get; set; }
        public string Sifra { get; set; }
    }
}
