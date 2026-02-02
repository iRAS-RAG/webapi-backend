using IRasRag.Application.Common.Mappings;
using IRasRag.Application.Services.Implementations;
using IRasRag.Application.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IRasRag.Application.DI
{
    public static class DependencyInjection
    {
        public static void AddApplicationServices(
            this IServiceCollection services,
            IConfiguration config
        )
        {
            services.AddServices();
            services.AddAutoMapper(config);
        }

        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IGrowthStageService, GrowthStageService>();
            services.AddScoped<ISpeciesStageConfigService, SpeciesStageConfigService>();
            services.AddScoped<ISpeciesThresholdService, SpeciesThresholdService>();
        }

        public static void AddAutoMapper(this IServiceCollection services, IConfiguration config)
        {
            services.AddAutoMapper(cfg =>
            {
                cfg.LicenseKey = config["AutoMapperKey"];
                cfg.AddProfile<GrowthStageProfile>();
                cfg.AddProfile<SpeciesStageConfigProfile>();
                cfg.AddProfile<SpeciesThresholdProfile>();
            });
        }
    }
}
