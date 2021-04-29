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
    public class WareHousesController : ControllerBase
    {
        private readonly BuildingMaterialsShopContext _context;

        public WareHousesController(BuildingMaterialsShopContext context)
        {
            _context = context;
        }

        // GET: api/WareHouses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WareHouse>>> GetWareHouses()
        {
            return await _context.WareHouses.ToListAsync();
        }

        // GET: api/WareHouses/5
        [HttpGet("id")]
        public async Task<ActionResult<WareHouse>> GetWareHouse(int id)
        {

            var warehouse = await _context.WareHouses.FindAsync(id);

            if (warehouse == null)
            {
                return NotFound();
            }

            return warehouse;
        }

        // GET: api/WareHouses/5
        [HttpGet("SoLuongTon/{productId}")]
        public async Task<ActionResult<WareHouse>> GetSoLuongTon(string productId)
        {
            var wareHouse = await _context.WareHouses.Where(wareHouse => wareHouse.ProductId == productId).ToListAsync();

            if (wareHouse.Count < 1)
            {
                return NotFound();
            }

            var maxId = wareHouse.Max(wareHouse => wareHouse.Id);

            return await _context.WareHouses.FindAsync(maxId);
        }

        // PUT: api/WareHouses/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWareHouse(int id, WareHouse wareHouse)
        {
            if (id != wareHouse.Id)
            {
                return BadRequest();
            }

            _context.Entry(wareHouse).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WareHouseExists(id))
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

        // POST: api/WareHouses
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<WareHouse>> PostWareHouse(WareHouse wareHouse)
        {
            _context.WareHouses.Add(wareHouse);

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetWareHouse", new { id = wareHouse.Id }, wareHouse);
        }

        // DELETE: api/WareHouses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWareHouse(int id)
        {
            var wareHouse = await _context.WareHouses.FindAsync(id);
            if (wareHouse == null)
            {
                return NotFound();
            }

            _context.WareHouses.Remove(wareHouse);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool WareHouseExists(int id)
        {
            return _context.WareHouses.Any(e => e.Id == id);
        }
        
    }
}
