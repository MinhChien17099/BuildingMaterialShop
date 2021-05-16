using BuildingMaterialShop.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildingMaterialShop.ApiModels.ProductViewModel
{
    public class InsertProductViewModel
    {
        public string productId { get; set; }
        public string productName { get; set; }
        public decimal price { get; set; }
        public string unit { get; set; }
        public string images { get; set; }
        public string descriptions { get; set; }
        public bool status { get; set; }
        public string categoryId { get; set; }
        public string[] supplies { get; set; }
       

        public Product ToProduct()
        {
            Product product = new Product();
            product.ProductId = this.productId;
            product.ProductName = this.productName;
            product.Price = this.price;
            product.Unit = this.unit;
            product.Images = this.images;
            product.Descriptions = this.categoryId;
            product.Status = this.status;
            product.CategoryId = this.categoryId;
            product.Supplies = getSupplies(this.supplies);

            return product;
        }

        private ICollection<Supply> getSupplies(string[] supplies)
        {
            Supply[] rs = new Supply[supplies.Length];
            for (int i = 0; i < supplies.Length; i++)
            {
                Supply supply = new Supply();
                supply.ProductId = this.productId;
                supply.SupplierId = supplies[i];
                rs[i] = supply;
            }
            return rs;
        }
    }

}
