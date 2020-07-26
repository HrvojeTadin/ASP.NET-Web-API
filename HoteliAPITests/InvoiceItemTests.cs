using HotelAPI.Controllers;
using HotelAPI.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HotelAPITests
{
    [TestClass]
    public class InvoiceItemTests
    {
        private HotelsContext _context;
        private InvoiceItemController _InvoiceItemController;

        [TestInitialize]
        public void TestInit()
        {
            _context = Utility.GetDbContext();
            _InvoiceItemController = new InvoiceItemController(_context);
        }

        [TestMethod]
        [Description("Get invoice items by inovice Id.")]
        public void GetInvoiceItemsTest()
        {
            var id = Guid.Parse("75A3489C-6321-4D5C-9061-1AABA0FC8BBA");
            Assert.IsNotNull(_context.Invoice.Find(id), "Null. No invoice by Id.");
            var result = _InvoiceItemController.GetInvoiceItems(id).Value.ToList();
            Assert.IsNotNull(result, "Null. No items by invoice Id.");
            var resultDB = _context.InvoiceItem.Where(x => x.InvoiceId == id).ToList();
            Assert.IsTrue(result.Count() == resultDB.Count(), "False. Count of items is not equal.");
        }

        [TestMethod]
        [Description("Get one invoice item.")]
        public void GetInvoiceItemTest()
        {
            var id = Guid.Parse("75A3489C-6321-4D5C-9061-1AABA0FC8BBA");
            Assert.IsNotNull(_context.Invoice.Find(id), "Null. No inovoice by Id.");

            var idItem = Guid.Parse("D3FAA09F-849A-4570-9159-FF939DB5B5C7");
            Assert.IsNotNull(_context.InvoiceItem.Find(idItem), "Null. No invoice items by invoice Id.");

            var result = _InvoiceItemController.GetInvoiceItem(id, idItem).Value;
            Assert.IsNotNull(result, "Null. No inovice item by Id.");
        }

        [TestMethod]
        [Description("Insert new invoice item.")]
        public void PostInvoiceItemTest()
        {
            InvoiceItem item = new InvoiceItem()
            {
                Amount = 2,
                TotalPrice = 0,
                ServiceId = Guid.Parse("BCAD3EFE-2064-4D20-BDC3-5881A8AA1B2C"),
                InvoiceId = Guid.Parse("75A3489C-6321-4D5C-9061-1AABA0FC8BBA"),
                PIN = "99888776655"
            };

            int itemBefore = _context.InvoiceItem.Count();
            _InvoiceItemController.PostInvoiceItem(item);
            Assert.IsNotNull(_context.InvoiceItem.FirstOrDefault(x => x.PIN == item.PIN), "Null. No items in DB.");
            int itemsAfter = _context.InvoiceItem.Count();

            Assert.AreNotEqual(itemBefore, itemsAfter, "Number of inovice item is equal before and after insert.");

            _context.InvoiceItem.Remove(item);
            _context.SaveChanges();
        }

        [TestMethod]
        [Description("Insert existing invoice item.")]
        public void PostExisitingInvoiceItemTest()
        {
            InvoiceItem item = new InvoiceItem()
            {
                Amount = 2,
                TotalPrice = 0,
                ServiceId = Guid.Parse("BCAD3EFE-2064-4D20-BDC3-5881A8AA1B2C"),
                InvoiceId = Guid.Parse("75A3489C-6321-4D5C-9061-1AABA0FC8BBA"),
                PIN = "11223344556"
            };

            int itemsBefore = _context.InvoiceItem.Count();
            _InvoiceItemController.PostInvoiceItem(item);
            int itemsAfter = _context.InvoiceItem.Count();

            Assert.AreEqual(itemsBefore, itemsAfter, "Number of items must be equal before and after insert.");
        }

        [TestMethod]
        [Description("Update invoice item.")]
        public void PutitemTest()
        {
            InvoiceItem item = new InvoiceItem()
            {
                Id = Guid.Parse("D3FAA09F-849A-4570-9159-FF939DB5B5C7"),
                Amount = 8,
                TotalPrice = 0,
                ServiceId = Guid.Parse("BCAD3EFE-2064-4D20-BDC3-5881A8AA1B2C"),
                InvoiceId = Guid.Parse("75A3489C-6321-4D5C-9061-1AABA0FC8BBA"),
                PIN = "11223344556"
            };

            var itemDBAmount = _context.InvoiceItem.Find(item.Id).Amount;
            var invoiceDBInTotal = _context.Invoice.Find(item.InvoiceId).InTotal;
            Assert.IsNotNull(itemDBAmount, "Null. Amount not found.");
            Assert.IsNotNull(invoiceDBInTotal, "Null. InTotal not found.");

            Assert.AreNotEqual(item.Amount, itemDBAmount, "Amount must differ.");

            _InvoiceItemController.PutInvoiceItem(item);

            Assert.AreNotEqual(invoiceDBInTotal, _context.Invoice.Find(item.InvoiceId).InTotal, "InTotal must differ.");

            _context.InvoiceItem.Find(item.Id).Amount = 5;
            _context.Invoice.Find(item.InvoiceId).InTotal = invoiceDBInTotal;
            _context.SaveChanges();
        }

        [TestMethod]
        [Description("Delete invoice item.")]
        public void DeleteInvoiceItemTest()
        {
            var id = Guid.Parse("53BEAF4B-F9A2-4C1C-88F3-F334F4261B79");
            var invoiceId = Guid.Parse("75A3489C-6321-4D5C-9061-1AABA0FC8BBA");
            var item = _context.InvoiceItem.Find(id);

            int itemBefore = _context.InvoiceItem.Count();

            _InvoiceItemController.DeleteInvoiceItem(invoiceId, id);

            Assert.IsNull(_context.InvoiceItem.Find(id), "Item exists in DB.");

            int stavkeAfter = _context.InvoiceItem.Count();

            Assert.AreNotEqual(itemBefore, stavkeAfter, "Number of invoice items is equal before and after delete.");

            _context.InvoiceItem.Add(item);
            _context.SaveChanges();
        }
    }
}
