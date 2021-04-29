using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BuildingMaterialShop.Models;

namespace BuildingMaterialShop.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SuppliersController : ControllerBase
    {
        private readonly BuildingMaterialsShopContext _context;

        public SuppliersController(BuildingMaterialsShopContext context)
        {
            _context = context;
        }

        // GET: api/Suppliers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Supplier>>> GetSuppliers()
        {
            return await _context.Suppliers.ToListAsync();
        }

        // GET: api/Suppliers/5
        [HttpGet("{supplierId}")]
        public async Task<ActionResult<Supplier>> GetSupplier(string supplierId)
        {
            var supplier = await _context.Suppliers.SingleOrDefaultAsync(sup=>sup.SupplierId==supplierId);
            if (supplier == null)
            {
                return NotFound();
            }
            _context.Entry(supplier)
                .Collection(sup =>sup.Supplies)
               .Query().Include(supply => supply.Product)
                .Load();

            

            return supplier;
        }

        // PUT: api/Suppliers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{supplierId}")]
        public async Task<IActionResult> PutSupplier(string supplierId, Supplier supplier)
        {
            if (supplierId != supplier.SupplierId)
            {
                return BadRequest();
            }

            _context.Entry(supplier).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SupplierExists(supplierId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Suppliers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Supplier>> PostSupplier(Supplier supplier)
        {
            _context.Suppliers.Add(supplier);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (SupplierExists(supplier.SupplierId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetSupplier", new { id = supplier.SupplierId }, supplier);
        }

        

        private bool SupplierExists(string id)
        {
            return _context.Suppliers.Any(e => e.SupplierId == id);
        }
    }
}
