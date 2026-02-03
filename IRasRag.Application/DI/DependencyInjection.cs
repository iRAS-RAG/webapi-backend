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
            services.AddScoped<IFarmService, FarmService>();
            services.AddScoped<IFeedTypeService, FeedTypeService>();
            services.AddScoped<IFishTankService, FishTankService>();
            services.AddScoped<IGrowthStageService, GrowthStageService>();
            services.AddScoped<ISpeciesService, SpeciesService>();
            services.AddScoped<ISpeciesStageConfigService, SpeciesStageConfigService>();
            services.AddScoped<ISpeciesThresholdService, SpeciesThresholdService>();
            services.AddScoped<IUserService, UserService>();
        }

        public static void AddAutoMapper(this IServiceCollection services, IConfiguration config)
        {
            services.AddAutoMapper(cfg =>
            {
                cfg.LicenseKey = config["AutoMapperKey"];
                cfg.AddProfile<FarmProfile>();
                cfg.AddProfile<FeedTypeProfile>();
                cfg.AddProfile<FishTankProfile>();
                cfg.AddProfile<GrowthStageProfile>();
                cfg.AddProfile<SpeciesProfile>();
                cfg.AddProfile<SpeciesStageConfigProfile>();
                cfg.AddProfile<SpeciesThresholdProfile>();
                cfg.AddProfile<UserProfile>();
            });
        }
    }
}
