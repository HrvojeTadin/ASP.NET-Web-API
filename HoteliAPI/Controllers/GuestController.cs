using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelAPI.Models;

namespace HotelAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GuestController : ControllerBase
    {
        private readonly HotelsContext _context;

        public GuestController(HotelsContext context)
        {
            _context = context;
        }

        // GET: api/Guest
        [HttpGet]
        public ActionResult<IEnumerable<Guest>> GetGuests()
        {
            var guests = _context.Guest.ToList();

            if (guests.Count > 0)
            {
                return guests;
            }
            else
                return NoContent();
        }

        // GET: api/Guest/55236552652
        [HttpGet("{id}")]
        public ActionResult<Guest> GetGuest(Guid id)
        {
            var guest = _context.Guest.FirstOrDefault(x => x.Id == id);

            if (guest == null)
            {
                return BadRequest("The guest by given Id does not exist in the database.");
            }

            return guest;
        }

        // PUT: api/Guest
        [HttpPut]
        public IActionResult PutGuest(Guest guest)
        {
            var guestDB = _context.Guest.FirstOrDefault(x => x.Id == guest.Id);

            if (guestDB == null)
            {
                return BadRequest("The guest by given Id does not exist in the database.");
            }

            guestDB.Name = guest.Name;
            guestDB.Surname = guest.Surname;

            _context.Entry(guestDB).State = EntityState.Modified;

            try
            {
                _context.SaveChanges();
                return Ok("The change of Guest data has been saved successfully.");
            }
            catch (Exception)
            {
                return BadRequest("Database update has failed.");
            }
        }

        // POST: api/Guest
        [HttpPost]
        public ActionResult<Guest> PostGuest(Guest guest)
        {
            var guestDB = _context.Guest.FirstOrDefault(x => x.Id == guest.Id);

            if (guestDB == null)
            {
                _context.Guest.Add(guest);
                _context.SaveChanges();
                return CreatedAtAction("GetGuest", new { guest.Id }, guest);
            }
            return BadRequest("An attempt to insert failed. The guest already exists in the base.");
        }

        // DELETE: api/Guest/54957385647
        [HttpDelete("{id}")]
        public ActionResult<Guest> DeleteGuest(Guid id)
        {
            var guest = _context.Guest.FirstOrDefault(x => x.Id == id);

            if (guest == null)
            {
                return BadRequest("The guest by given Id does not exist in the database.");
            }

            _context.Guest.Remove(guest);
            _context.SaveChanges();

            return guest;
        }
    }
}
