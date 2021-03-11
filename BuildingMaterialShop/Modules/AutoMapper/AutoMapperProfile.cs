using AutoMapper;
using BuildingMaterialShop.Models;

namespace BuildingMaterialShop.Modules.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Customer, Customer>();
        }
    }
}
