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
    public class GostController : ControllerBase
    {
        private readonly HoteliContext _context;

        public GostController(HoteliContext context)
        {
            _context = context;
        }

        // GET: api/Gost
        [HttpGet]
        public ActionResult<IEnumerable<Gost>> GetGosti()
        {
            var gosti = _context.Gost.ToList();

            if (gosti.Count > 0)
            {
                return gosti;
            }
            else
                return NoContent();
        }

        // GET: api/Gost/55236552652
        [HttpGet("{id}")]
        public ActionResult<Gost> GetGost(Guid id)
        {
            var gost = _context.Gost.FirstOrDefault(x => x.Id == id);

            if (gost == null)
            {
                return BadRequest("Gost po zadanom ID-u ne postoji u bazi.");
            }

            return gost;
        }

        // PUT: api/Gost
        [HttpPut]
        public IActionResult PutGost(Gost gost)
        {
            var gostDB = _context.Gost.FirstOrDefault(x => x.Id == gost.Id);

            if (gostDB == null)
            {
                return BadRequest("Gost po zadanom Id-u ne postoji u bazi.");
            }

            gostDB.Ime = gost.Ime;
            gostDB.Prezime = gost.Prezime;

            _context.Entry(gostDB).State = EntityState.Modified;

            try
            {
                _context.SaveChanges();
                return Ok("Promjena podataka o gostu je uspješno pohranjena.");
            }
            catch (Exception)
            {
                return BadRequest("Ažuriranje na bazi nije uspjelo.");
            }
        }

        // POST: api/Gost
        [HttpPost]
        public ActionResult<Gost> PostGost(Gost gost)
        {
            var gostDB = _context.Gost.FirstOrDefault(x => x.Id == gost.Id);

            if (gostDB == null)
            {
                _context.Gost.Add(gost);
                _context.SaveChanges();
                return CreatedAtAction("GetGost", new { gost.Id }, gost);
            }
            return BadRequest("Pokušaj unosa nije uspio. Gost već postoji u bazi.");
        }

        // DELETE: api/Gost/54957385647
        [HttpDelete("{id}")]
        public ActionResult<Gost> DeleteGost(Guid id)
        {
            var gost = _context.Gost.FirstOrDefault(x => x.Id == id);

            if (gost == null)
            {
                return BadRequest("Gost po zadanom Id-u ne postoji u bazi.");
            }

            _context.Gost.Remove(gost);
            _context.SaveChanges();

            return gost;
        }
    }
}
