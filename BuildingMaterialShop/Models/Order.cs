using System;
using System.Collections.Generic;

#nullable disable

namespace BuildingMaterialShop.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
            OrderStatus = new HashSet<OrderStatus>();
        }

        public int OrderId { get; set; }
        public DateTime? OrderDate { get; set; }
        public decimal? Amount { get; set; }
        public int? CustomerId { get; set; }
        public string Address { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<OrderStatus> OrderStatus { get; set; }
    }
}
