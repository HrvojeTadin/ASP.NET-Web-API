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
    public class InvoiceItemController : ControllerBase
    {
        private readonly HotelsContext _context;

        public InvoiceItemController(HotelsContext context)
        {
            _context = context;
        }

        // GET: api/InvoiceItem/d6b35732-c12a-44fa-91c9-843f98ef7fd3
        [HttpGet("{id}")]
        public ActionResult<IEnumerable<InvoiceItem>> GetInvoiceItems(Guid id)
        {
            var invoice = _context.Invoice.FirstOrDefault(x => x.Id == id);
            if (invoice == null)
            {
                return BadRequest("Invoide by Id does not exist in the database.");
            }

            var invoiceItems = _context.InvoiceItem.Where(x => x.InvoiceId == id).ToList();

            if (invoiceItems.Count > 0)
            {

                return invoiceItems;
            }
            else
                return NoContent();
        }

        // GET: api/InvoiceItem/d6b35732-c12a-44fa-91c9-843f98ef7fd3/d6b35732-c12a-44fa-91c9-843f98ef7777
        [HttpGet("{idInvoice}/{invoiceItemId}")]
        public ActionResult<InvoiceItem> GetInvoiceItem(Guid idInvoice, Guid invoiceItemId)
        {
            var invoice = _context.Invoice.FirstOrDefault(x => x.Id == idInvoice);
            if (invoice == null)
            {
                return BadRequest("Invoice by default invoice code does not exist in the database.");
            }

            var invoiceItem = _context.InvoiceItem
                .Where(x => x.Id == invoiceItemId && x.InvoiceId == idInvoice).SingleOrDefault();

            if (invoiceItem != null)
            {
                return invoiceItem;
            }
            else
                return BadRequest("Invoice item by ID was not assigned to the issued invoice.");
        }

        // PUT: api/InvoiceItem
        [HttpPut]
        public ActionResult PutInvoiceItem(InvoiceItem invoiceItemRequest)
        {
            var invoiceItem = _context.InvoiceItem
                .Where(x => x.PIN == invoiceItemRequest.PIN && x.InvoiceId == invoiceItemRequest.InvoiceId).SingleOrDefault();

            if (invoiceItem == null)
            {
                return BadRequest("Invoice item by ID does not exist in the database.");
            }

            var service = _context.Service.FirstOrDefault(x => x.Id == invoiceItemRequest.ServiceId);
            if (service == null)
            {
                return BadRequest("Unable to update account item -> Service by default ID does not exist in database.");
            }

            try
            {
                // calculation of the total price of the item
                invoiceItem.TotalPrice = invoiceItemRequest.Amount * service.PricePerItem;
                invoiceItem.ServiceId = service.Id;
                invoiceItem.Amount = invoiceItemRequest.Amount;

                _context.Entry(invoiceItem).State = EntityState.Modified;
                _context.SaveChanges();

                // update Total prices on the account
                var invoice = _context.Invoice
                    .Include(x => x.InvoiceItem)
                    .FirstOrDefault(x => x.Id == invoiceItem.InvoiceId);

                invoice.TotalWithoutDiscounts = invoice.InvoiceItem.Sum(x => x.TotalPrice);  //The sum of all total invoice item prices
                invoice.InTotal = invoice.TotalWithoutDiscounts - (invoice.TotalWithoutDiscounts * invoice.DiscountAmount);
                _context.Update(invoice);
                _context.SaveChanges();

                return Ok("The information of invoice item has been saved successfuly.");
            }
            catch (Exception)
            {
                return BadRequest("Database update has failed.");
            }
        }

        // POST: api/InvoiceItem
        [HttpPost]
        public ActionResult<InvoiceItem> PostInvoiceItem(InvoiceItem invoiceItemRequest)
        {
            var invoice = _context.Invoice
                .Include(x => x.InvoiceItem)
                .FirstOrDefault(x => x.Id == invoiceItemRequest.InvoiceId);

            if (invoice == null)
            {
                return BadRequest("Unable to update invoice -> Inovice by default ID does not exist in database.");
            }

            var invoiceItem = _context.InvoiceItem
            .Where(x => x.PIN == invoiceItemRequest.PIN && x.InvoiceId == invoiceItemRequest.InvoiceId).SingleOrDefault();

            if (invoiceItem == null)
            {
                var serviceDB = _context.Service.FirstOrDefault(x => x.Id == invoiceItemRequest.ServiceId);
                if (serviceDB == null)
                {
                    return BadRequest("Unable to update account item -> Service by default ID does not exist in database.");
                }
                invoiceItemRequest.TotalPrice = invoiceItemRequest.Amount * serviceDB.PricePerItem;

                _context.Add(invoiceItemRequest);
                _context.SaveChanges();

                // update Total prices on the invoice
                invoice.TotalWithoutDiscounts = invoice.InvoiceItem.Sum(x => x.TotalPrice);  //The sum of all total invoice item prices
                invoice.InTotal = invoice.TotalWithoutDiscounts - (invoice.TotalWithoutDiscounts * invoice.DiscountAmount);
                _context.Update(invoice);
                _context.SaveChanges();

                return CreatedAtAction("GetInvoiceItem", new { invoiceItemRequest.InvoiceId, invoiceItemRequest.Id }, invoiceItemRequest);
            }
            return BadRequest("An attempt to enter failed. The inovice item already exists in the database.");
        }

        // DELETE: api/InvoiceItem/cd50460c-0070-4c74-ade5-065d289eb516/d6b35732-c12a-44fa-91c9-843f98ef7fd3
        [HttpDelete("{idInvoice}/{invoiceItemId}")]
        public IActionResult DeleteInvoiceItem(Guid idInvoice, Guid invoiceItemId)
        {
            var item = _context.InvoiceItem.FirstOrDefault(x => x.Id == invoiceItemId);
            if (item == null)
            {
                return BadRequest("The invoice item by default ID does not exist in the database.");
            }

            var invoice = _context.Invoice
                        .Include(x => x.InvoiceItem)
                        .FirstOrDefault(x => x.Id == idInvoice);
            if (invoice == null)
            {
                return BadRequest("The invoice by default ID does not exist in the database.");
            }

            _context.InvoiceItem.Remove(item);
            _context.SaveChanges();

            // update Total prices on the account
            invoice.InvoiceItem.Remove(item);
            invoice.TotalWithoutDiscounts = invoice.InvoiceItem.Sum(x => x.TotalPrice) - item.TotalPrice;  //The sum of total invoice item prices - price of an item
            invoice.InTotal = invoice.TotalWithoutDiscounts - (invoice.TotalWithoutDiscounts * invoice.DiscountAmount);
            _context.Update(invoice);
            _context.SaveChanges();

            return Ok(String.Format("Invoice ID {0} was successfully deleted.", invoiceItemId));
        }
    }
}
