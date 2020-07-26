using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HoteliAPI.Models;
using System.Text;

namespace HoteliAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RezervacijaController : ControllerBase
    {
        private readonly HoteliContext _context;

        public RezervacijaController(HoteliContext context)
        {
            _context = context;
        }

        // GET: api/Rezervacija
        [HttpGet]
        public ActionResult<IEnumerable<Rezervacija>> GetRezervacije()
        {
            var rezervacije = _context.Rezervacija.ToList();

            if (rezervacije.Count > 0)
            {
                return rezervacije;
            }
            else
                return NoContent();
        }

        // GET: api/Rezervacija/4FF2BA72-2A10-4EB6-84E9-59E572E7F97D
        [HttpGet("{id}")]
        public ActionResult<Rezervacija> GetRezervacija(Guid id)
        {
            var rezervacija = _context.Rezervacija.FirstOrDefault(x => x.Id == id);

            if (rezervacija == null)
            {
                return BadRequest("Rezervacija po Id-u ne postoji u bazi.");
            }

            return rezervacija;
        }

        // PUT: api/Rezervacija
        [HttpPut]
        public IActionResult PutRezervacija(Rezervacija rezervacijaRequest)
        {
            var rezervacijaDB = _context.Rezervacija.FirstOrDefault(x => x.Id == rezervacijaRequest.Id);

            if (rezervacijaDB == null)
            {
                return BadRequest("Rezervacija po zadanoj Šifri ne postoji u bazi.");
            }

            var gostDB = _context.Gost.FirstOrDefault(x => x.Id == rezervacijaRequest.GostId);
            if (gostDB == null)
            {
                return BadRequest("Nemoguće ažurirati rezervaciju -> Gost po zadanom Id-u ne postoji u bazi.");
            }

            var voditeljDB = _context.Voditelj.FirstOrDefault(x => x.Id == rezervacijaRequest.VoditeljId);
            if (voditeljDB == null)
            {
                return BadRequest("Nemoguće ažurirati rezervaciju -> Voditelj po Id-u ne postoji u bazi.");
            }

            var sobaDB = _context.Soba.FirstOrDefault(x => x.Id == rezervacijaRequest.SobaId);
            if (sobaDB == null)
            {
                return BadRequest("Nemoguće ažurirati rezervaciju -> Soba po Id-u ne postoji u bazi.");
            }

            rezervacijaDB.DatumOd = rezervacijaRequest.DatumOd;
            rezervacijaDB.DatumDo = rezervacijaRequest.DatumDo;
            rezervacijaDB.Aktivna = rezervacijaRequest.Aktivna;
            rezervacijaDB.GostId = gostDB.Id;
            rezervacijaDB.VoditeljId = voditeljDB.Id;
            rezervacijaDB.SobaId = sobaDB.Id;

            _context.Entry(rezervacijaDB).State = EntityState.Modified;

            try
            {
                _context.SaveChanges();
                return Ok("Promjena podataka o rezervaciji je uspješno pohranjena.");
            }
            catch (Exception)
            {
                return BadRequest("Ažuriranje na bazi nije uspjelo.");
            }
        }

        // POST: api/Rezervacija
        [HttpPost]
        public ActionResult<Rezervacija> PostRezervacija(Rezervacija rezervacijaRequest)
        {
            var rezervacijaDB = _context.Rezervacija.FirstOrDefault(x => x.Id == rezervacijaRequest.Id);

            if (rezervacijaDB == null)
            {
                var gostDB = _context.Gost.FirstOrDefault(x => x.Id == rezervacijaRequest.GostId);
                if (gostDB == null)
                {
                    return BadRequest("Unos nove rezervacije nije moguć -> Gost po zadanom Id-u ne postoji u bazi.");
                }

                var voditeljDB = _context.Voditelj.FirstOrDefault(x => x.Id == rezervacijaRequest.VoditeljId);
                if (voditeljDB == null)
                {
                    return BadRequest("Unos nove rezervacije nije moguć -> Voditelj po Id-u ne postoji u bazi.");
                }

                var sobaDB = _context.Soba.FirstOrDefault(x => x.Id == rezervacijaRequest.SobaId);
                if (sobaDB == null)
                {
                    return BadRequest("Unos nove rezervacije nije moguć -> Soba po Id-u ne postoji u bazi.");
                }

                if (rezervacijaRequest.Sifra.Length != 11) return BadRequest("Unos nove rezervacije nije moguć -> Sifra mora imati 11 znakova");

                var rezervacije = _context.Rezervacija.Where(x => x.SobaId == sobaDB.Id).ToList();

                if (rezervacije.Any())
                {
                    foreach (var rez in rezervacije)
                    {
                        if (((rezervacijaRequest.DatumOd <= rez.DatumOd && rezervacijaRequest.DatumDo <= rez.DatumOd)
                            || (rezervacijaRequest.DatumOd >= rez.DatumDo && rezervacijaRequest.DatumDo >= rez.DatumDo)) == false)
                            return BadRequest("Unos nove rezervacije nije moguć -> Soba je zauzeta u odabranom datumskom terminu.");
                    }
                }

                rezervacijaRequest.Aktivna = 1;
                _context.Rezervacija.Add(rezervacijaRequest);

                // moram napraviti save kako bi se generirao ID rezervacije
                _context.SaveChanges();

                Random random = new Random();
                StringBuilder randomSifra = new StringBuilder();

                for (int i = 0; i < 11; i++)
                {
                    randomSifra.Append(random.Next(0, 9));
                }
                // prilikom stvaranja rezervacije, stvara se račun
                var racun = new Racun()
                {
                    UkupnoBezPopusta = 0,
                    IznosPopusta = 0,
                    Ukupno = 0,
                    Placeno = false,
                    RezervacijaId = rezervacijaRequest.Id,
                    Sifra = randomSifra.ToString()
                };

                _context.Racun.Add(racun);
                _context.SaveChanges();
                return CreatedAtAction("GetRezervacija", new { rezervacijaRequest.Id }, rezervacijaRequest);
            }
            return BadRequest("Pokušaj unosa nije uspio. Rezervacija već postoji u bazi.");
        }

        // DELETE: api/Rezervacija/4FF2BA72-2A10-4EB6-84E9-59E572E7F97D
        [HttpDelete("{id}")]
        public IActionResult DeleteRezervacija(Guid id)
        {
            var rezervacija = _context.Rezervacija.FirstOrDefault(x => x.Id == id);
            if (rezervacija == null)
            {
                return BadRequest("Rezervacija po Id-u ne postoji u bazi.");
            }

            var racun = _context.Racun
                        .Include(x => x.StavkaRacuna)
                        .FirstOrDefault(x => x.RezervacijaId == id);

            if (racun != null)
            {
                if (racun.StavkaRacuna.Any())
                {
                    _context.StavkaRacuna.RemoveRange(racun.StavkaRacuna);
                }
                _context.Racun.Remove(racun);
                _context.SaveChanges();
            }

            _context.Rezervacija.Remove(rezervacija);
            _context.SaveChanges();

            return Ok(String.Format("Rezervacija sa Id-om {0} je uspješno obrisana.", id));
        }
    }
}
