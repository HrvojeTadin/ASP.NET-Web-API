using HotelAPI.Models;
using HotelAPI.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using System.Linq;

namespace HotelAPITests
{
    [TestClass]
    public class GuestTests
    {
        private HotelsContext _context;
        private GuestController _GuestController;

        [TestInitialize]
        public void TestInit()
        {
            _context = Utility.GetDbContext();
            _GuestController = new GuestController(_context);
        }

        [TestMethod]
        [Description("Get the list of guests.")]
        public void GetGuestsTest()
        {
            Assert.IsNotNull(_GuestController.GetGuests().Value, "The method returned null. There are no guests in the database.");
            Assert.IsTrue(_GuestController.GetGuests().Value.Count() > 0, "There is no information in the list.");
        }

        [TestMethod]
        [Description("Get one guest by Id.")]
        public void GetGuestTest()
        {
            var guest = _GuestController.GetGuest(Guid.Parse("E3AD5D92-B30D-4A61-AEA9-90C601D8C9E3")).Value;

            Assert.IsNotNull(guest, "The method returned null.");
            Assert.IsInstanceOfType(guest, typeof(Guest), "The object is not of the Guest type.");
            Assert.AreEqual(guest.Id, Guid.Parse("E3AD5D92-B30D-4A61-AEA9-90C601D8C9E3"), "Id's are not the same.");
        }

        [TestMethod]
        [Description("Enter of one Guest.")]
        public void PostGuestTest()
        {
            Guest guest = new Guest();
            guest.Name = "Edith";
            guest.Surname = "Hanson";

            Random random = new Random();
            StringBuilder output = new StringBuilder();

            for (int i = 0; i < 11; i++)
            {
                output.Append(random.Next(0, 9));
            }
            guest.PIN = output.ToString();

            var GuestsBefore = _context.Guest.Count();
            _GuestController.PostGuest(guest);
            var GuestsAfter = _context.Guest.Count();

            Assert.IsTrue(GuestsAfter > GuestsBefore, "Count of GuestsAfter is not greater then GuestsBefore.");
        }

        [TestMethod]
        [Description("The case where there is a Guest with the same ID in the database.")]
        public void PostExistingGuest()
        {
            Guest guest = new Guest();
            guest.Id = Guid.Parse("BA3279A7-AB43-41EC-9867-6F7B7E646C0E");
            guest.Name = "Edith";
            guest.Surname = "Hanson";

            Random random = new Random();
            StringBuilder output = new StringBuilder();

            for (int i = 0; i < 11; i++)
            {
                output.Append(random.Next(0, 9));
            }
            guest.PIN = output.ToString();

            var GuestsBefore = _context.Guest.Count();
            _GuestController.PostGuest(guest);
            var GuestsAfter = _context.Guest.Count();

            Assert.IsTrue(GuestsAfter == GuestsBefore, "Count of GuestsAfter is not equal then GuestsBefore.");
        }

        [TestMethod]
        [Description("Editing data on an existing Guest.")]
        public void PutGuestTest()
        {
            Guest guest = new Guest();
            guest.Id = Guid.Parse("BA3279A7-AB43-41EC-9867-6F7B7E646C0E");
            guest.Name = "Edith";
            guest.Surname = "Hanson";

            _GuestController.PutGuest(guest);

            var GuestAfter = _context.Guest.Find(guest.Id);

            Assert.IsNotNull(GuestAfter, "The method returned null.");
            Assert.AreEqual(guest.Name, GuestAfter.Name, "Guest name is not equal to GuestsAfter name.");
            Assert.AreEqual(guest.Surname, GuestAfter.Surname, "Guest Surname and GuestAfter Surname are not equel.");
        }

        [TestMethod]
        [Description("Change of non-existent Guest data.")]
        public void PutNeExistingGuestTest()
        {
            Guest guest = new Guest();
            guest.Id = Guid.Parse("BA3279A7-AB43-41EC-9867-6F7B7E646C1E");
            guest.Name = "Little";
            guest.Surname = "Mouse";

            _GuestController.PutGuest(guest);

            var GuestAfter = _context.Guest.Find(guest.Id);

            Assert.IsNull(GuestAfter, "The method did not return null.");
        }

        [TestMethod]
        [Description("Deleting an existing Guest")]
        public void DeleteGuestTest()
        {
            var id = Guid.Parse("0D4A6A72-1E8E-44B6-8E40-3E68D924F107");

            Assert.IsNotNull(_context.Guest.Find(id), "The method returned null. the guest can't be found.");

            var guest = _context.Guest.Find(id);

            var deletedGuest = _GuestController.DeleteGuest(id).Value;

            Assert.IsNotNull(deletedGuest, "The method returned null. The deleted guest can't be found.");

            Assert.IsNull(_context.Guest.Find(id), "The method did not return null. It finds Guest in the database.");

            _context.Add(guest);
            _context.SaveChanges();
        }

        [TestMethod]
        [Description("Deleting a non-existent Guest")]
        public void DeleteNeExistingGuest()
        {
            var id = Guid.Parse("0D4A6A72-1E8E-44B6-8E40-55555524F107");

            Assert.IsNull(_context.Guest.Find(id), "MThe method did not return null. It finds Guest in the database.");

            var deletedGuest = _GuestController.DeleteGuest(id).Value;

            Assert.IsNull(deletedGuest, "The method did not return null.");
            Assert.IsNull(_context.Guest.Find(id), "The method did not return null.");
        }
    }
}
