using HoteliAPI.Controllers;
using HoteliAPI.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace HoteliAPITests
{
    [TestClass]
    public class SobaTests
    {
        private HoteliContext _context;
        private SobaController _sobaController;

        [TestInitialize]
        public void TestInit()
        {
            _context = Utility.GetDbContext();
            _sobaController = new SobaController(_context);
        }

        [TestMethod]
        [Description("Dohvat svih soba")]
        public void GetSobeTest()
        {
            Assert.IsNotNull(_context.Soba, "Vraća null. U bazi nema Soba.");
            Assert.IsNotNull(_sobaController.GetSobe().Value, "Vraća null. Metoda ne dohvaća Sobe.");
            Assert.IsTrue(_context.Soba.Count() == _sobaController.GetSobe().Value.Count(), "Broj soba u bazi i vraćenih po metodi nije jednak.");
        }

        [TestMethod]
        [Description("Dohvat jedne sobe")]
        public void GetSobaTest()
        {
            var id = Guid.Parse("222B3F14-A1D0-4B27-82D9-9208CE4FB917");
            Soba soba = _context.Soba.Find(id);
            Soba rezultat = _sobaController.GetSoba(id).Value;
            Assert.AreEqual(soba, rezultat, "Dohvaćena soba po ID-u nije ona koja je dohvaćena po metodi.");
        }

        [TestMethod]
        [Description("Dohvat nepostojeće sobe")]
        public void GetNepostojecaSobaTest()
        {
            var id = Guid.Parse("B34DE1EA-1111-4E4A-9790-DA176B35B5EF");
            Assert.IsNull(_context.Soba.Find(id), "Ne vraća null. Pronalazi nepostojeću sobu na bazu.");
            Assert.IsNull(_sobaController.GetSoba(id).Value, "Ne vraća null. Metoda pronalazi nepostojeću sobu.");
        }

        [TestMethod]
        [Description("Unos nove sobe")]
        public void PostSobaTest()
        {
            Soba soba = new Soba();
            soba.BrojSobe = 88;
            soba.CijenaPoNocenju = 2880m;

            int sobaBefore = _context.Soba.Count();
            _sobaController.PostSoba(soba);
            int sobaAfter = _context.Soba.Count();
            Assert.IsTrue(sobaBefore < sobaAfter, "Broj soba prije i nakon unosa je jednak");
        }

        [TestMethod]
        [Description("Unos iste sobe")]
        public void PostIsteSobaTest()
        {
            var id = Guid.Parse("222B3F14-A1D0-4B27-82D9-9208CE4FB917");
            Soba soba = _context.Soba.Find(id);
            Assert.IsNotNull(soba, "Vraća null. Ne pronalazi sobu na bazi.");
            Soba rezultat = _sobaController.PostSoba(soba).Value;
            Assert.IsNull(rezultat, "Ne vraća null. Pronalazi nepostojeću sobu na bazi.");
        }

        [TestMethod]
        [Description("Izmjena podataka o sobi")]
        public void PutSobaTest()
        {
            var id = Guid.Parse("2A04CDDC-5041-4D64-B802-56907BFE13C1");
            Soba soba = new Soba();

            soba.Id = id;
            soba.BrojSobe = 55;
            soba.CijenaPoNocenju = 1005m;

            _sobaController.PutSoba(soba);
            Soba sobaAfter = _context.Soba.Find(id);
            Assert.AreEqual(soba.BrojSobe, sobaAfter.BrojSobe, "soba.BrojSobe != sobaAfter.BrojSobe.");
            Assert.AreEqual(soba.CijenaPoNocenju, sobaAfter.CijenaPoNocenju, "Cijene po noćenju prije i kasnije nisu iste.");
        }


        [TestMethod]
        [Description("Izmjena podataka o nepostojećoj sobi")]
        public void PutNepostojecaSobaTest()
        {
            var id = Guid.Parse("B34DE1EA-1111-4E4A-9790-DA176B35B5EF");
            Soba soba = new Soba();

            soba.Id = id;
            soba.BrojSobe = 55;
            soba.CijenaPoNocenju = 1005m;

            _sobaController.PutSoba(soba);
            Assert.IsNull(_context.Soba.Find(id), "Ne vraća null. Pronalazi nepostojeću sobu na bazi.");
        }

        [TestMethod]
        [Description("Brisanje podataka o sobi")]
        public void DeleteSobaTest()
        {
            var id = Guid.Parse("10B9B4E2-085D-4F78-8A07-97B47F44179E");

            Soba soba = _context.Soba.Find(id);
            Assert.IsNotNull(soba, "Vraća null. Ne pronalazi sobu na bazi.");

            _sobaController.DeleteSoba(id);
            Assert.IsNull(_context.Soba.Find(id), "Ne vraća null. Pronalazi obirsanu sobu u bazi.");

            _context.Add(soba);
            _context.SaveChanges();
            Assert.IsNotNull(soba, "Vraća null. Nakon spremanja na bazu, ne pronalazi sobu.");
        }

        [TestMethod]
        [Description("Brisanje podataka o nepostojećoj sobi")]
        public void DeleteNepostojecaSobaTest()
        {
            var id = Guid.Parse("222B3F14-1111-4B27-82D9-9208CE4FB917");
            Assert.IsNull(_sobaController.DeleteSoba(id).Value, "Ne vraća null. Pronalazi sobu u bazi.");
        }
    }
}
