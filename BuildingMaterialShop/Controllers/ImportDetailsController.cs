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
    public class ImportDetailsController : ControllerBase
    {
        private readonly BuildingMaterialsShopContext _context;

        public ImportDetailsController(BuildingMaterialsShopContext context)
        {
            _context = context;
        }

        // GET: api/ImportDetails
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ImportDetail>>> GetImportDetails()
        {
            return await _context.ImportDetails.ToListAsync();
        }

        // GET: api/ImportDetails/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ImportDetail>> GetImportDetail(int id)
        {
            var importDetail = await _context.ImportDetails.FindAsync(id);

            if (importDetail == null)
            {
                return NotFound();
            }

            return importDetail;
        }

        // PUT: api/ImportDetails/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutImportDetail(int id, ImportDetail importDetail)
        {
            if (id != importDetail.ImportId)
            {
                return BadRequest();
            }

            _context.Entry(importDetail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ImportDetailExists(id))
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

        // POST: api/ImportDetails
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ImportDetail>> PostImportDetail(ImportDetail importDetail)
        {
            _context.ImportDetails.Add(importDetail);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ImportDetailExists(importDetail.ImportId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetImportDetail", new { id = importDetail.ImportId }, importDetail);
        }

        // DELETE: api/ImportDetails/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteImportDetail(int id)
        {
            var importDetail = await _context.ImportDetails.FindAsync(id);
            if (importDetail == null)
            {
                return NotFound();
            }

            _context.ImportDetails.Remove(importDetail);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ImportDetailExists(int id)
        {
            return _context.ImportDetails.Any(e => e.ImportId == id);
        }
    }
}
