using HoteliAPI.Models;
using HoteliAPI.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using System.Linq;

namespace HoteliAPITests
{
    [TestClass]
    public class GostTests
    {
        private HoteliContext _context;
        private GostController _gostController;

        [TestInitialize]
        public void TestInit()
        {
            _context = Utility.GetDbContext();
            _gostController = new GostController(_context);
        }

        [TestMethod]
        [Description("Dohvat liste gostiju.")]
        public void GetGostiTest()
        {
            Assert.IsNotNull(_gostController.GetGosti().Value, "Metoda je vratila null. Na bazi nema gostiju.");
            Assert.IsTrue(_gostController.GetGosti().Value.Count() > 0, "U listi nema niti jednog podatka.");
        }

        [TestMethod]
        [Description("Dohvat jednog gosta po ID-u.")]
        public void GetGostTest()
        {
            var gost = _gostController.GetGost(Guid.Parse("E3AD5D92-B30D-4A61-AEA9-90C601D8C9E3")).Value;

            Assert.IsNotNull(gost, "Metoda je vratila null.");
            Assert.IsInstanceOfType(gost, typeof(Gost), "Objekt nije tipa Gost.");
            Assert.AreEqual(gost.Id, Guid.Parse("E3AD5D92-B30D-4A61-AEA9-90C601D8C9E3"), "Id-i nisu isti.");
        }

        [TestMethod]
        [Description("Unos jednog gosta.")]
        public void PostGostTest()
        {
            Gost gost = new Gost();
            gost.Ime = "Maja";
            gost.Prezime = "Majic";

            Random random = new Random();
            StringBuilder output = new StringBuilder();

            for (int i = 0; i < 11; i++)
            {
                output.Append(random.Next(0, 9));
            }
            gost.Oib = output.ToString();

            var gostiBefore = _context.Gost.Count();
            _gostController.PostGost(gost);
            var gostiAfter = _context.Gost.Count();

            Assert.IsTrue(gostiAfter > gostiBefore, "Objekt gostAfter nije veći od gostiBefore.");
        }

        [TestMethod]
        [Description("Slučaj kada postoji gost sa istim ID-om u bazi.")]
        public void PostPostojeciGost()
        {
            Gost gost = new Gost();
            gost.Id = Guid.Parse("BA3279A7-AB43-41EC-9867-6F7B7E646C0E");
            gost.Ime = "Maja";
            gost.Prezime = "Majic";

            Random random = new Random();
            StringBuilder output = new StringBuilder();

            for (int i = 0; i < 11; i++)
            {
                output.Append(random.Next(0, 9));
            }
            gost.Oib = output.ToString();

            var gostiBefore = _context.Gost.Count();
            _gostController.PostGost(gost);
            var gostiAfter = _context.Gost.Count();

            Assert.IsTrue(gostiAfter == gostiBefore, "Objekt gostiAfter nije jednak gostiBefore.");
        }

        [TestMethod]
        [Description("Izmjena podatka o postojećem gostu.")]
        public void PutGostTest()
        {
            Gost gost = new Gost();
            gost.Id = Guid.Parse("BA3279A7-AB43-41EC-9867-6F7B7E646C0E");
            gost.Ime = "Ivana";
            gost.Prezime = "Ivic";

            _gostController.PutGost(gost);

            var gostAfter = _context.Gost.Find(gost.Id);

            Assert.IsNotNull(gostAfter, "Metoda je vratila null.");
            Assert.AreEqual(gost.Ime, gostAfter.Ime, "gost.Ime i gostAfter.ime nisu isti.");
            Assert.AreEqual(gost.Prezime, gostAfter.Prezime, "gost.Prezime i gostAfter.Prezime nisu isti.");
        }

        [TestMethod]
        [Description("Izmjena podataka o NEpostojećem gostu.")]
        public void PutNepostojeciGostTest()
        {
            Gost gost = new Gost();
            gost.Id = Guid.Parse("BA3279A7-AB43-41EC-9867-6F7B7E646C1E");
            gost.Ime = "Mali";
            gost.Prezime = "Miš";

            _gostController.PutGost(gost);

            var gostAfter = _context.Gost.Find(gost.Id);

            Assert.IsNull(gostAfter, "Metoda nije vratila null.");
        }

        [TestMethod]
        [Description("Brisanje postojećeg gosta")]
        public void DeleteGostTest()
        {
            var id = Guid.Parse("0D4A6A72-1E8E-44B6-8E40-3E68D924F107");

            Assert.IsNotNull(_context.Gost.Find(id), "Metoda je vratila null. Ne pronalazi gosta.");

            var gost = _context.Gost.Find(id);

            var obrisaniGost = _gostController.DeleteGost(id).Value;

            Assert.IsNotNull(obrisaniGost, "Metoda je vratila null. Ne pronalazi obrisanog gosta.");

            Assert.IsNull(_context.Gost.Find(id), "Metoda nije vratila null. Pronalazi gosta na bazi.");

            _context.Add(gost);
            _context.SaveChanges();
        }

        [TestMethod]
        [Description("Brisanje NEpostojećeg gosta")]
        public void DeleteNepostojeciGost()
        {
            var id = Guid.Parse("0D4A6A72-1E8E-44B6-8E40-55555524F107");

            Assert.IsNull(_context.Gost.Find(id), "Metoda nije vratila null. Pronalazi gosta na bazi.");

            var obrisaniGost = _gostController.DeleteGost(id).Value;

            Assert.IsNull(obrisaniGost, "Metoda nije vratila null.");
            Assert.IsNull(_context.Gost.Find(id), "Metoda nije vratila null.");
        }
    }
}
