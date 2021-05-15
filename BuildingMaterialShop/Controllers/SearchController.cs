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

    public class SearchController : ControllerBase
    {
        private readonly BuildingMaterialsShopContext _context;

        public SearchController(BuildingMaterialsShopContext context)
        {
            _context = context;
        }

        // GET: Search/keyword
        [HttpGet()]
        public async Task<ActionResult<IEnumerable<Product>>> Search(string keyword, string type)
        {
            List<Product> products;
            keyword = string.IsNullOrEmpty(keyword) ? "" : keyword;
            switch (type)
            {
                case "cate":
                    {
                        products = await _context.Products.Where(pro => (pro.Category.CategoryId + pro.Category.CategoryName).ToUpper().Contains(keyword.Trim().ToUpper()))
                                                   .ToListAsync();
                        break;
                    };
                case "product":
                    {
                        products = await _context.Products.Where(pro => (pro.ProductId + pro.ProductName + pro.Descriptions).ToUpper().Contains(keyword.Trim().ToUpper()))
                                                   .ToListAsync();
                        break;
                    }

                default:
                    {
                        products = await _context.Products.Where(pro => (pro.ProductName + pro.Descriptions + pro.ProductId + pro.Category.CategoryName).ToUpper().Contains(keyword.Trim().ToUpper()))
                                                    .ToListAsync();
                        break;

                    }
            }
            //Eager loading
            //var products = await _context.Products.Where(pro => RemoveUnicode(pro.ProductName + pro.Descriptions + pro.ProductId + pro.Category.CategoryName).ToUpper().Contains(keyword.ToUpper())).ToListAsync();

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

            if (products == null || products.Count == 0)
            {
                return Ok("Không tìm thấy sản phẩm phù hợp.");
            }

            return products;
        }


        private string RemoveUnicode(string text)
        {
            //string[] arr1 = new string[] { "á", "à", "ả", "ã", "ạ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ",
            //                                "đ",
            //                                "é","è","ẻ","ẽ","ẹ","ê","ế","ề","ể","ễ","ệ",
            //                                "í","ì","ỉ","ĩ","ị",
            //                                "ó","ò","ỏ","õ","ọ","ô","ố","ồ","ổ","ỗ","ộ","ơ","ớ","ờ","ở","ỡ","ợ",
            //                                "ú","ù","ủ","ũ","ụ","ư","ứ","ừ","ử","ữ","ự",
            //                                "ý","ỳ","ỷ","ỹ","ỵ",};
            //string[] arr2 = new string[] { "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a",
            //                                "d",
            //                                "e","e","e","e","e","e","e","e","e","e","e",
            //                                "i","i","i","i","i",
            //                                "o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o",
            //                                "u","u","u","u","u","u","u","u","u","u","u",
            //                                "y","y","y","y","y",};
            //for (int i = 0; i < arr1.Length; i++)
            //{
            //    text = text.Replace(arr1[i], arr2[i]);
            //    text = text.Replace(arr1[i].ToUpper(), arr2[i].ToUpper());
            //}
            return text;
        }
    }
}
