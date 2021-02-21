using System;
using System.Collections.Generic;

#nullable disable

namespace BuildingMaterialShop.Models
{
    public partial class Check
    {
        public Check()
        {
            CheckDetails = new HashSet<CheckDetail>();
        }

        public int CheckId { get; set; }
        public DateTime? Date { get; set; }
        public bool? Status { get; set; }
        public int? EmployeeId { get; set; }

        public virtual Employee Employee { get; set; }
        public virtual ICollection<CheckDetail> CheckDetails { get; set; }
    }
}
