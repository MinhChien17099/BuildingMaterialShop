using BuildingMaterialShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildingMaterialShop.ApiModels.EmployeeViewModel
{
    public class EmployeeViewModel : Employee
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }

        public EmployeeViewModel(Employee employee)
        {
            this.EmployeeId = employee.EmployeeId;
            this.Address = employee.Address;
            this.BirthDay = employee.BirthDay;
            this.Email = employee.Email;
            this.FullName = employee.FullName;
            this.Gender = employee.Gender;
            this.PhoneNumber = employee.PhoneNumber;
            this.RoleId = employee.RoleId;
        }
    }
}
