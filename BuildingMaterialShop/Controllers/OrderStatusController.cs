using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BuildingMaterialShop.Models;
using Microsoft.AspNetCore.Cors;

namespace BuildingMaterialShop.Controllers
{
    [Route("[controller]")]
    [ApiController]

    public class OrderStatusController : ControllerBase
    {
        private readonly BuildingMaterialsShopContext _context;

        public OrderStatusController(BuildingMaterialsShopContext context)
        {
            _context = context;
        }

        // GET: api/OrderStatus
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderStatus>>> GetOrderStatus()
        {
            return await _context.OrderStatus.ToListAsync();
        }

        // GET: api/OrderStatus/5
        [HttpGet("GetOrderStatusByOrderId/{orderId}")]
        public async Task<ActionResult<OrderStatus>> GetOrderStatus(int orderId)
        {
            var orderStatus = await _context.OrderStatus.Where(orderStatus => orderStatus.OrderId == orderId)
                                                        .FirstOrDefaultAsync();

            if (orderStatus == null)
            {
                return NotFound();
            }

            return orderStatus;
        }


        // POST: api/OrderStatus
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("DeleteOrderByCustomer/{orderId}")]
        public async Task<ActionResult<OrderStatus>> DeleteByCustomer(int orderId)
        {
            var order = await _context.Orders.Where(o => o.OrderId == orderId)
                                                .Include(o => o.OrderDetails)
                                                .Include(o => o.OrderStatus)
                                                .FirstOrDefaultAsync();
            if (order == null)
            {
                return NotFound();
            }

            if (order.OrderStatus.Count > 1 || order.OrderStatus.First().StatusId == 1)
            {
                return Ok("Không thể hủy đơn hàng.");
            }

            //Xoá trạng thái chờ trong đơn hàng
            if (_context.OrderStatus.Count() != 0)
                _context.OrderStatus.Remove(order.OrderStatus.First());

            //Thêm trạng thái hủy
            OrderStatus orderStatus = new OrderStatus();
            orderStatus.Date = DateTime.Now;
            orderStatus.StatusId = 1;
            orderStatus.OrderId = orderId;

            _context.OrderStatus.Add(orderStatus);

            //Thêm lại số lượng sản phẩm
            tangsoluong(order.OrderDetails);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (OrderStatusExists(orderStatus.StatusId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetOrderStatus", new { id = orderStatus.StatusId }, orderStatus);
        }

        [HttpPost("DeleteOrderByEmployee/{orderId}")]
        public async Task<ActionResult<OrderStatus>> DeleteByEmployee(int orderId)
        {
            var order = await _context.Orders.Where(o => o.OrderId == orderId)
                                               .Include(o => o.OrderDetails)
                                               .Include(o => o.OrderStatus)
                                               .FirstOrDefaultAsync();

            if (order == null)
            {
                return NotFound();
            }

            if (order.OrderStatus.Count > 2 || order.OrderStatus.First().StatusId == 1)
            {
                return Ok("Không thể hủy đơn hàng.");
            }

            //Xoá trạng thái chờ trong đơn hàng
            if (_context.OrderStatus.Count() != 0)
                _context.OrderStatus.RemoveRange(order.OrderStatus);

            //Thêm trạng thái hủy
            OrderStatus orderStatus = new OrderStatus();
            orderStatus.Date = DateTime.Now;
            orderStatus.StatusId = 1;
            orderStatus.OrderId = orderId;

            _context.OrderStatus.Add(orderStatus);

            //Thêm lại số lượng sản phẩm
            tangsoluong(order.OrderDetails);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (OrderStatusExists(orderStatus.StatusId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetOrderStatus", new { id = orderStatus.StatusId }, orderStatus);
        }

        // POST: api/OrderStatus
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("ConfirmOrder/{orderId}")]
        public async Task<ActionResult<OrderStatus>> XacNhanDonHang(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return NotFound();
            }

            OrderStatus orderStatus = new OrderStatus();
            orderStatus.Date = DateTime.Now;
            orderStatus.StatusId = 3;
            orderStatus.OrderId = orderId;

            _context.OrderStatus.Add(orderStatus);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (OrderStatusExists(orderStatus.StatusId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetOrderStatus", new { id = orderStatus.StatusId }, orderStatus);
        }
        // POST: api/OrderStatus
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("DeliveredOrder/{orderId}")]
        public async Task<ActionResult<OrderStatus>> DaGiaoDonHang(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return NotFound();
            }

            OrderStatus orderStatus = new OrderStatus();
            orderStatus.Date = DateTime.Now;
            orderStatus.StatusId = 4;
            orderStatus.OrderId = orderId;

            _context.OrderStatus.Add(orderStatus);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (OrderStatusExists(orderStatus.StatusId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetOrderStatus", new { id = orderStatus.StatusId }, orderStatus);
        }
        // POST: api/OrderStatus
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<OrderStatus>> PostOrderStatus(OrderStatus orderStatus)
        {
            _context.OrderStatus.Add(orderStatus);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (OrderStatusExists(orderStatus.StatusId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetOrderStatus", new { id = orderStatus.StatusId }, orderStatus);
        }



        private bool OrderStatusExists(int id)
        {
            return _context.OrderStatus.Any(e => e.StatusId == id);
        }
        private int? getProductQuantity(string productId)
        {
            return _context.WareHouses.Where(w => w.ProductId == productId).ToList().Last().Quantity;
        }

        private void tangsoluong(ICollection<OrderDetail> details)
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
