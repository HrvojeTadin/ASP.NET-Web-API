using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HoteliAPI.Models;

namespace HoteliAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StavkaRacunaController : ControllerBase
    {
        private readonly HoteliContext _context;

        public StavkaRacunaController(HoteliContext context)
        {
            _context = context;
        }

        // GET: api/StavkaRacuna/d6b35732-c12a-44fa-91c9-843f98ef7fd3
        [HttpGet("{id}")]
        public ActionResult<IEnumerable<StavkaRacuna>> GetStavkeRacuna(Guid id)
        {
            var racun = _context.Racun.FirstOrDefault(x => x.Id == id);
            if (racun == null)
            {
                return BadRequest("Račun po Id-u ne postoji u bazi.");
            }

            var stavkeRacuna = _context.StavkaRacuna.Where(x => x.RacunId == id).ToList();

            if (stavkeRacuna.Count > 0)
            {

                return stavkeRacuna;
            }
            else
                return NoContent();
        }

        // GET: api/StavkaRacuna/d6b35732-c12a-44fa-91c9-843f98ef7fd3/d6b35732-c12a-44fa-91c9-843f98ef7777
        [HttpGet("{idRacuna}/{idStavke}")]
        public ActionResult<StavkaRacuna> GetStavkaRacuna(Guid idRacuna, Guid idStavke)
        {
            var racun = _context.Racun.FirstOrDefault(x => x.Id == idRacuna);
            if (racun == null)
            {
                return BadRequest("Račun po zadanoj Šifri računa ne postoji u bazi.");
            }

            var stavkaRacuna = _context.StavkaRacuna
                .Where(x => x.Id == idStavke && x.RacunId == idRacuna).SingleOrDefault();

            if (stavkaRacuna != null)
            {
                return stavkaRacuna;
            }
            else
                return BadRequest("Stavka računa po ID-u nije dodjeljena zadanom računu.");
        }

        // PUT: api/StavkaRacuna
        [HttpPut]
        public ActionResult PutStavkaRacuna(StavkaRacuna stavkaRacunaRequest)
        {
            var stavkaRacuna = _context.StavkaRacuna
                .Where(x => x.Sifra == stavkaRacunaRequest.Sifra && x.RacunId == stavkaRacunaRequest.RacunId).SingleOrDefault();

            if (stavkaRacuna == null)
            {
                return BadRequest("Stavka računa po ID-u ne postoji u bazi.");
            }

            var usluga = _context.Usluga.FirstOrDefault(x => x.Id == stavkaRacunaRequest.UslugaId);
            if (usluga == null)
            {
                return BadRequest("Nemoguće ažurirati stavku računa -> Usluga po zadanom ID-u ne postoji u bazi.");
            }

            try
            {
                // izračun ukupne cijene stavke
                stavkaRacuna.UkupnaCijena = stavkaRacunaRequest.Kolicina * usluga.JedinicnaCijena;
                stavkaRacuna.UslugaId = usluga.Id;
                stavkaRacuna.Kolicina = stavkaRacunaRequest.Kolicina;

                _context.Entry(stavkaRacuna).State = EntityState.Modified;
                _context.SaveChanges();

                // update Ukupne cijene na računu
                var racun = _context.Racun
                    .Include(x => x.StavkaRacuna)
                    .FirstOrDefault(x => x.Id == stavkaRacuna.RacunId);

                racun.UkupnoBezPopusta = racun.StavkaRacuna.Sum(x => x.UkupnaCijena);  //Suma svih ukupnih cijena stavaka računa
                racun.Ukupno = racun.UkupnoBezPopusta - (racun.UkupnoBezPopusta * racun.IznosPopusta);
                _context.Update(racun);
                _context.SaveChanges();

                return Ok("Promjena podataka o stavci računa je uspješno pohranjena.");
            }
            catch (Exception)
            {
                return BadRequest("Ažuriranje na bazi nije uspjelo.");
            }
        }

        // POST: api/StavkaRacuna
        [HttpPost]
        public ActionResult<StavkaRacuna> PostStavkaRacuna(StavkaRacuna stavkaRacunaRequest)
        {
            var racun = _context.Racun
                .Include(x => x.StavkaRacuna)
                .FirstOrDefault(x => x.Id == stavkaRacunaRequest.RacunId);

            if (racun == null)
            {
                return BadRequest("Nemoguće ažurirati stavku računa -> Račun po zadanom ID-u ne postoji u bazi.");
            }

            var stavkaRacuna = _context.StavkaRacuna
            .Where(x => x.Sifra == stavkaRacunaRequest.Sifra && x.RacunId == stavkaRacunaRequest.RacunId).SingleOrDefault();

            if (stavkaRacuna == null)
            {
                var uslugaDB = _context.Usluga.FirstOrDefault(x => x.Id == stavkaRacunaRequest.UslugaId);
                if (uslugaDB == null)
                {
                    return BadRequest("Nemoguće ažurirati stavku računa -> Usluga po zadanom ID-u ne postoji u bazi.");
                }
                stavkaRacunaRequest.UkupnaCijena = stavkaRacunaRequest.Kolicina * uslugaDB.JedinicnaCijena;

                _context.Add(stavkaRacunaRequest);
                _context.SaveChanges();

                // update Ukupne cijene na računu
                racun.UkupnoBezPopusta = racun.StavkaRacuna.Sum(x => x.UkupnaCijena);  //Suma svih ukupnih cijena stavaka računa
                racun.Ukupno = racun.UkupnoBezPopusta - (racun.UkupnoBezPopusta * racun.IznosPopusta);
                _context.Update(racun);
                _context.SaveChanges();

                return CreatedAtAction("GetStavkaRacuna", new { stavkaRacunaRequest.RacunId, stavkaRacunaRequest.Id }, stavkaRacunaRequest);
            }
            return BadRequest("Pokušaj unosa nije uspio. Stavka računa već postoji u bazi.");
        }

        // DELETE: api/StavkaRacuna/cd50460c-0070-4c74-ade5-065d289eb516/d6b35732-c12a-44fa-91c9-843f98ef7fd3
        [HttpDelete("{idRacuna}/{idStavke}")]
        public IActionResult DeleteStavkaRacuna(Guid idRacuna, Guid idStavke)
        {
            var stavka = _context.StavkaRacuna.FirstOrDefault(x => x.Id == idStavke);
            if (stavka == null)
            {
                return BadRequest("Stavka računa po zadanom ID-u ne postoji u bazi.");
            }

            var racun = _context.Racun
                        .Include(x => x.StavkaRacuna)
                        .FirstOrDefault(x => x.Id == idRacuna);
            if (racun == null)
            {
                return BadRequest("Račun po zadanom ID-u ne postoji u bazi.");
            }

            _context.StavkaRacuna.Remove(stavka);
            _context.SaveChanges();

            // update Ukupne cijene na računu
            racun.StavkaRacuna.Remove(stavka);
            racun.UkupnoBezPopusta = racun.StavkaRacuna.Sum(x => x.UkupnaCijena) - stavka.UkupnaCijena;  //Suma svih ukupnih cijena stavaka računa - iznos stavke
            racun.Ukupno = racun.UkupnoBezPopusta - (racun.UkupnoBezPopusta * racun.IznosPopusta);
            _context.Update(racun);
            _context.SaveChanges();

            return Ok(String.Format("Stavka računa ID-a {0} je uspješno obrisana.", idStavke));
        }
    }
}
