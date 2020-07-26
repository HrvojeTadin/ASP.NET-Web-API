using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HoteliAPI.Models;

namespace HoteliAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoditeljController : ControllerBase
    {
        private readonly HoteliContext _context;

        public VoditeljController(HoteliContext context)
        {
            _context = context;
        }

        // GET: api/Voditelj
        [HttpGet]
        public ActionResult<IEnumerable<Voditelj>> GetVoditelji()
        {
            var voditelj = _context.Voditelj.ToList();

            if (voditelj.Count() > 0)
            {
                return voditelj;
            }
            else
                return NoContent();
        }

        // GET: api/Voditelj/f8231975-a42a-4f9b-9bf8-1597307b33ab
        [HttpGet("{id}")]
        public ActionResult<Voditelj> GetVoditelj(Guid id)
        {
            var voditelj = _context.Voditelj.FirstOrDefault(x => x.Id == id);

            if (voditelj == null)
            {
                return BadRequest("Voditelj po zadanom ID-u nije pronađen u bazi.");
            }
            return voditelj;
        }

        // PUT: api/Voditelj
        [HttpPut]
        public ActionResult<Voditelj> PutVoditelj(Voditelj voditelj)
        {
            var voditeljDB = _context.Voditelj.FirstOrDefault(x => x.Id == voditelj.Id);

            if (voditeljDB == null)
            {
                return BadRequest("Voditelj po zadanom ID-u nije pronađen u bazi.");
            }

            voditeljDB.Ime = voditelj.Ime;
            voditeljDB.Prezime = voditelj.Prezime;
            voditeljDB.Sifra = voditelj.Sifra;

            _context.Entry(voditeljDB).State = EntityState.Modified;

            try
            {
                _context.SaveChanges();
                return Ok("Promjena podataka o voditelju je uspješno pohranjena.");
            }
            catch (Exception)
            {
                return BadRequest("Ažuriranje na bazi nije uspjelo.");
            }
        }

        [HttpPost]
        public ActionResult<Voditelj> PostVoditelj(Voditelj voditelj)
        {
            var voditeljDB = _context.Voditelj.FirstOrDefault(x => x.Id == voditelj.Id);

            if (voditeljDB == null)
            {
                _context.Voditelj.Add(voditelj);
                _context.SaveChanges();
                return CreatedAtAction("GetVoditelj", new { voditelj.Id}, voditelj);
            }
            return BadRequest("Pokušaj unosa nije uspio. Voditelj već postoji u bazi.");

        }

        // DELETE: api/Voditelj/77731975-a42a-4f9b-9bf8-1597307b33EE
        [HttpDelete("{id}")]
        public ActionResult<Voditelj> DeleteVoditelj(Guid id)
        {
            var voditelj = _context.Voditelj.FirstOrDefault(x => x.Id == id);

            if (voditelj == null)
            {
                return BadRequest("Voditelj po zadanom ID-u ne postoji u bazi.");
            }

            _context.Voditelj.Remove(voditelj);
            _context.SaveChanges();

            return voditelj;
        }
    }
}
