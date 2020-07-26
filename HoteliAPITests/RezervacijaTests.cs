using HoteliAPI.Controllers;
using HoteliAPI.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;


namespace HoteliAPITests
{
    [TestClass]
    public class RezervacijaTests
    {
        private HoteliContext _context;
        private RezervacijaController _rezervacijaController;

        [TestInitialize]
        public void TestInit()
        {
            _context = Utility.GetDbContext();
            _rezervacijaController = new RezervacijaController(_context);
        }

        [TestMethod]
        [Description("Dohvat svih rezervacija")]
        public void GetRezervacijeTest()
        {
            int brojRezervacija = _context.Rezervacija.Count();
            int dohvaceno = _rezervacijaController.GetRezervacije().Value.Count();
            Assert.AreEqual(brojRezervacija, dohvaceno, "Broj dohvaćenih rezervacija i onih na bazi nije jednak.");
        }

        [TestMethod]
        [Description("Dohvat jedne rezervacije")]
        public void GetRezervacijaTest()
        {
            var id = Guid.Parse("FC84634E-A97C-49A6-8D7D-6D151142ACFD");
            Assert.IsNotNull(_context.Rezervacija.Find(id), "Ne pronalazi rezervaciju na bazi po ID-u.");

            var rezervacija = _rezervacijaController.GetRezervacija(id).Value;
            Assert.IsNotNull(rezervacija, "Ne pronalazi rezervaciju na bazi po ID-u.");
            Assert.AreEqual(rezervacija.Id, id, "ID nije jednak kao ID rezervacije na bazi.");
        }

        [TestMethod]
        [Description("Unos nove rezervacije")]
        public void PostRezervacijaTest()
        {
            Rezervacija rezervacija = new Rezervacija()
            {
                GostId = Guid.Parse("58D8BFA5-7CF8-4F3F-B03D-0DE87DD9A48A"),
                VoditeljId = Guid.Parse("AF959367-F535-433C-82A8-6236CFA80FDB"),
                SobaId = Guid.Parse("48CC484C-90B6-49C0-A761-9EE936AF5C0D"),
                Sifra = "12345678901",
                DatumOd = DateTime.Now.AddDays(-2),
                DatumDo = DateTime.Now.AddDays(5)
            };

            _rezervacijaController.PostRezervacija(rezervacija);

            Rezervacija rezultat = _context.Rezervacija.FirstOrDefault(x => x.Sifra == rezervacija.Sifra);
            Assert.IsNotNull(rezultat, "Na bazi nije pronađena rezervacija po danoj šifri.");
            Assert.IsTrue(rezultat.Aktivna == 1, "Rezervacija nije aktivna.");
            Racun racun = _context.Racun.FirstOrDefault(x => x.RezervacijaId == rezultat.Id);
            Assert.IsNotNull(racun, "Nije pronađen račun po ID-u rezervacije.");

            _context.Racun.Remove(racun);
            _context.Rezervacija.Remove(rezultat);
            _context.SaveChanges();
        }

        [TestMethod]
        [Description("Unos postojeće rezervacije")]
        public void PostPostojecaRezervacijaTest()
        {
            Rezervacija rezervacija = new Rezervacija()
            {
                GostId = Guid.Parse("58D8BFA5-7CF8-4F3F-B03D-0DE87DD9A48A"),
                VoditeljId = Guid.Parse("AF959367-F535-433C-82A8-6236CFA80FDB"),
                SobaId = Guid.Parse("48CC484C-90B6-49C0-A761-9EE936AF5C0D"),
                Sifra = "55555566666",
                DatumOd = DateTime.Now.AddDays(7),
                DatumDo = DateTime.Now.AddDays(9)
            };

            int rezervacijaBefore = _context.Rezervacija.Count();
            _rezervacijaController.PostRezervacija(rezervacija);
            int rezervacijaAfter = _context.Rezervacija.Count();
            Assert.AreEqual(rezervacijaBefore, rezervacijaAfter, "Nakon pokušaja unosa postojeće rezervaije, broj rezervacija na bazi nije jednak.");
            Assert.IsNotNull(_context.Rezervacija.FirstOrDefault(x => x.Sifra == rezervacija.Sifra), "Ne pronalazi na bazi rezervaciju po danoj šifri.");
        }

        [TestMethod]
        [Description("Izmjena postojeće rezervacije")]
        public void PutRezervacijaTest()
        {
            Rezervacija rezervacija = new Rezervacija()
            {
                Id = Guid.Parse("FC84634E-A97C-49A6-8D7D-6D151142ACFD"),
                DatumOd = DateTime.Now.AddDays(7),
                DatumDo = DateTime.Now.AddDays(11),
                Aktivna = 1,
                Sifra = "55555566666",
                GostId = Guid.Parse("58D8BFA5-7CF8-4F3F-B03D-0DE87DD9A48A"),
                VoditeljId = Guid.Parse("AF959367-F535-433C-82A8-6236CFA80FDB"),
                SobaId = Guid.Parse("48CC484C-90B6-49C0-A761-9EE936AF5C0D")
            };

            var rezervacijaDB = _context.Rezervacija.FirstOrDefault(x => x.Sifra == rezervacija.Sifra);
            Assert.IsNotNull(rezervacijaDB, "Ne pronalazi na bazi rezervaciju po danoj šifri.");

            _rezervacijaController.PutRezervacija(rezervacija);

            Assert.AreEqual(rezervacija.Id, rezervacijaDB.Id, "ID nije jednak sa ID-om u bazi.");
            Assert.AreEqual(rezervacija.DatumOd, rezervacijaDB.DatumOd, "DatumOd nije jednak sa DatumOd u bazi.");
            Assert.AreEqual(rezervacija.DatumDo, rezervacijaDB.DatumDo, "DatumDo nije jednak sa DatumDo u bazi.");
            Assert.AreEqual(rezervacija.Aktivna, rezervacijaDB.Aktivna, "Aktivna nije jednak sa Aktivna u bazi.");
            Assert.AreEqual(rezervacija.GostId, rezervacijaDB.GostId, "GostId nije jednak sa GostId-om u bazi.");
            Assert.AreEqual(rezervacija.VoditeljId, rezervacijaDB.VoditeljId, "VoditeljId nije jednak sa VoditeljId-om u bazi.");
            Assert.AreEqual(rezervacija.SobaId, rezervacijaDB.SobaId, "SobaId nije jednak sa SobaId-om u bazi.");
        }


        [TestMethod]
        [Description("Izmjena NEpostojeće rezervacije")]
        public void PutNepostojecaRezervacijaTest()
        {
            Rezervacija rezervacija = new Rezervacija()
            {
                Id = Guid.Parse("FC84634E-1111-1111-8D7D-6D151142ACFD"),
                DatumOd = DateTime.Now.AddDays(7),
                DatumDo = DateTime.Now.AddDays(11),
                Aktivna = 1,
                Sifra = "55555566666",
                GostId = Guid.Parse("58D8BFA5-7CF8-4F3F-B03D-0DE87DD9A48A"),
                VoditeljId = Guid.Parse("AF959367-F535-433C-82A8-6236CFA80FDB"),
                SobaId = Guid.Parse("48CC484C-90B6-49C0-A761-9EE936AF5C0D")
            };

            _rezervacijaController.PutRezervacija(rezervacija);
            Assert.IsNull(_context.Rezervacija.Find(rezervacija.Id), "Ne pronalazi na bazi rezervaciju po ID-u.");
        }

        [TestMethod]
        [Description("Brisanje postojeće rezervacije")]
        public void DeleteRezervacijaTest()
        {
            var id = Guid.Parse("C3366C16-6755-4814-9F62-8A24408891EB");

            Rezervacija rez = _context.Rezervacija.Find(id);

            Assert.IsNotNull(rez, "Ne pronalazi na bazi rezervaciju po ID-u.");

            _rezervacijaController.DeleteRezervacija(id);

            Assert.IsNull(_context.Rezervacija.Find(id), "Pronalazi na bazi rezervaciju koju smo obrisali.");

            _context.Rezervacija.Add(rez);
            _context.SaveChanges();

            Assert.IsNotNull(_context.Rezervacija.Find(id), "Ne pronalazi rezervaciju po ID-u nakon što je vraćena na bazu.");
        }

        [TestMethod]
        [Description("Brisanje NEpostojeće rezervacije")]
        public void DeleteNepostojeceRezervacijaTest()
        {
            var id = Guid.Parse("C3366C16-1111-1111-1111-8A24408891EB");

            Assert.IsNull(_context.Rezervacija.Find(id), "Pronalazi na bazi nepostojeću rezervaciju po ID-u.");

            _rezervacijaController.DeleteRezervacija(id);

            Assert.IsNull(_context.Rezervacija.Find(id), "Nakon brisanja pronalazi na bazi nepostojeću rezervaciju po ID-u.");
        }
    }
}
