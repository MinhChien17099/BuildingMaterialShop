using BuildingMaterialShop.Auth;
using BuildingMaterialShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildingMaterialShop.ApiModels.CustomerViewModels
{
    public class CustomerRegisterViewModel
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string PassWord { get; set; }
        public bool? Gender { get; set; }
        public DateTime? BirthDay { get; set; }
        public Customer ToCustomer()
        {
            Customer customer = new Customer();
            customer.Address = this.Address;
            customer.BirthDay = this.BirthDay;
            customer.Email = this.Email;
            customer.FullName = this.FullName;
            customer.Gender = this.Gender;
            customer.PhoneNumber = this.PhoneNumber;
            customer.PassWord = MD5.CreateMD5(this.PassWord);
            return customer;
        }
    }
}
