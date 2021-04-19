using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BuildingMaterialShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;

namespace BuildingMaterialShop.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]

    public class ProductsController : ControllerBase
    {
        private readonly BuildingMaterialsShopContext _context;

        public ProductsController(BuildingMaterialsShopContext context)
        {
            _context = context;
        }

        // GET: Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _context.Products.Include(pro => pro.WareHouses)
                                            .Include(pro => pro.Category)
                                            .ToListAsync();

        }


        // GET: Products/5
        [HttpGet("{productId}")]
        public async Task<ActionResult<Product>> GetProductDetails(string productId)
        {
            var product = await _context.Products.Include(pro => pro.WareHouses)
                                                .Include(pro => pro.Category)
                                                .Include(pro => pro.Supplies).ThenInclude(pro => pro.Supplier)
                                                .Where(pro => pro.ProductId == productId).FirstOrDefaultAsync();

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        // POST: Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            if (String.IsNullOrEmpty(product.ProductId))
            {
                return Ok("Mã sản phẩm không được để trống.");
            }

            if (!CategoryExists(product.CategoryId))
            {
                return NotFound();
            }

            WareHouse ware = new WareHouse();
            ware.Date = DateTime.Now;
            ware.ProductId = product.ProductId;
            ware.Quantity = 0;

            product.WareHouses.Add(ware);

            _context.Products.Add(product);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ProductExists(product.ProductId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetProductDetails", new { id = product.ProductId }, product);
        }

        // PUT: Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(string id, Product product)
        {
            if (id != product.ProductId)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
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

        // DELETE: Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(string id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
        private bool CategoryExists(string categoryId)
        {
            return _context.Categories.Any(e => e.CategoryId == categoryId);
        }
    }
}
