using DemoBed.Services.Payment.ApplicationService.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DemoBed.Services.Payment.ApplicationService
{
    public static class DependenyInjection
    {
        public static IServiceCollection AddApplicationService(
            this IServiceCollection services)
        {
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddTransient<IPaymentService, PaymentService>();

            return services;
        }
    }
}