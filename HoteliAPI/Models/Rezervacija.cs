using System;
using System.ComponentModel.DataAnnotations;

namespace HoteliAPI.Models
{
    public partial class Rezervacija
    {
        public Guid Id { get; set; }
        [Required, DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}")]
        public DateTime DatumOd { get; set; }
        [Required, DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}")]
        public DateTime DatumDo { get; set; }
        public int Aktivna { get; set; }
        public Guid GostId { get; set; }
        public Guid VoditeljId { get; set; }
        public Guid SobaId { get; set; }
        [StringLength(11)]
        public string Sifra { get; set; }
    }
}
