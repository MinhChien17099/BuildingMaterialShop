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
    public class CheckDetailsController : ControllerBase
    {
        private readonly BuildingMaterialsShopContext _context;

        public CheckDetailsController(BuildingMaterialsShopContext context)
        {
            _context = context;
        }

        // GET: api/CheckDetails
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CheckDetail>>> GetCheckDetails()
        {
            return await _context.CheckDetails.ToListAsync();
        }

        // GET: api/CheckDetails/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CheckDetail>> GetCheckDetail(int id)
        {
            var checkDetail = await _context.CheckDetails.FindAsync(id);

            if (checkDetail == null)
            {
                return NotFound();
            }

            return checkDetail;
        }

        // PUT: api/CheckDetails/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCheckDetail(int id, CheckDetail checkDetail)
        {
            if (id != checkDetail.CheckId)
            {
                return BadRequest();
            }

            _context.Entry(checkDetail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CheckDetailExists(id))
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

        // POST: api/CheckDetails
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CheckDetail>> PostCheckDetail(CheckDetail checkDetail)
        {
            _context.CheckDetails.Add(checkDetail);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CheckDetailExists(checkDetail.CheckId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetCheckDetail", new { id = checkDetail.CheckId }, checkDetail);
        }

        // DELETE: api/CheckDetails/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCheckDetail(int id)
        {
            var checkDetail = await _context.CheckDetails.FindAsync(id);
            if (checkDetail == null)
            {
                return NotFound();
            }

            _context.CheckDetails.Remove(checkDetail);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CheckDetailExists(int id)
        {
            return _context.CheckDetails.Any(e => e.CheckId == id);
        }
    }
}
