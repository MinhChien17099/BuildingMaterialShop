using System;
using System.Collections.Generic;

#nullable disable

namespace BuildingMaterialShop.Models
{
    public partial class ImportDetail
    {
        public int? Quantity { get; set; }
        public string Unit { get; set; }
        public int ImportId { get; set; }
        public string ProductId { get; set; }

        public virtual Import Import { get; set; }
        public virtual Product Product { get; set; }
    }
}
