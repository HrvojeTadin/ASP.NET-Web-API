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
    public class ServiceController : ControllerBase
    {
        private readonly HotelsContext _context;

        public ServiceController(HotelsContext context)
        {
            _context = context;
        }

        // GET: api/Service
        [HttpGet]
        public ActionResult<IEnumerable<Service>> GetServices()
        {
            var service = _context.Service.ToList();

            if (service.Count() > 0)
            {
                return service;
            }
            else
                return NoContent();
        }


        // GET: api/Service/26e257cd-8aa7-4f50-93cc-00288ff42041
        [HttpGet("{id}")]
        public ActionResult<Service> GetService(Guid id)
        {
            var service = _context.Service.FirstOrDefault(x => x.Id == id);

            if (service == null)
            {
                return BadRequest("Service, by given Id, does not exist in database.");
            }

            return service;
        }

        // PUT: api/Service
        [HttpPut]
        public IActionResult PutService(Service service)
        {
            var serviceDB = _context.Service.FirstOrDefault(x => x.Id == service.Id);

            if (serviceDB == null)
            {
                return BadRequest("The service by default Id does not exist in the database.");
            }

            serviceDB.Name = service.Name;
            serviceDB.PricePerItem = service.PricePerItem;

            _context.Entry(serviceDB).State = EntityState.Modified;

            try
            {
                _context.SaveChanges();
                return Ok("The service update has been successful.");
            }
            catch (Exception)
            {
                return BadRequest("Database update has failed.");
            }
        }

        // POST: api/Service
        [HttpPost]
        public ActionResult<Service> PostService(Service service)
        {
            var serviceDB = _context.Service.FirstOrDefault(x => x.Id == service.Id);

            if (serviceDB == null)
            {
                _context.Service.Add(service);
                _context.SaveChanges();
                return CreatedAtAction("GetService", new { service.Id }, service);
            }
            return BadRequest("Can not insert the service in database. The service aleready exist in database.");
        }

        // DELETE: api/Service/5ad7216f-fc9e-41a2-8e87-6c82b566c1a7
        [HttpDelete("{id}")]
        public ActionResult<Service> DeleteService(Guid id)
        {
            var service = _context.Service.FirstOrDefault(x => x.Id == id);

            if (service == null)
            {
                return BadRequest("Service by given ID does not exist in database.");
            }

            _context.Service.Remove(service);
            _context.SaveChanges();

            return service;
        }
    }
}
