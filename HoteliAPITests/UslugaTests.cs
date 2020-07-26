using HoteliAPI.Controllers;
using HoteliAPI.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace HoteliAPITests
{
    [TestClass]
    public class UslugaTests
    {
        private HoteliContext _context;
        private UslugaController _uslugaController;

        [TestInitialize]
        public void TestInit()
        {
            _context = Utility.GetDbContext();
            _uslugaController = new UslugaController(_context);
        }

        [TestMethod]
        [Description("Dohvat svih usluga")]
        public void GetUslugeTest()
        {
            int ukupno = _context.Usluga.Count();
            int ukupnoDohvat = _uslugaController.GetUsluge().Value.Count();
            Assert.AreEqual(ukupno, ukupnoDohvat, "Broj dohvaćenih usluga preko metode i sa baze nije jednak.");
        }

        [TestMethod]
        [Description("Dohvat jedne usluge")]
        public void GetUslugaTest()
        {
            var id = (Guid.Parse("2DE8380F-A6DC-4525-9E73-9A42DD2BB45E"));
            var usluga = _uslugaController.GetUsluga(id).Value;
            Assert.AreEqual(_context.Usluga.Find(id), usluga, "Metoda ne dohvaća uslugu.");
        }

        [TestMethod]
        [Description("Dohvat NEpostojeće usluge")]
        public void GetNepostojecaUslugaTest()
        {
            var id = (Guid.Parse("2DE8380F-1111-4525-9E73-9A42DD2BB45E"));
            Usluga usluga = _uslugaController.GetUsluga(id).Value;
            Assert.IsNull(usluga, "Metoda dohvaća nepostojeću uslugu.");
        }

        [TestMethod]
        [Description("Unos jedne usluge")]
        public void PostUslugaTest()
        {
            Usluga usluga = new Usluga();
            usluga.Naziv = "Čitanje priče za laku noć";
            usluga.JedinicnaCijena = 2500.99m;
            usluga.Sifra = "12345678901";
            int uslugaBefore = _context.Usluga.Count();
            _uslugaController.PostUsluga(usluga);
            int uslugaAfter = _context.Usluga.Count();

            Assert.IsTrue(uslugaAfter > uslugaBefore, "Broj usluga prije i nakon unosa je jednak.");
        }

        [TestMethod]
        [Description("Izmjena jedne usluge")]
        public void PutUslugaTest()
        {
            Usluga usluga = new Usluga();
            usluga.Id = (Guid.Parse("2DE8380F-A6DC-4525-9E73-9A42DD2BB45E"));
            usluga.Naziv = "Čitanje priče za laku noć";
            usluga.JedinicnaCijena = 3000.99m;
            usluga.Sifra = "12345678901";

            _uslugaController.PutUsluga(usluga);
            var uslugaAfter = _context.Usluga.Find(usluga.Id);
            Assert.IsNotNull(uslugaAfter, "Nije pronađena usluga po Id-u");
            Assert.AreEqual(usluga.Id, uslugaAfter.Id, "Usluga na bazi se po Id-u ne podudara sa izmjenjenom uslugom.");
            Assert.AreEqual(usluga.Naziv, uslugaAfter.Naziv, "Usluga na bazi se po nazivu ne podudara sa izmjenjenom uslugom.");
            Assert.AreEqual(usluga.JedinicnaCijena, uslugaAfter.JedinicnaCijena, "Usluga na bazi se po jediničnoj cijeni ne podudara sa izmjenjenom uslugom.");
        }

        [TestMethod]
        [Description("Brisanje postojeće usluge")]
        public void DeleteUslugaTest()
        {
            var id = (Guid.Parse("2DE8380F-A6DC-4525-9E73-9A42DD2BB45E"));
            
            Usluga usluga = _context.Usluga.Find(id);
            Assert.IsNotNull(usluga, "Na bazi nije pronađena usluga po Id-u");
            _uslugaController.DeleteUsluga(id);
            Assert.IsNull(_context.Usluga.Find(id), "Nakon brisanja na bazi nije pronađena usluga po Id-u");

            _context.Add(usluga);
            _context.SaveChanges();
        }

        [TestMethod]
        [Description("Brisanje NEpostojeće usluge")]
        public void DeleteNepostojecaUslugaTest()
        {
            var id = (Guid.Parse("2DE8380F-1111-4525-9E73-9A42DD2BB45E"));

            Usluga usluga = _uslugaController.DeleteUsluga(id).Value;
            Assert.IsNull(usluga, "Pronalazi obrisanu uslugu.");            
        }
    }
}
