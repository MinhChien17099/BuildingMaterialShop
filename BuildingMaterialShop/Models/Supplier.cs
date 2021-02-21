using System;
using System.Collections.Generic;

#nullable disable

namespace BuildingMaterialShop.Models
{
    public partial class Supplier
    {
        public Supplier()
        {
            Imports = new HashSet<Import>();
            Supplies = new HashSet<Supply>();
        }

        public string SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }

        public virtual ICollection<Import> Imports { get; set; }
        public virtual ICollection<Supply> Supplies { get; set; }
    }
}
