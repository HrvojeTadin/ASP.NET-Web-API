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
    public class SobaController : ControllerBase
    {
        private readonly HoteliContext _context;

        public SobaController(HoteliContext context)
        {
            _context = context;
        }

        // GET: api/Soba
        [HttpGet]
        public ActionResult<IEnumerable<Soba>> GetSobe()
        {
            var soba = _context.Soba.ToList();

            if (soba.Count > 0)
            {
                return soba;
            }
            else
                return NoContent();
        }


        // GET: api/Soba/88ef6d64-e524-4251-8a4e-27a407fa6f53
        [HttpGet("{id}")]
        public ActionResult<Soba> GetSoba(Guid id)
        {
            var soba = _context.Soba.FirstOrDefault(x => x.Id == id);

            if (soba == null)
            {
                return BadRequest("Soba po zadanom Id-u ne postoji u bazi.");
            }

            return soba;
        }

        // PUT: api/Soba
        [HttpPut]
        public IActionResult PutSoba(Soba soba)
        {
            var sobaDB = _context.Soba.FirstOrDefault(x => x.Id == soba.Id);

            if (sobaDB == null)
            {
                return BadRequest("Soba po zadanom Id-u ne postoji u bazi.");
            }

            sobaDB.CijenaPoNocenju = soba.CijenaPoNocenju;

            _context.Entry(sobaDB).State = EntityState.Modified;

            try
            {
                _context.SaveChanges();
                return Ok("Promjena podataka o sobi je uspješno pohranjena.");
            }
            catch (Exception)
            {
                return BadRequest("Ažuriranje na bazi nije uspjelo.");
            }
        }

        // POST: api/Soba
        [HttpPost]
        public ActionResult<Soba> PostSoba(Soba soba)
        {
            var sobaDB = _context.Soba.FirstOrDefault(x => x.Id == soba.Id);

            if (sobaDB == null)
            {
                _context.Soba.Add(soba);
                _context.SaveChanges();
                return CreatedAtAction("GetSoba", new { soba.Id }, soba);
            }
            return BadRequest("Pokušaj unosa nije uspio. Soba već postoji u bazi.");
        }


        // DELETE: api/Soba/5
        [HttpDelete("{id}")]
        public ActionResult<Soba> DeleteSoba(Guid id)
        {
            var soba = _context.Soba.FirstOrDefault(x => x.Id == id);

            if (soba == null)
            {
                return BadRequest("Soba po zadanom Id-u ne postoji u bazi.");
            }

            _context.Soba.Remove(soba);
            _context.SaveChanges();

            return soba;
        }
    }
}
