using DemoBed.Services.Order.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DemoBed.Services.Order.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<OrderDbContext>(
                options => options.UseSqlServer(configuration
                .GetConnectionString("DatabaseConnectionString")));

            services.AddScoped<IOrderDbContext, OrderDbContext>();
            return services;
        }
    }
}