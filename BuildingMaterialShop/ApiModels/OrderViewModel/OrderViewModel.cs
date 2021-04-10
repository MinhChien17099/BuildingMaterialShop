using BuildingMaterialShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildingMaterialShop.ApiModels.OrderViewModel
{
    public class OrderViewModel
    {
        public decimal? Amount { get; set; }
        public int? CustomerId { get; set; }
        public string Address { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<OrderStatus> OrderStatus { get; set; }

    }
}
