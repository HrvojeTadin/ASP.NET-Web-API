using HoteliAPI.Controllers;
using HoteliAPI.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace HoteliAPITests
{
    [TestClass]
    public class VoditeljTests
    {
        private HoteliContext _context;
        private VoditeljController _voditeljController;

        [TestInitialize]
        public void TestInit()
        {
            _context = Utility.GetDbContext();
            _voditeljController = new VoditeljController(_context);
        }

        [TestMethod]
        [Description("Dohvat svih voditelja")]
        public void GetVoditeljiTest()
        {
            Assert.IsNotNull(_context.Voditelj, "Na bazi nema voditelja.");
            Assert.IsNotNull(_voditeljController.GetVoditelji().Value, "Metoda nije dohvatila niti jednog voditelja.");
            Assert.IsTrue(_context.Voditelj.Count() == _voditeljController.GetVoditelji().Value.Count(), "Broj voditelja na bazi i dohvaćenih nije jednak.");
        }

        [TestMethod]
        [Description("Dohvat postojećeg voditelja")]
        public void GetVoditeljTest()
        {
            var id = Guid.Parse("68D777FD-2D74-41A2-A95D-8E2D974BF943");
            var voditelj = _context.Voditelj.Find(id);
            Assert.IsNotNull(voditelj, "Vraća null. Voditelj na bazi nije pronađen.");
            var rezultat = _voditeljController.GetVoditelj(id).Value;
            Assert.AreEqual(voditelj, rezultat, "Dohvaćen voditelj po metodi i onaj u bazi nisu jednaki.");
        }

        [TestMethod]
        [Description("Dohvat Nepostojećeg voditelja")]
        public void GetNepostojeciVoditeljTest()
        {
            var id = Guid.Parse("68D777FD-2D74-41A2-A95D-11111111F943");
            Assert.IsNull(_context.Voditelj.Find(id), "Pronalazi na bazi nepostojećeg voditelja.");
            Assert.IsNull(_voditeljController.GetVoditelj(id).Value, " Metoda vraća nepostojećeg voditelja.");
        }

        [TestMethod]
        [Description("Unos novog voditelja")]
        public void PostVoditeljGost()
        {
            Voditelj voditelj = new Voditelj();
            voditelj.Ime = "Crni";
            voditelj.Prezime = "Labud";
            voditelj.Sifra = "12345678901";

            int voditeljBefore = _context.Voditelj.Count();
            _voditeljController.PostVoditelj(voditelj);
            int voditeljAfter = _context.Voditelj.Count();
            Assert.IsTrue(voditeljBefore < voditeljAfter, "Broj voditelja na bazi je jednak.");
        }

        [TestMethod]
        [Description("Unos istog voditelja")]
        public void PostIstiVoditeljGost()
        {
            Voditelj voditelj = new Voditelj();
            voditelj.Id = Guid.Parse("68D777FD-2D74-41A2-A95D-8E2D974BF943");
            voditelj.Ime = "Crni";
            voditelj.Prezime = "Labud";
            voditelj.Sifra = "12345678901";

            int voditeljBefore = _context.Voditelj.Count();

            var rezultat = _voditeljController.PostVoditelj(voditelj).Value;

            Assert.IsNull(rezultat, "Metoda ne vraća null.");

            int voditeljAfter = _context.Voditelj.Count();
            Assert.AreEqual(voditeljBefore, voditeljAfter, "Broj voditelja prije i nakon izmjene nije isti.");
        }

        [TestMethod]
        [Description("Izmjena podataka o postojećem gostu")]
        public void PutVoditeljTest()
        {
            var id = Guid.Parse("C9AAF44B-643C-4D4A-B4E8-98061F180441");
            Voditelj voditelj = new Voditelj();

            voditelj.Id = id;
            voditelj.Ime = "Bijeli";
            voditelj.Prezime = "Labud";
            voditelj.Sifra = "12345432345";

            _voditeljController.PutVoditelj(voditelj);

            var voditeljAfter = _context.Voditelj.Find(id);
            Assert.AreEqual(voditelj.Ime, voditelj.Ime, "Nakon izmjene ime je različito.");
            Assert.AreEqual(voditelj.Prezime, voditelj.Prezime, "Nakon izmjene prezime je različito.");
        }

        [TestMethod]
        [Description("Izmjena podataka o NEpostojećem gostu")]
        public void PutNepostojeciVoditeljTest()
        {
            var id = Guid.Parse("C9AAF44B-643C-4D4A-6666-98061F180441");
            Voditelj voditelj = new Voditelj();

            voditelj.Id = id;
            voditelj.Ime = "Bijeli";
            voditelj.Prezime = "Tigar";
            voditelj.Sifra = "12345432345";

            _voditeljController.PutVoditelj(voditelj);
            Assert.IsNull(_context.Voditelj.Find(id), "Pronalazak nepostojećeg gosta.");
        }

        [TestMethod]
        [Description("Brisanje postojećeg voditelja")]
        public void DeleteVoditeljTest()
        {
            var id = Guid.Parse("C9AAF44B-643C-4D4A-B4E8-98061F180441");
            int voditeljBefore = _context.Voditelj.Count();
            var voditelj = _context.Voditelj.Find(id);

            _voditeljController.DeleteVoditelj(id);

            int voditeljAfter = _context.Voditelj.Count();
            Assert.IsTrue(voditeljBefore > voditeljAfter, "Broj voditelja prije i nakon brisanja je isti.");

            _context.Add(voditelj);
            _context.SaveChanges();
        }

        [TestMethod]
        [Description("Brisanje NEpostojećeg voditelja")]
        public void DeleteNepostojeciVoditeljTest()
        {
            var id = Guid.Parse("C9AAF44B-643C-4D4A-B4E8-111111111441");
            Assert.IsNull(_context.Voditelj.Find(id), "Vraća nepostojećeg voditlja sa baze.");
        }
    }
}
