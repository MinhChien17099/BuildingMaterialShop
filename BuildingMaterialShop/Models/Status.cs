using System;
using System.Collections.Generic;

#nullable disable

namespace BuildingMaterialShop.Models
{
    public partial class Status
    {
        public Status()
        {
            OrderStatus = new HashSet<OrderStatus>();
        }

        public int StatusId { get; set; }
        public string StatusName { get; set; }

        public virtual ICollection<OrderStatus> OrderStatus { get; set; }
    }
}
