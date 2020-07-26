using HotelAPI.Controllers;
using HotelAPI.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace HotelAPITests
{
    [TestClass]
    public class ServiceTests
    {
        private HotelsContext _context;
        private ServiceController _ServiceController;

        [TestInitialize]
        public void TestInit()
        {
            _context = Utility.GetDbContext();
            _ServiceController = new ServiceController(_context);
        }

        [TestMethod]
        [Description("Get all hotel services.")]
        public void GetServicesTest()
        {
            int inTotal = _context.Service.Count();
            int inTotalDohvat = _ServiceController.GetServices().Value.Count();
            Assert.AreEqual(inTotal, inTotalDohvat, "Number of services is not equal.");
        }

        [TestMethod]
        [Description("Get one service.")]
        public void GetServiceTest()
        {
            var id = (Guid.Parse("2DE8380F-A6DC-4525-9E73-9A42DD2BB45E"));
            var service = _ServiceController.GetService(id).Value;
            Assert.AreEqual(_context.Service.Find(id), service, "No services by API.");
        }

        [TestMethod]
        [Description("Get unesisting service.")]
        public void GetNepostojecaServiceTest()
        {
            var id = (Guid.Parse("2DE8380F-1111-4525-9E73-9A42DD2BB45E"));
            Service service = _ServiceController.GetService(id).Value;
            Assert.IsNull(service, "API gets unexisting service.");
        }

        [TestMethod]
        [Description("Insert one service.")]
        public void PostServiceTest()
        {
            Service service = new Service();
            service.Name = "Telling good night stories";
            service.PricePerItem = 2500.99m;
            service.PIN = "12345678901";
            int serviceBefore = _context.Service.Count();
            _ServiceController.PostService(service);
            int serviceAfter = _context.Service.Count();

            Assert.IsTrue(serviceAfter > serviceBefore, "Number of service is equal after insert.");
        }

        [TestMethod]
        [Description("Update one service.")]
        public void PutServiceTest()
        {
            Service service = new Service();
            service.Id = (Guid.Parse("2DE8380F-A6DC-4525-9E73-9A42DD2BB45E"));
            service.Name = "Telling good night stories";
            service.PricePerItem = 3000.99m;
            service.PIN = "12345678901";

            _ServiceController.PutService(service);
            var serviceAfter = _context.Service.Find(service.Id);
            Assert.IsNotNull(serviceAfter, "No service by Id.");
            Assert.AreEqual(service.Id, serviceAfter.Id, "Service differ by Id in DB and API.");
            Assert.AreEqual(service.Name, serviceAfter.Name, "Service differ by Name in DB and API.");
            Assert.AreEqual(service.PricePerItem, serviceAfter.PricePerItem, "Service differ by Price per item in DB and API.");
        }

        [TestMethod]
        [Description("Delete service.")]
        public void DeleteServiceTest()
        {
            var id = (Guid.Parse("2DE8380F-A6DC-4525-9E73-9A42DD2BB45E"));
            
            Service service = _context.Service.Find(id);
            Assert.IsNotNull(service, "There is no service by Id in DB.");
            _ServiceController.DeleteService(id);
            Assert.IsNull(_context.Service.Find(id), "No service by ID after delete.");

            _context.Add(service);
            _context.SaveChanges();
        }

        [TestMethod]
        [Description("Delete unexisting service.")]
        public void DeleteNepostojecaServiceTest()
        {
            var id = (Guid.Parse("2DE8380F-1111-4525-9E73-9A42DD2BB45E"));

            Service service = _ServiceController.DeleteService(id).Value;
            Assert.IsNull(service, "Unexisting service is still in DB, after delete.");            
        }
    }
}
