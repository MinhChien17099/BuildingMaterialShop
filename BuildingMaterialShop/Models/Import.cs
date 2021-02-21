using System;
using System.Collections.Generic;

#nullable disable

namespace BuildingMaterialShop.Models
{
    public partial class Import
    {
        public Import()
        {
            ImportDetails = new HashSet<ImportDetail>();
        }

        public int ImportId { get; set; }
        public DateTime? Date { get; set; }
        public int? EmployeeId { get; set; }
        public string SupplierId { get; set; }

        public virtual Employee Employee { get; set; }
        public virtual Supplier Supplier { get; set; }
        public virtual ICollection<ImportDetail> ImportDetails { get; set; }
    }
}
