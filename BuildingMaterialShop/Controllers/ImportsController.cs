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
    public class ImportsController : ControllerBase
    {
        private readonly BuildingMaterialsShopContext _context;

        public ImportsController(BuildingMaterialsShopContext context)
        {
            _context = context;
        }

        // GET: api/Imports
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Import>>> GetImports()
        {
            return await _context.Imports.ToListAsync();
        }

        // GET: api/Imports/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Import>> GetImport(int id)
        {
            var import = await _context.Imports.FindAsync(id);

            if (import == null)
            {
                return NotFound();
            }

            return import;
        }



        // POST: api/Imports
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Import>> PostImport(Import import)
        {
            _context.Imports.Add(import);

            tangsoluong(import.ImportDetails);

            await _context.SaveChangesAsync();



            return CreatedAtAction("GetImport", new { id = import.ImportId }, import);
        }

        // DELETE: api/Imports/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteImport(int id)
        {
            var import = await _context.Imports.FindAsync(id);
            if (import == null)
            {
                return NotFound();
            }

            _context.Imports.Remove(import);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ImportExists(int id)
        {
            return _context.Imports.Any(e => e.ImportId == id);
        }
        private int? getProductQuantity(string productId)
        {
            return _context.WareHouses.Where(w => w.ProductId == productId).ToList().Last().Quantity;
        }
        private void tangsoluong(ICollection<ImportDetail> details)
        {
            foreach (var detail in details)
            {
                string productId = detail.ProductId;
                int? quantity = detail.Quantity;

                WareHouse ware = new WareHouse();
                ware.Date = DateTime.Now;
                ware.ProductId = productId;
                ware.Quantity = getProductQuantity(productId) + quantity;

                _context.WareHouses.Add(ware);
            }
        }
    }
}
