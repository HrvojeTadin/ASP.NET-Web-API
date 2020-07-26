using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelAPI.Models;

namespace HotelAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly HotelsContext _context;

        public InvoiceController(HotelsContext context)
        {
            _context = context;
        }

        // GET: api/Invoice
        [HttpGet]
        public ActionResult<IEnumerable<Invoice>> GetInvoices()
        {
            var invoices = _context.Invoice.Include(x => x.InvoiceItem).ToList();

            if (invoices.Count > 0)
            {
                return invoices;
            }
            else
                return NoContent();
        }

        // GET: api/Invoice/55415445544
        [HttpGet("{id}")]
        public ActionResult<Invoice> GetInvoice(Guid id)
        {
            var invoice = _context.Invoice.FirstOrDefault(x => x.Id == id);

            if (invoice == null)
            {
                return BadRequest("Invoice by Id does not exist in the database.");
            }

            return invoice;
        }

        // PUT: api/Invoice/5
        [HttpPut]
        public IActionResult PutInvoice(Invoice InvoiceRequest)
        {
            var invoiceDB = _context.Invoice.FirstOrDefault(x => x.Id == InvoiceRequest.Id);

            if (invoiceDB == null)
            {
                return BadRequest("Invoice by Id does not exist in the database.");
            }
            _context.Entry(invoiceDB).State = EntityState.Modified;

            try
            {
                if (!invoiceDB.Paid)
                {
                    invoiceDB.DiscountAmount = InvoiceRequest.DiscountAmount;
                    invoiceDB.InTotal = invoiceDB.TotalWithoutDiscounts - (invoiceDB.TotalWithoutDiscounts * invoiceDB.DiscountAmount);
                }

                _context.SaveChanges();
                return Ok("Invoice change saved successfully.");

            }
            catch (Exception)
            {
                return BadRequest("Database update has failed.");
            }
        }
    }
}
