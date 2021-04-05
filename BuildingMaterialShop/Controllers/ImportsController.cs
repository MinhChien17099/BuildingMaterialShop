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

        // PUT: api/Imports/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutImport(int id, Import import)
        {
            if (id != import.ImportId)
            {
                return BadRequest();
            }

            _context.Entry(import).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ImportExists(id))
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

        // POST: api/Imports
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Import>> PostImport(Import import)
        {
            _context.Imports.Add(import);
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
    }
}
