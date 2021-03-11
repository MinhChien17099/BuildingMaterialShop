using Microsoft.Extensions.DependencyInjection;

namespace BuildingMaterialShop.Modules.AutoMapper
{
    public static class RegisterAutoMapper
    {
        public static IServiceCollection ConfigureAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);
            return services;
        }
    }
}
