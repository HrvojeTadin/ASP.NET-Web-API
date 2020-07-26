using HotelAPI.Controllers;
using HotelAPI.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace HotelAPITests
{
    [TestClass]
    public class HotelManagerTests
    {
        private HotelsContext _context;
        private HotelManagerController _HotelManagerController;

        [TestInitialize]
        public void TestInit()
        {
            _context = Utility.GetDbContext();
            _HotelManagerController = new HotelManagerController(_context);
        }

        [TestMethod]
        [Description("Get all managers.")]
        public void GetHotelManageriTest()
        {
            Assert.IsNotNull(_context.HotelManager, "No table HotelManager in DB.");
            Assert.IsNotNull(_HotelManagerController.GetHotelManageri().Value, "No managers in DB.");
            Assert.IsTrue(_context.HotelManager.Count() == _HotelManagerController.GetHotelManageri().Value.Count(), "Number of hotel managers differ on DB and API.");
        }

        [TestMethod]
        [Description("Get hotel manager.")]
        public void GetHotelManagerTest()
        {
            var id = Guid.Parse("68D777FD-2D74-41A2-A95D-8E2D974BF943");
            var hotelManager = _context.HotelManager.Find(id);
            Assert.IsNotNull(hotelManager, "Null. No manager by Id in DB.");
            var rezultat = _HotelManagerController.GetHotelManager(id).Value;
            Assert.AreEqual(hotelManager, rezultat, "Manager by Id differ on DB and API.");
        }

        [TestMethod]
        [Description("Get unexisting mangaer.")]
        public void GetNepostojeciHotelManagerTest()
        {
            var id = Guid.Parse("68D777FD-2D74-41A2-A95D-11111111F943");
            Assert.IsNull(_context.HotelManager.Find(id), "There is unexisting manager in DB by Id.");
            Assert.IsNull(_HotelManagerController.GetHotelManager(id).Value, "API gets unexsisting manager by Id.");
        }

        [TestMethod]
        [Description("Insert new manager.")]
        public void PostHotelManagerGuest()
        {
            HotelManager hotelManager = new HotelManager();
            hotelManager.Name = "Erik";
            hotelManager.Surname = "Erikson";
            hotelManager.PIN = "12345678901";

            int HotelManagerBefore = _context.HotelManager.Count();
            _HotelManagerController.PostHotelManager(hotelManager);
            int HotelManagerAfter = _context.HotelManager.Count();
            Assert.IsTrue(HotelManagerBefore < HotelManagerAfter, "Number of hotel managers differ.");
        }

        [TestMethod]
        [Description("Insert existing manager.")]
        public void PostExistingHotelManager()
        {
            HotelManager hotelManager = new HotelManager();
            hotelManager.Id = Guid.Parse("68D777FD-2D74-41A2-A95D-8E2D974BF943");
            hotelManager.Name = "Erik";
            hotelManager.Surname = "Erikson";
            hotelManager.PIN = "12345678901";

            int hotelManagerBefore = _context.HotelManager.Count();

            var rezultat = _HotelManagerController.PostHotelManager(hotelManager).Value;

            Assert.IsNull(rezultat, "It should be null.");

            int HotelManagerAfter = _context.HotelManager.Count();
            Assert.AreEqual(hotelManagerBefore, HotelManagerAfter, "Number of managers differ, after insert.");
        }

        [TestMethod]
        [Description("Update hotel manager.")]
        public void PutHotelManagerTest()
        {
            var id = Guid.Parse("C9AAF44B-643C-4D4A-B4E8-98061F180441");
            HotelManager hotelManager = new HotelManager();

            hotelManager.Id = id;
            hotelManager.Name = "Erna";
            hotelManager.Surname = "Solberg";
            hotelManager.PIN = "12345432345";

            _HotelManagerController.PutHotelManager(hotelManager);

            Assert.AreEqual(hotelManager.Name, hotelManager.Name, "The name is not equal.");
            Assert.AreEqual(hotelManager.Surname, hotelManager.Surname, "The suraname is not equal.");
        }

        [TestMethod]
        [Description("Update unexisting manager")]
        public void PutUnexistingHotelManagerTest()
        {
            var id = Guid.Parse("C9AAF44B-643C-4D4A-6666-98061F180441");
            HotelManager hotelManager = new HotelManager();

            hotelManager.Id = id;
            hotelManager.Name = "Marge";
            hotelManager.Surname = "Pederson";
            hotelManager.PIN = "12345432345";

            _HotelManagerController.PutHotelManager(hotelManager);
            Assert.IsNull(_context.HotelManager.Find(id), "Unexisting manager is found.");
        }

        [TestMethod]
        [Description("Delete manager.")]
        public void DeleteHotelManagerTest()
        {
            var id = Guid.Parse("C9AAF44B-643C-4D4A-B4E8-98061F180441");
            int hotelManagerBefore = _context.HotelManager.Count();
            var hotelManager = _context.HotelManager.Find(id);

            _HotelManagerController.DeleteHotelManager(id);

            int hotelManagerAfter = _context.HotelManager.Count();
            Assert.IsTrue(hotelManagerBefore > hotelManagerAfter, "Number of managers after delete is the same.");

            _context.Add(hotelManager);
            _context.SaveChanges();
        }

        [TestMethod]
        [Description("Delete unexisting manager.")]
        public void DeletUnexistingHotelManagerTest()
        {
            var id = Guid.Parse("C9AAF44B-643C-4D4A-B4E8-111111111441");
            Assert.IsNull(_context.HotelManager.Find(id), "Gets the unexisting manager from DB.");
        }
    }
}
