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
    public class RoomController : ControllerBase
    {
        private readonly HotelsContext _context;

        public RoomController(HotelsContext context)
        {
            _context = context;
        }

        // GET: api/Room
        [HttpGet]
        public ActionResult<IEnumerable<Room>> GetRooms()
        {
            var room = _context.Room.ToList();

            if (room.Count > 0)
            {
                return room;
            }
            else
                return NoContent();
        }


        // GET: api/Room/88ef6d64-e524-4251-8a4e-27a407fa6f53
        [HttpGet("{id}")]
        public ActionResult<Room> GetRoom(Guid id)
        {
            var Room = _context.Room.FirstOrDefault(x => x.Id == id);

            if (Room == null)
            {
                return BadRequest("There is no room by given ID in the database.");
            }

            return Room;
        }

        // PUT: api/Room
        [HttpPut]
        public IActionResult PutRoom(Room Room)
        {
            var RoomDB = _context.Room.FirstOrDefault(x => x.Id == Room.Id);

            if (RoomDB == null)
            {
                return BadRequest("here is no room by given ID in the database.");
            }

            RoomDB.PricePerNight = Room.PricePerNight;

            _context.Entry(RoomDB).State = EntityState.Modified;

            try
            {
                _context.SaveChanges();
                return Ok("Update of room was successful.");
            }
            catch (Exception)
            {
                return BadRequest("Database update has failed.");
            }
        }

        // POST: api/Room
        [HttpPost]
        public ActionResult<Room> PostRoom(Room Room)
        {
            var RoomDB = _context.Room.FirstOrDefault(x => x.Id == Room.Id);

            if (RoomDB == null)
            {
                _context.Room.Add(Room);
                _context.SaveChanges();
                return CreatedAtAction("GetRoom", new { Room.Id }, Room);
            }
            return BadRequest("Can not insert the room. The room already exists in database.");
        }


        // DELETE: api/Room/5
        [HttpDelete("{id}")]
        public ActionResult<Room> DeleteRoom(Guid id)
        {
            var Room = _context.Room.FirstOrDefault(x => x.Id == id);

            if (Room == null)
            {
                return BadRequest("Tere is no room by given ID in the database.");
            }

            _context.Room.Remove(Room);
            _context.SaveChanges();

            return Room;
        }
    }
}
