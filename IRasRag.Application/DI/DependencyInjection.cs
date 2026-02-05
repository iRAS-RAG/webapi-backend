using IRasRag.Application.Common.Interfaces;
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
            services.AddScoped<IAlertService, AlertService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICameraService, CameraService>();
            services.AddScoped<IControlDeviceService, ControlDeviceService>();
            services.AddScoped<IControlDeviceTypeService, ControlDeviceTypeService>();
            services.AddScoped<ICorrectiveActionService, CorrectiveActionService>();
            services.AddScoped<IDocumentService, DocumentService>();
            services.AddScoped<IFarmingBatchService, FarmingBatchService>();
            services.AddScoped<IFarmService, FarmService>();
            services.AddScoped<IFeedingLogService, FeedingLogService>();
            services.AddScoped<IFeedTypeService, FeedTypeService>();
            services.AddScoped<IFishTankService, FishTankService>();
            services.AddScoped<IGrowthStageService, GrowthStageService>();
            services.AddScoped<IJobControlMappingService, JobControlMappingService>();
            services.AddScoped<IJobService, JobService>();
            services.AddScoped<IJobTypeService, JobTypeService>();
            services.AddScoped<IMasterBoardService, MasterBoardService>();
            services.AddScoped<IMortalityLogService, MortalityLogService>();
            services.AddScoped<IRecommendationService, RecommendationService>();
            services.AddScoped<ISensorService, SensorService>();
            services.AddScoped<ISensorTypeService, SensorTypeService>();
            services.AddScoped<ISpeciesService, SpeciesService>();
            services.AddScoped<ISpeciesStageConfigService, SpeciesStageConfigService>();
            services.AddScoped<ISpeciesThresholdService, SpeciesThresholdService>();
            services.AddScoped<IUserFarmService, UserFarmService>();
            services.AddScoped<IUserService, UserService>();
        }

        public static void AddAutoMapper(this IServiceCollection services, IConfiguration config)
        {
            services.AddAutoMapper(cfg =>
            {
                cfg.LicenseKey = config["AutoMapperKey"];
                cfg.AddProfile<AlertProfile>();
                cfg.AddProfile<CameraProfile>();
                cfg.AddProfile<ControlDeviceProfile>();
                cfg.AddProfile<ControlDeviceTypeProfile>();
                cfg.AddProfile<CorrectiveActionProfile>();
                cfg.AddProfile<DocumentProfile>();
                cfg.AddProfile<FarmingBatchProfile>();
                cfg.AddProfile<FarmProfile>();
                cfg.AddProfile<FeedingLogProfile>();
                cfg.AddProfile<FeedTypeProfile>();
                cfg.AddProfile<FishTankProfile>();
                cfg.AddProfile<GrowthStageProfile>();
                cfg.AddProfile<JobControlMappingProfile>();
                cfg.AddProfile<JobProfile>();
                cfg.AddProfile<JobTypeProfile>();
                cfg.AddProfile<MasterBoardProfile>();
                cfg.AddProfile<MortalityLogProfile>();
                cfg.AddProfile<RecommendationProfile>();
                cfg.AddProfile<SensorProfile>();
                cfg.AddProfile<SensorTypeProfile>();
                cfg.AddProfile<SpeciesProfile>();
                cfg.AddProfile<SpeciesStageConfigProfile>();
                cfg.AddProfile<SpeciesThresholdProfile>();
                cfg.AddProfile<UserFarmProfile>();
                cfg.AddProfile<UserProfile>();
            });
        }
    }
}
