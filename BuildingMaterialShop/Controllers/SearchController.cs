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

    public class SearchController : ControllerBase
    {
        private readonly BuildingMaterialsShopContext _context;

        public SearchController(BuildingMaterialsShopContext context)
        {
            _context = context;
        }

        // GET: Search/keyword
        [HttpGet()]
        public async Task<ActionResult<IEnumerable<Product>>> Search(string keyword)
        {
            //Eager loading
            var products = await _context.Products.Where(pro =>(pro.ProductName + pro.Descriptions + pro.ProductId + pro.Category.CategoryName).ToUpper().Contains(keyword.ToUpper()))
                                                .ToListAsync();

            ////Explicit loading
            //var product = await _context.Products.SingleAsync(pro => pro.ProductId == id);

            ////One - One
            //_context.Entry(product)
            //    .Reference(pro => pro.Category)
            //    .Load();

            ////One - Many
            //_context.Entry(product)
            //    .Collection(pro => pro.Supplies)
            //    .Query().Include(sup => sup.Supplier)
            //    .Load();

            //_context.Entry(product)
            //    .Collection(pro => pro.WareHouses)
            //    .Load();

            if(products==null || products.Count==0)
            {
                return Ok("Không tìm thấy sản phẩm phù hợp.");
            }

            return products;
        }
    }
}
