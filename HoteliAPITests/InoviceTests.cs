using HotelAPI.Controllers;
using HotelAPI.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace HotelAPITests
{
    [TestClass]
    public class InvoiceTests
    {
        private HotelsContext _context;
        private InvoiceController _InvoiceController;

        [TestInitialize]
        public void TestInit()
        {
            _context = Utility.GetDbContext();
            _InvoiceController = new InvoiceController(_context);
        }

        [TestMethod]
        [Description("Get all invoices.")]
        public void GetInvoicesTest()
        {
            int inTotalDB = _context.Invoice.Count();
            int inTotal = _InvoiceController.GetInvoices().Value.Count();
            Assert.IsTrue(inTotal == inTotalDB, "Number of invoices is not equal.");
        }

        [TestMethod]
        [Description("Get one invoice.")]
        public void GetInvoiceTest()
        {
            var id = Guid.Parse("75A3489C-6321-4D5C-9061-1AABA0FC8BBA");
            Invoice invoiceDB = _context.Invoice.Find(id);
            Assert.IsNotNull(invoiceDB, "Null. Inovice is not in database.");

            Invoice invoice = _InvoiceController.GetInvoice(id).Value;
            Assert.IsNotNull(invoice, "Null. Inovice is not in database.");

            Assert.AreEqual(invoiceDB.Id, invoice.Id, "InvoiceDB.Id != Invoice.Id");
        }

        [TestMethod]
        [Description("Update existing invoice.")]
        public void PutInvoiceTest()
        {
            var id = Guid.Parse("75A3489C-6321-4D5C-9061-1AABA0FC8BBA");
            Invoice invoice = _context.Invoice.Find(id);
            Assert.IsNotNull(invoice, "Null. Inovice is not in database.");

            decimal InTotalDB = invoice.InTotal;

            invoice.DiscountAmount = 0.18m;

            _InvoiceController.PutInvoice(invoice);

            Assert.AreNotEqual(_context.Invoice.Find(id).InTotal, InTotalDB, "InTotal == InTotalDB.");

            _context.Invoice.Find(id).DiscountAmount = 0.9m;
            _context.Invoice.Find(id).InTotal = InTotalDB;
            _context.SaveChanges();
        }
    }
}
