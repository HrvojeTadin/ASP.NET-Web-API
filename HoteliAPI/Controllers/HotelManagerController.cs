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
    public class HotelManagerController : ControllerBase
    {
        private readonly HotelsContext _context;

        public HotelManagerController(HotelsContext context)
        {
            _context = context;
        }

        // GET: api/HotelManager
        [HttpGet]
        public ActionResult<IEnumerable<HotelManager>> GetHotelManageri()
        {
            var hotelManager = _context.HotelManager.ToList();

            if (hotelManager.Count() > 0)
            {
                return hotelManager;
            }
            else
                return NoContent();
        }

        // GET: api/HotelManager/f8231975-a42a-4f9b-9bf8-1597307b33ab
        [HttpGet("{id}")]
        public ActionResult<HotelManager> GetHotelManager(Guid id)
        {
            var hotelManager = _context.HotelManager.FirstOrDefault(x => x.Id == id);

            if (hotelManager == null)
            {
                return BadRequest("Hotel manager by given ID does not exist in database.");
            }
            return hotelManager;
        }

        // PUT: api/HotelManager
        [HttpPut]
        public ActionResult<HotelManager> PutHotelManager(HotelManager hotelManager)
        {
            var hotelManagerDB = _context.HotelManager.FirstOrDefault(x => x.Id == hotelManager.Id);

            if (hotelManagerDB == null)
            {
                return BadRequest("Hotel manager by given ID does not exist in database.");
            }

            hotelManagerDB.Name = hotelManager.Name;
            hotelManagerDB.Surname = hotelManager.Surname;
            hotelManagerDB.PIN = hotelManager.PIN;

            _context.Entry(hotelManagerDB).State = EntityState.Modified;

            try
            {
                _context.SaveChanges();
                return Ok("Hotel manager change data has succeed.");
            }
            catch (Exception)
            {
                return BadRequest("Database update has failed.");
            }
        }

        [HttpPost]
        public ActionResult<HotelManager> PostHotelManager(HotelManager hotelManager)
        {
            var hotelManagerDB = _context.HotelManager.FirstOrDefault(x => x.Id == hotelManager.Id);

            if (hotelManagerDB == null)
            {
                _context.HotelManager.Add(hotelManager);
                _context.SaveChanges();
                return CreatedAtAction("GetHotelManager", new { hotelManager.Id}, hotelManager);
            }
            return BadRequest("Hotel manager insert did not succeed. This hotel manager already exist in database.");

        }

        // DELETE: api/HotelManager/77731975-a42a-4f9b-9bf8-1597307b33EE
        [HttpDelete("{id}")]
        public ActionResult<HotelManager> DeleteHotelManager(Guid id)
        {
            var hotelManager = _context.HotelManager.FirstOrDefault(x => x.Id == id);

            if (hotelManager == null)
            {
                return BadRequest("Hotel manager by given ID does not exist in database.");
            }

            _context.HotelManager.Remove(hotelManager);
            _context.SaveChanges();

            return hotelManager;
        }
    }
}
