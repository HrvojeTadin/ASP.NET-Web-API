using HotelAPI.Controllers;
using HotelAPI.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace HotelAPITests
{
    [TestClass]
    public class ReservationTests
    {
        private HotelsContext _context;
        private ReservationController _ReservationController;

        [TestInitialize]
        public void TestInit()
        {
            _context = Utility.GetDbContext();
            _ReservationController = new ReservationController(_context);
        }

        [TestMethod]
        [Description("Get all reservations.")]
        public void GetAllReservationTest()
        {
            int countReservationDB = _context.Reservation.Count();
            int countReservationAPI = _ReservationController.GetReservation().Value.Count();
            Assert.AreEqual(countReservationDB, countReservationAPI, "Number of reservations is not equaL.");
        }

        [TestMethod]
        [Description("Get one reservation.")]
        public void GetReservationTest()
        {
            var id = Guid.Parse("FC84634E-A97C-49A6-8D7D-6D151142ACFD");
            Assert.IsNotNull(_context.Reservation.Find(id), "No reservation in DB by ID.");

            var reservation = _ReservationController.GetReservation(id).Value;
            Assert.IsNotNull(reservation, "NNo reservation in DB by ID.");
            Assert.AreEqual(reservation.Id, id, "Reservation ID from DB is not equal to reservation ID from API.");
        }

        [TestMethod]
        [Description("Insert new reservation.")]
        public void PostReservationTest()
        {
            Reservation reservation = new Reservation()
            {
                GuestId = Guid.Parse("58D8BFA5-7CF8-4F3F-B03D-0DE87DD9A48A"),
                HotelManagerId = Guid.Parse("AF959367-F535-433C-82A8-6236CFA80FDB"),
                RoomId = Guid.Parse("48CC484C-90B6-49C0-A761-9EE936AF5C0D"),
                PIN = "12345678901",
                DateFrom = DateTime.Now.AddDays(-2),
                DateTo = DateTime.Now.AddDays(5)
            };

            _ReservationController.PostReservation(reservation);

            Reservation resDB = _context.Reservation.FirstOrDefault(x => x.PIN == reservation.PIN);
            Assert.IsNotNull(resDB, "There is no reservation in DB by ID.");
            Assert.IsTrue(resDB.IsActive == 1, "Reservation is not active.");
            Invoice invoice = _context.Invoice.FirstOrDefault(x => x.ReservationId == resDB.Id);
            Assert.IsNotNull(invoice, "There is no invoice in DB by ID.");

            _context.Invoice.Remove(invoice);
            _context.Reservation.Remove(resDB);
            _context.SaveChanges();
        }

        [TestMethod]
        [Description("Insert existing reservation.")]
        public void PostExistingReservationTest()
        {
            Reservation reservation = new Reservation()
            {
                GuestId = Guid.Parse("58D8BFA5-7CF8-4F3F-B03D-0DE87DD9A48A"),
                HotelManagerId = Guid.Parse("AF959367-F535-433C-82A8-6236CFA80FDB"),
                RoomId = Guid.Parse("48CC484C-90B6-49C0-A761-9EE936AF5C0D"),
                PIN = "55555566666",
                DateFrom = DateTime.Now.AddDays(7),
                DateTo = DateTime.Now.AddDays(9)
            };

            int reservationBefore = _context.Reservation.Count();
            _ReservationController.PostReservation(reservation);
            int reservationAfter = _context.Reservation.Count();
            Assert.AreEqual(reservationBefore, reservationAfter, "Number of reservation on DB is not as before.");
            Assert.IsNotNull(_context.Reservation.FirstOrDefault(x => x.PIN == reservation.PIN), "There is no reservation in DB by PIN.");
        }

        [TestMethod]
        [Description("Update existing reservation.")]
        public void PutReservationTest()
        {
            Reservation reservation = new Reservation()
            {
                Id = Guid.Parse("FC84634E-A97C-49A6-8D7D-6D151142ACFD"),
                DateFrom = DateTime.Now.AddDays(7),
                DateTo = DateTime.Now.AddDays(11),
                IsActive = 1,
                PIN = "55555566666",
                GuestId = Guid.Parse("58D8BFA5-7CF8-4F3F-B03D-0DE87DD9A48A"),
                HotelManagerId = Guid.Parse("AF959367-F535-433C-82A8-6236CFA80FDB"),
                RoomId = Guid.Parse("48CC484C-90B6-49C0-A761-9EE936AF5C0D")
            };

            var reservationDB = _context.Reservation.FirstOrDefault(x => x.PIN == reservation.PIN);
            Assert.IsNotNull(reservationDB, "There is no reservation in DB by PIN.");

            _ReservationController.PutReservation(reservation);

            Assert.AreEqual(reservation.Id, reservationDB.Id, "The ID of reservation is not equal.");
            Assert.AreEqual(reservation.DateFrom, reservationDB.DateFrom, "DateFrom is not equal.");
            Assert.AreEqual(reservation.DateTo, reservationDB.DateTo, "DateTo is not equal.");
            Assert.AreEqual(reservation.IsActive, reservationDB.IsActive, "IsActive is not equal.");
            Assert.AreEqual(reservation.GuestId, reservationDB.GuestId, "GuestId is not equal.");
            Assert.AreEqual(reservation.HotelManagerId, reservationDB.HotelManagerId, "HotelManagerId is not equal.");
            Assert.AreEqual(reservation.RoomId, reservationDB.RoomId, "RoomId is not equal.");
        }

        [TestMethod]
        [Description("Update unexisting reservation.")]
        public void PutUnexistingReservationTest()
        {
            Reservation reservation = new Reservation()
            {
                Id = Guid.Parse("FC84634E-1111-1111-8D7D-6D151142ACFD"),
                DateFrom = DateTime.Now.AddDays(7),
                DateTo = DateTime.Now.AddDays(11),
                IsActive = 1,
                PIN = "55555566666",
                GuestId = Guid.Parse("58D8BFA5-7CF8-4F3F-B03D-0DE87DD9A48A"),
                HotelManagerId = Guid.Parse("AF959367-F535-433C-82A8-6236CFA80FDB"),
                RoomId = Guid.Parse("48CC484C-90B6-49C0-A761-9EE936AF5C0D")
            };

            _ReservationController.PutReservation(reservation);
            Assert.IsNull(_context.Reservation.Find(reservation.Id), "There is no reservation in DB by ID.");
        }

        [TestMethod]
        [Description("Delete existing reservation.")]
        public void DeleteReservationTest()
        {
            var id = Guid.Parse("C3366C16-6755-4814-9F62-8A24408891EB");

            Reservation rez = _context.Reservation.Find(id);

            Assert.IsNotNull(rez, "There is no reservation in DB by ID.");

            _ReservationController.DeleteReservation(id);

            Assert.IsNull(_context.Reservation.Find(id), "Reservation was not deleted, it's still in DB.");

            _context.Reservation.Add(rez);
            _context.SaveChanges();

            Assert.IsNotNull(_context.Reservation.Find(id), "Reservation is not id DB, after insert.");
        }

        [TestMethod]
        [Description("Delete unexisting reservation.")]
        public void DeleteUnexistingReservationTest()
        {
            var id = Guid.Parse("C3366C16-1111-1111-1111-8A24408891EB");

            Assert.IsNull(_context.Reservation.Find(id), "Reservation is in DB, even we don't expect it by ID.");

            _ReservationController.DeleteReservation(id);

            Assert.IsNull(_context.Reservation.Find(id), "Reservation was not deleted, it's still in DB.");
        }
    }
}
