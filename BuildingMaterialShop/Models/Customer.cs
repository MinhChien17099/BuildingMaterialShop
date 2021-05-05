using System;
using System.Collections.Generic;

#nullable disable

namespace BuildingMaterialShop.Models
{
    public partial class Customer
    {
        public Customer()
        {
            Orders = new HashSet<Order>();
            RefreshTokenCustomers = new HashSet<RefreshTokenCustomer>();
        }

        public int CustomerId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string PassWord { get; set; }
        public bool? Gender { get; set; }
        public DateTime? BirthDay { get; set; }
        public bool IsBlocked { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<RefreshTokenCustomer> RefreshTokenCustomers { get; set; }
    }
}
