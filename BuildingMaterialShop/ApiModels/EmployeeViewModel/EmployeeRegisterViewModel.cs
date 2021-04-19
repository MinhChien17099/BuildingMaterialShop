using BuildingMaterialShop.Auth;
using BuildingMaterialShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildingMaterialShop.ApiModels.EmployeeViewModel
{
    public class EmployeeRegisterViewModel
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string PassWord { get; set; }
        public bool? Gender { get; set; }
        public DateTime? BirthDay { get; set; }
        public string RoleId { get; set; }

        public Employee ToEmployee()
        {
            Employee employee = new Employee();
            employee.Address = this.Address;
            employee.BirthDay = this.BirthDay;
            employee.Email = this.Email;
            employee.FullName = this.FullName;
            employee.Gender = this.Gender;
            employee.PhoneNumber = this.PhoneNumber;
            employee.PassWord = MD5.CreateMD5(this.PassWord);
            employee.RoleId = this.RoleId;
            return employee;
        }
    }
}
