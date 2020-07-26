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
    public class UslugaController : ControllerBase
    {
        private readonly HoteliContext _context;

        public UslugaController(HoteliContext context)
        {
            _context = context;
        }

        // GET: api/Usluga
        [HttpGet]
        public ActionResult<IEnumerable<Usluga>> GetUsluge()
        {
            var usluga = _context.Usluga.ToList();

            if (usluga.Count() > 0)
            {
                return usluga;
            }
            else
                return NoContent();
        }


        // GET: api/Usluga/26e257cd-8aa7-4f50-93cc-00288ff42041
        [HttpGet("{id}")]
        public ActionResult<Usluga> GetUsluga(Guid id)
        {
            var usluga = _context.Usluga.FirstOrDefault(x => x.Id == id);

            if (usluga == null)
            {
                return BadRequest("Usluga po zadanoj sifri ne postoji u bazi.");
            }

            return usluga;
        }

        // PUT: api/Usluga
        [HttpPut]
        public IActionResult PutUsluga(Usluga usluga)
        {
            var uslugaDB = _context.Usluga.FirstOrDefault(x => x.Id == usluga.Id);

            if (uslugaDB == null)
            {
                return BadRequest("Gost po zadanom ID-u ne postoji u bazi.");
            }

            uslugaDB.Naziv = usluga.Naziv;
            uslugaDB.JedinicnaCijena = usluga.JedinicnaCijena;

            _context.Entry(uslugaDB).State = EntityState.Modified;

            try
            {
                _context.SaveChanges();
                return Ok("Promjena podataka o usluzi je uspješno pohranjena.");
            }
            catch (Exception)
            {
                return BadRequest("Ažuriranje na bazi nije uspjelo.");
            }
        }

        // POST: api/Usluga
        [HttpPost]
        public ActionResult<Usluga> PostUsluga(Usluga usluga)
        {
            var uslugaDB = _context.Usluga.FirstOrDefault(x => x.Id == usluga.Id);

            if (uslugaDB == null)
            {
                _context.Usluga.Add(usluga);
                _context.SaveChanges();
                return CreatedAtAction("GetUsluga", new { usluga.Id }, usluga);
            }
            return BadRequest("Pokušaj unosa nije uspio. Usluga već postoji u bazi.");
        }

        // DELETE: api/Usluga/5ad7216f-fc9e-41a2-8e87-6c82b566c1a7
        [HttpDelete("{id}")]
        public ActionResult<Usluga> DeleteUsluga(Guid id)
        {
            var usluga = _context.Usluga.FirstOrDefault(x => x.Id == id);

            if (usluga == null)
            {
                return BadRequest("Usluga po zadanom ID-u ne postoji u bazi.");
            }

            _context.Usluga.Remove(usluga);
            _context.SaveChanges();

            return usluga;
        }
    }
}
