using System;
using System.Collections.Generic;

#nullable disable

namespace BuildingMaterialShop.Models
{
    public partial class OrderStatus
    {
        public DateTime? Date { get; set; }
        public int StatusId { get; set; }
        public int OrderId { get; set; }

        public virtual Order Order { get; set; }
        public virtual Status Status { get; set; }
    }
}
