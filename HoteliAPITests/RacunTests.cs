using HoteliAPI.Controllers;
using HoteliAPI.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HoteliAPITests
{
    [TestClass]
    public class RacunTests
    {
        private HoteliContext _context;
        private RacunController _racunController;

        [TestInitialize]
        public void TestInit()
        {
            _context = Utility.GetDbContext();
            _racunController = new RacunController(_context);
        }

        [TestMethod]
        [Description("Dohvati sve račune")]
        public void GetRacuniTest()
        {
            int ukupnoDB = _context.Racun.Count();
            int ukupno = _racunController.GetRacuni().Value.Count();
            Assert.IsTrue(ukupno == ukupnoDB, "Broj dohvaćenih računa i onih u bazi nije jednak.");
        }

        [TestMethod]
        [Description("Dohvat jednog računa")]
        public void GetRacunTest()
        {
            var id = Guid.Parse("75A3489C-6321-4D5C-9061-1AABA0FC8BBA");
            Racun racunDB = _context.Racun.Find(id);
            Assert.IsNotNull(racunDB, "Vraća null. Račun po tom ID-u nije pronađen u bazi.");

            Racun racun = _racunController.GetRacun(id).Value;
            Assert.IsNotNull(racun, "Vraća null. Račun nije dohvaćen iz baze.");

            Assert.AreEqual(racunDB.Id, racun.Id, "racunDB.Id != racun.Id");
        }

        [TestMethod]
        [Description("Izmjena postojećeg računa")]
        public void PutRacunTest()
        {
            var id = Guid.Parse("75A3489C-6321-4D5C-9061-1AABA0FC8BBA");
            Racun racun = _context.Racun.Find(id);
            Assert.IsNotNull(racun, "Vraća null. Račun nije dohvaćen iz baze.");

            decimal UkupnoDB = racun.Ukupno;

            racun.IznosPopusta = 0.18m;

            _racunController.PutRacun(racun);

            Assert.AreNotEqual(_context.Racun.Find(id).Ukupno, UkupnoDB, "Ukupno == UkupnoDB.");

            _context.Racun.Find(id).IznosPopusta = 0.9m;
            _context.Racun.Find(id).Ukupno = UkupnoDB;
            _context.SaveChanges();
        }
    }
}
