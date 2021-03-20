using BuildingMaterialShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildingMaterialShop.ApiModels.CustomerViewModels
{
    public class CustomerViewModel : Customer
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }

        public CustomerViewModel(Customer customer)
        {
            this.CustomerId = customer.CustomerId;
            this.Address = customer.Address;
            this.BirthDay = customer.BirthDay;
            this.Email = customer.Email;
            this.FullName = customer.FullName;
            this.Gender = customer.Gender;
            this.PhoneNumber = customer.PhoneNumber;
        }
    }
}
