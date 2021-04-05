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
    [Route("api/[controller]")]
    [ApiController]
    public class SuppliesController : ControllerBase
    {
        private readonly BuildingMaterialsShopContext _context;

        public SuppliesController(BuildingMaterialsShopContext context)
        {
            _context = context;
        }

        // GET: api/Supplies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Supply>>> GetSupplies()
        {
            return await _context.Supplies.ToListAsync();
        }

        // GET: api/Supplies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Supply>> GetSupply(string id)
        {
            var supply = await _context.Supplies.FindAsync(id);

            if (supply == null)
            {
                return NotFound();
            }

            return supply;
        }

        // PUT: api/Supplies/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSupply(string id, Supply supply)
        {
            if (id != supply.ProductId)
            {
                return BadRequest();
            }

            _context.Entry(supply).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SupplyExists(id))
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

        // POST: api/Supplies
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Supply>> PostSupply(Supply supply)
        {
            _context.Supplies.Add(supply);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (SupplyExists(supply.ProductId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetSupply", new { id = supply.ProductId }, supply);
        }

        // DELETE: api/Supplies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSupply(string id)
        {
            var supply = await _context.Supplies.FindAsync(id);
            if (supply == null)
            {
                return NotFound();
            }

            _context.Supplies.Remove(supply);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SupplyExists(string id)
        {
            return _context.Supplies.Any(e => e.ProductId == id);
        }
    }
}
