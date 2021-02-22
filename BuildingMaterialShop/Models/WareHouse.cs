using System;
using System.Collections.Generic;

#nullable disable

namespace BuildingMaterialShop.Models
{
    public partial class WareHouse
    {
        public int Id { get; set; }
        public int? Quantity { get; set; }
        public DateTime? Date { get; set; }
        public string ProductId { get; set; }

        public virtual Product Product { get; set; }
    }
}
