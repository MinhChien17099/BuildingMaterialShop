using BuildingMaterialShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildingMaterialShop.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
       
        [HttpGet]
        public IEnumerable<Product> Get()
        {
            using (var context = new BuildingMaterialsShopContext())
            {
                return context.Products.ToList();
            }
        }
    }
}
