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
    public class StavkaRacunaTests
    {
        private HoteliContext _context;
        private StavkaRacunaController _stavkaRacunaController;

        [TestInitialize]
        public void TestInit()
        {
            _context = Utility.GetDbContext();
            _stavkaRacunaController = new StavkaRacunaController(_context);
        }

        [TestMethod]
        [Description("Dohvat stavaka po Id-u računa")]
        public void GetStavkeRacunaTest()
        {
            var id = Guid.Parse("75A3489C-6321-4D5C-9061-1AABA0FC8BBA");
            Assert.IsNotNull(_context.Racun.Find(id), "Vraća null. Ne pronalazi stavke po IDu.");
            var rezultat = _stavkaRacunaController.GetStavkeRacuna(id).Value.ToList();
            Assert.IsNotNull(rezultat, "Vraća null. Metoda ne vraća stavke po IDu.");
            var rezultatDB = _context.StavkaRacuna.Where(x => x.RacunId == id).ToList();
            Assert.IsTrue(rezultat.Count() == rezultatDB.Count(), "Vraća false. Količin stavki u bazi i vraćenih nije jednaka.");
        }

        [TestMethod]
        [Description("Dohvat jedne stavke")]
        public void GetStavkaRacunaTest()
        {
            var id = Guid.Parse("75A3489C-6321-4D5C-9061-1AABA0FC8BBA");
            Assert.IsNotNull(_context.Racun.Find(id), "Vraća null. Ne pronalazi račun po IDu.");

            var idStavka = Guid.Parse("D3FAA09F-849A-4570-9159-FF939DB5B5C7");
            Assert.IsNotNull(_context.StavkaRacuna.Find(idStavka), "Vraća null. Ne pronalazi stavke po IDu.");

            var rezultat = _stavkaRacunaController.GetStavkaRacuna(id, idStavka).Value;
            Assert.IsNotNull(rezultat, "Vraća null. Metoda ne pronalazi stavku po ID-ima.");
        }


        [TestMethod]
        [Description("Unos nove stavke")]
        public void PostStavkaRacunaTest()
        {
            StavkaRacuna stavka = new StavkaRacuna()
            {
                Kolicina = 2,
                UkupnaCijena = 0,
                UslugaId = Guid.Parse("BCAD3EFE-2064-4D20-BDC3-5881A8AA1B2C"),
                RacunId = Guid.Parse("75A3489C-6321-4D5C-9061-1AABA0FC8BBA"),
                Sifra = "99888776655"
            };

            int stavkaBefore = _context.StavkaRacuna.Count();
            _stavkaRacunaController.PostStavkaRacuna(stavka);
            Assert.IsNotNull(_context.StavkaRacuna.FirstOrDefault(x => x.Sifra == stavka.Sifra), "Vraća null. Ne pronalazi stavke na bazi.");
            int stavkeAfter = _context.StavkaRacuna.Count();

            Assert.AreNotEqual(stavkaBefore, stavkeAfter, "Broj stavki prije i nakon unosa je jednak.");

            _context.StavkaRacuna.Remove(stavka);
            _context.SaveChanges();
        }

        [TestMethod]
        [Description("Unos postojeće stavke")]
        public void PostPostojecaStavkaRacunaTest()
        {
            StavkaRacuna stavka = new StavkaRacuna()
            {
                Kolicina = 2,
                UkupnaCijena = 0,
                UslugaId = Guid.Parse("BCAD3EFE-2064-4D20-BDC3-5881A8AA1B2C"),
                RacunId = Guid.Parse("75A3489C-6321-4D5C-9061-1AABA0FC8BBA"),
                Sifra = "11223344556"
            };

            int stavkaBefore = _context.StavkaRacuna.Count();
            _stavkaRacunaController.PostStavkaRacuna(stavka);
            int stavkeAfter = _context.StavkaRacuna.Count();

            Assert.AreEqual(stavkaBefore, stavkeAfter, "Broj stavki mora biti jednak.");
        }

        [TestMethod]
        [Description("Izmjena stavke")]
        public void PutStavkaTest()
        {
            StavkaRacuna stavka = new StavkaRacuna()
            {
                Id = Guid.Parse("D3FAA09F-849A-4570-9159-FF939DB5B5C7"),
                Kolicina = 8,
                UkupnaCijena = 0,
                UslugaId = Guid.Parse("BCAD3EFE-2064-4D20-BDC3-5881A8AA1B2C"),
                RacunId = Guid.Parse("75A3489C-6321-4D5C-9061-1AABA0FC8BBA"),
                Sifra = "11223344556"
            };

            var stavkaDBkolicina = _context.StavkaRacuna.Find(stavka.Id).Kolicina;
            var racunDBukupno = _context.Racun.Find(stavka.RacunId).Ukupno;
            Assert.IsNotNull(stavkaDBkolicina, "Vraća null. Nije pronađena količina.");
            Assert.IsNotNull(racunDBukupno, "Vraća null. Na računu nije pronađeno Ukupno.");

            Assert.AreNotEqual(stavka.Kolicina, stavkaDBkolicina, "Količina u stavci mora biti različita od količine stavke na bazi.");

            _stavkaRacunaController.PutStavkaRacuna(stavka);

            Assert.AreNotEqual(racunDBukupno, _context.Racun.Find(stavka.RacunId).Ukupno, "Ukupno na računu mora biti različita od ukupno na računu na bazi.");

            _context.StavkaRacuna.Find(stavka.Id).Kolicina = 5;
            _context.Racun.Find(stavka.RacunId).Ukupno = racunDBukupno;
            _context.SaveChanges();
        }

        [TestMethod]
        [Description("Brisanje stavke")]
        public void DeleteStavkaRacunaTest()
        {
            var id = Guid.Parse("53BEAF4B-F9A2-4C1C-88F3-F334F4261B79");
            var racunId = Guid.Parse("75A3489C-6321-4D5C-9061-1AABA0FC8BBA");
            var stavka = _context.StavkaRacuna.Find(id);

            int stavkaBefore = _context.StavkaRacuna.Count();

            _stavkaRacunaController.DeleteStavkaRacuna(racunId, id);

            Assert.IsNull(_context.StavkaRacuna.Find(id), "Stavka postoji na bazi.");

            int stavkeAfter = _context.StavkaRacuna.Count();

            Assert.AreNotEqual(stavkaBefore, stavkeAfter, "Broj stavki prije i nakon brisanja je jednak.");

            _context.StavkaRacuna.Add(stavka);
            _context.SaveChanges();
        }
    }
}
