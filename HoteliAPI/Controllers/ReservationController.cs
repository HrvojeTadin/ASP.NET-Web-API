using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelAPI.Models;
using System.Text;

namespace HotelAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly HotelsContext _context;

        public ReservationController(HotelsContext context)
        {
            _context = context;
        }

        // GET: api/Reservation
        [HttpGet]
        public ActionResult<IEnumerable<Reservation>> GetReservation()
        {
            var reservations = _context.Reservation.ToList();

            if (reservations.Count > 0)
            {
                return reservations;
            }
            else
                return NoContent();
        }

        // GET: api/Reservation/4FF2BA72-2A10-4EB6-84E9-59E572E7F97D
        [HttpGet("{id}")]
        public ActionResult<Reservation> GetReservation(Guid id)
        {
            var reservation = _context.Reservation.FirstOrDefault(x => x.Id == id);

            if (reservation == null)
            {
                return BadRequest("Reservation by Id does not exist in the database.");
            }

            return reservation;
        }

        // PUT: api/Reservation
        [HttpPut]
        public IActionResult PutReservation(Reservation reservationRequest)
        {
            var reservationDB = _context.Reservation.FirstOrDefault(x => x.Id == reservationRequest.Id);

            if (reservationDB == null)
            {
                return BadRequest("Reservation by default code does not exist in the database.");
            }

            var guestDB = _context.Guest.FirstOrDefault(x => x.Id == reservationRequest.GuestId);
            if (guestDB == null)
            {
                return BadRequest("Unable to update reservation -> The guest by default Id does not exist in the database.");
            }

            var hotelManagerDB = _context.HotelManager.FirstOrDefault(x => x.Id == reservationRequest.HotelManagerId);
            if (hotelManagerDB == null)
            {
                return BadRequest("Unable to update reservation -> Hotel manager by Id does not exist in the database.");
            }

            var roomDB = _context.Room.FirstOrDefault(x => x.Id == reservationRequest.RoomId);
            if (roomDB == null)
            {
                return BadRequest("Unable to update reservation -> Room by Id does not exist in the database.");
            }

            reservationDB.DateFrom = reservationRequest.DateFrom;
            reservationDB.DateTo = reservationRequest.DateTo;
            reservationDB.IsActive = reservationRequest.IsActive;
            reservationDB.GuestId = guestDB.Id;
            reservationDB.HotelManagerId = hotelManagerDB.Id;
            reservationDB.RoomId = roomDB.Id;

            _context.Entry(reservationDB).State = EntityState.Modified;

            try
            {
                _context.SaveChanges();
                return Ok("The change of reservation data is saved successfully.");
            }
            catch (Exception)
            {
                return BadRequest("Database update has failed.");
            }
        }

        // POST: api/Reservation
        [HttpPost]
        public ActionResult<Reservation> PostReservation(Reservation reservationRequest)
        {
            var reservationDB = _context.Reservation.FirstOrDefault(x => x.Id == reservationRequest.Id);

            if (reservationDB == null)
            {
                var guestDB = _context.Guest.FirstOrDefault(x => x.Id == reservationRequest.GuestId);
                if (guestDB == null)
                {
                    return BadRequest("It is not possible to enter new reservation -> The guest by default Id does not exist in the database.");
                }

                var hotelManagerDB = _context.HotelManager.FirstOrDefault(x => x.Id == reservationRequest.HotelManagerId);
                if (hotelManagerDB == null)
                {
                    return BadRequest("It is not possible to enter new reservation -> Hotel manager by Id does not exist in the database.");
                }

                var roomDB = _context.Room.FirstOrDefault(x => x.Id == reservationRequest.RoomId);
                if (roomDB == null)
                {
                    return BadRequest("It is not possible to enter new reservation -> room by Id does not exist in the database.");
                }

                if (reservationRequest.PIN.Length != 11) return BadRequest("It is not possible to enter new reservation -> PIN must be 11 characters long");

                var reservations = _context.Reservation.Where(x => x.RoomId == roomDB.Id).ToList();

                if (reservations.Any())
                {
                    foreach (var rez in reservations)
                    {
                        if (((reservationRequest.DateFrom <= rez.DateFrom && reservationRequest.DateTo <= rez.DateFrom)
                            || (reservationRequest.DateFrom >= rez.DateTo && reservationRequest.DateTo >= rez.DateTo)) == false)
                            return BadRequest("It is not possible to enter new reservation -> Room is occupied at the selected period.");
                    }
                }

                reservationRequest.IsActive = 1;
                _context.Reservation.Add(reservationRequest);

                // i have to save it to generate a reservation ID
                _context.SaveChanges();

                Random random = new Random();
                StringBuilder randomPIN = new StringBuilder();

                for (int i = 0; i < 11; i++)
                {
                    randomPIN.Append(random.Next(0, 9));
                }
                // When creating reservations, an account is created
                var Invoice = new Invoice()
                {
                    TotalWithoutDiscounts = 0,
                    DiscountAmount = 0,
                    InTotal = 0,
                    Paid = false,
                    ReservationId = reservationRequest.Id,
                    PIN = randomPIN.ToString()
                };

                _context.Invoice.Add(Invoice);
                _context.SaveChanges();
                return CreatedAtAction("GetReservation", new { reservationRequest.Id }, reservationRequest);
            }
            return BadRequest("An attempt to enter failed. Reservation already exists in the database.");
        }

        // DELETE: api/Reservation/4FF2BA72-2A10-4EB6-84E9-59E572E7F97D
        [HttpDelete("{id}")]
        public IActionResult DeleteReservation(Guid id)
        {
            var reservation = _context.Reservation.FirstOrDefault(x => x.Id == id);
            if (reservation == null)
            {
                return BadRequest("Reservation by Id does not exist in the database.");
            }

            var invoice = _context.Invoice
                        .Include(x => x.InvoiceItem)
                        .FirstOrDefault(x => x.ReservationId == id);

            if (invoice != null)
            {
                if (invoice.InvoiceItem.Any())
                {
                    _context.InvoiceItem.RemoveRange(invoice.InvoiceItem);
                }
                _context.Invoice.Remove(invoice);
                _context.SaveChanges();
            }

            _context.Reservation.Remove(reservation);
            _context.SaveChanges();

            return Ok(String.Format(".Reservation with Id {0} was successfully deleted", id));
        }
    }
}
