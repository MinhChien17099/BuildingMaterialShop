using System;
using System.Collections.Generic;

#nullable disable

namespace BuildingMaterialShop.Models
{
    public partial class Product
    {
        public Product()
        {
            CheckDetails = new HashSet<CheckDetail>();
            ImportDetails = new HashSet<ImportDetail>();
            OrderDetails = new HashSet<OrderDetail>();
            Supplies = new HashSet<Supply>();
            WareHouses = new HashSet<WareHouse>();
        }

        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal? Price { get; set; }
        public string Unit { get; set; }
        public string Images { get; set; }
        public string Descriptions { get; set; }
        public bool? Status { get; set; }
        public string CategoryId { get; set; }

        public virtual Category Category { get; set; }
        public virtual ICollection<CheckDetail> CheckDetails { get; set; }
        public virtual ICollection<ImportDetail> ImportDetails { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<Supply> Supplies { get; set; }
        public virtual ICollection<WareHouse> WareHouses { get; set; }
    }
}
