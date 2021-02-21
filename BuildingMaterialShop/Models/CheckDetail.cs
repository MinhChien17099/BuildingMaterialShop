using System;
using System.Collections.Generic;

#nullable disable

namespace BuildingMaterialShop.Models
{
    public partial class CheckDetail
    {
        public int? Quantity { get; set; }
        public int? ActualQuantity { get; set; }
        public string Reason { get; set; }
        public string ProductId { get; set; }
        public int CheckId { get; set; }

        public virtual Check Check { get; set; }
        public virtual Product Product { get; set; }
    }
}
