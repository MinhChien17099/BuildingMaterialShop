using System;
using System.Collections.Generic;

#nullable disable

namespace BuildingMaterialShop.Models
{
    public partial class RefreshTokenCustomer
    {
        public int TokenId { get; set; }
        public int CustomerId { get; set; }
        public string Token { get; set; }
        public DateTime ExpiryDate { get; set; }

        public virtual Customer Customer { get; set; }
    }
}
