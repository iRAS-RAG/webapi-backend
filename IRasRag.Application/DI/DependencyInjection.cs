using IRasRag.Application.Services.Implementations;
using IRasRag.Application.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace IRasRag.Application.DI
{
    public static class DependencyInjection
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddServices();
        }

        public static void AddServices(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(DependencyInjection).Assembly);
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IGrowthStageService, GrowthStageService>();
            services.AddScoped<ISpeciesStageConfigService, SpeciesStageConfigService>();
            services.AddScoped<ISpeciesThresholdService, SpeciesThresholdService>();
        }

    }
}
