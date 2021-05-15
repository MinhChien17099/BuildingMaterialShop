using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BuildingMaterialShop.Models;
using Microsoft.AspNetCore.Cors;
using System.Collections;
using Newtonsoft.Json;

namespace BuildingMaterialShop.Controllers
{
    [Route("[controller]")]
    [ApiController]

    public class ThongKeController : ControllerBase
    {
        private readonly BuildingMaterialsShopContext _context;

        public ThongKeController(BuildingMaterialsShopContext context)
        {
            _context = context;
        }

        // GET: Categories
        [HttpGet("ThongKeDoanhThuTheoLoai")]
        public async Task<ActionResult> ThongKeTheoLoaiSanPham(DateTime startDate, DateTime endDate)
        {
            if (!validDate(startDate) || !validDate(endDate) || DateTime.Compare(startDate, endDate) >0)
            {
                return Ok("Thời gian không hợp lệ.");
            }

            var loai = await _context.Categories.ToListAsync();

            Dictionary<string, decimal> doanhthu = new Dictionary<string, decimal>();

            foreach (var item in loai)
            {
                doanhthu.Add(item.CategoryId, Decimal.Zero);
                var chitiet = _context.OrderDetails.Where(c => c.Order.OrderStatus.Count == ((int)Enum.OrderStatus.DA_GIAO_HANG - 1)
                                                        && DateTime.Compare((DateTime)c.Order.OrderDate, startDate) > 0 && DateTime.Compare((DateTime)c.Order.OrderDate, endDate) < 0
                                                        && c.Product.CategoryId == item.CategoryId)
                                                        .Include(c => c.Order)
                                                            .ThenInclude(od => od.OrderStatus)
                                                        .Include(c => c.Product)
                                                        .ToList();

                for (int i = 0; i < chitiet.Count; i++)
                {
                    doanhthu[item.CategoryId] = (decimal)(doanhthu[item.CategoryId] + (chitiet[i].Quantity * chitiet[i].Product.Price));
                }

            }

            List<object> rs = new List<object>();
            foreach (var item in loai)
            {
                //Create my object
                var json = new
                {
                    name = item.CategoryName,
                    value = doanhthu[item.CategoryId]
                };
                rs.Add(json);
            }
            

            return Ok(JsonConvert.SerializeObject(rs));
        }

        private bool validDate(DateTime date)
        {
            DateTime minDate = new DateTime(1970, 1, 1);
            DateTime maxDate = DateTime.MaxValue;
            return DateTime.Compare(date, minDate) > 0 && DateTime.Compare(date, maxDate) < 0;
        }
    }
}
