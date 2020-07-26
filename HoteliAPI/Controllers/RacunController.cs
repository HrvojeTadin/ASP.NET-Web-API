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
    public class RacunController : ControllerBase
    {
        private readonly HoteliContext _context;

        public RacunController(HoteliContext context)
        {
            _context = context;
        }

        // GET: api/Racun
        [HttpGet]
        public ActionResult<IEnumerable<Racun>> GetRacuni()
        {
            var racuni = _context.Racun.Include(x => x.StavkaRacuna).ToList();

            if (racuni.Count > 0)
            {
                return racuni;
            }
            else
                return NoContent();
        }

        // GET: api/Racun/55415445544
        [HttpGet("{id}")]
        public ActionResult<Racun> GetRacun(Guid id)
        {
            var racun = _context.Racun.FirstOrDefault(x => x.Id == id);

            if (racun == null)
            {
                return BadRequest("Račun po Id-u ne postoji u bazi.");
            }

            return racun;
        }

        // PUT: api/Racun/5
        [HttpPut]
        public IActionResult PutRacun(Racun racunRequest)
        {
            var racunDB = _context.Racun.FirstOrDefault(x => x.Id == racunRequest.Id);

            if (racunDB == null)
            {
                return BadRequest("Račun prema Id-u ne postoji u bazi.");
            }
            _context.Entry(racunDB).State = EntityState.Modified;

            try
            {
                if (!racunDB.Placeno)
                {
                    racunDB.IznosPopusta = racunRequest.IznosPopusta;
                    racunDB.Ukupno = racunDB.UkupnoBezPopusta - (racunDB.UkupnoBezPopusta * racunDB.IznosPopusta);
                }

                _context.SaveChanges();
                return Ok("Promjena podataka o računu je uspješno pohranjena.");

            }
            catch (Exception)
            {
                return BadRequest("Ažuriranje na bazi nije uspjelo.");
            }
        }
    }
}
