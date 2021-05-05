using System;
using System.Collections.Generic;

#nullable disable

namespace BuildingMaterialShop.Models
{
    public partial class Employee
    {
        public Employee()
        {
            Checks = new HashSet<Check>();
            Imports = new HashSet<Import>();
            RefreshTokenEmployees = new HashSet<RefreshTokenEmployee>();
        }

        public int EmployeeId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string PassWord { get; set; }
        public bool? Gender { get; set; }
        public DateTime? BirthDay { get; set; }
        public string RoleId { get; set; }
        public bool IsBlocked { get; set; }

        public virtual Role Role { get; set; }
        public virtual ICollection<Check> Checks { get; set; }
        public virtual ICollection<Import> Imports { get; set; }
        public virtual ICollection<RefreshTokenEmployee> RefreshTokenEmployees { get; set; }
    }
}
