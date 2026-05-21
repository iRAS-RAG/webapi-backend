using IRasRag.Application.Common.Interfaces.Auth;
using IRasRag.Application.Common.Interfaces.BackgroundJobs;
using IRasRag.Application.Common.Interfaces.Cloudinary;
using IRasRag.Application.Common.Interfaces.Email;
using IRasRag.Application.Common.Interfaces.FileExtraction;
using IRasRag.Application.Common.Interfaces.FileExtractor;
using IRasRag.Application.Common.Interfaces.FileValidator;
using IRasRag.Application.Common.Interfaces.Mqtt;
using IRasRag.Application.Common.Interfaces.Persistence;
using IRasRag.Application.Common.Interfaces.Persistence.Repositories;
using IRasRag.Application.Common.Interfaces.Telemetry;
using IRasRag.Application.Common.Settings;
using IRasRag.Application.Common.Utils;
using IRasRag.Infrastructure.Persistence;
using IRasRag.Infrastructure.Repositories;
using IRasRag.Infrastructure.Services.Auth;
using IRasRag.Infrastructure.Services.BackgroundJobs;
using IRasRag.Infrastructure.Services.CloudFileStorage;
using IRasRag.Infrastructure.Services.Email;
using IRasRag.Infrastructure.Services.FileExtractors;
using IRasRag.Infrastructure.Services.FileValidator;
using IRasRag.Infrastructure.Services.Mqtt;
using IRasRag.Infrastructure.Services.Telemetry;
using IRasRag.Infrastructure.Services.TextExtractors;
using IRasRag.Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MQTTnet;

namespace IRasRag.Infrastructure.DI
{
    public static class DependencyInjection
    {
        public static void AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration config,
            IHostEnvironment env
        )
        {
            services.AddRepositories();
            services.AddServices();
            services.AddConnectionString(config, env);
            services.AddSettings(config);
            services.AddSingleton<IMqttClient>(_ => new MqttClientFactory().CreateMqttClient());
            services.AddSingleton<IMqttPublishService, MqttPublishService>();
            services.AddHostedService<MqttBackgroundService>();
        }

        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IAlertRepository, AlertRepository>();
        }

        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IHashingService, HashingService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IBackgroundJobService, HangfireBackgroundJobService>();

            // Telemetry
            services.AddSingleton<TelemetryLogBatchWriter>();
            services.AddSingleton<ITelemetryLogBatchWriter>(sp =>
                sp.GetRequiredService<TelemetryLogBatchWriter>()
            );
            services.AddHostedService(sp => sp.GetRequiredService<TelemetryLogBatchWriter>());
            services.AddScoped<ITelemetryCacheService, TelemetryCacheService>();
            services.AddScoped<ITelemetryDispatchService, TelemetryDispatchService>();
            services.AddSingleton<ILatestTelemetryCacheService, LatestTelemetryCacheService>();
            services.AddSingleton<IAlertStateCacheService, AlertStateCacheService>();
            services.AddScoped<IAlertStateEvaluator, AlertStateEvaluator>();

            // File processing
            services.AddScoped<IFileContentValidator, FileContentValidator>();
            services.AddScoped<ICloudFileStorageService, CloudnaryService>();
            services.AddScoped<IFileTextExtractorResolver, FileTextExtractorResolver>();
            services.AddScoped<IFileTextExtractor, PdfTextExtractor>();
            services.AddScoped<IFileTextExtractor, DocxTextExtractor>();
        }

        public static void AddSettings(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<EmailSettings>(config.GetSection("Email"));
            services.Configure<JwtSettings>(config.GetSection("JwtSettings"));
            services.Configure<CloudinarySettings>(config.GetSection("Cloudinary"));
            services.Configure<MqttSettings>(config.GetSection("Mqtt"));
            services.Configure<LogBatchWriterSettings>(config.GetSection("LogBatchWriter"));
            services.Configure<AlertSettings>(config.GetSection("AlertSettings"));

            var mqttSettings = config.GetSection("Mqtt").Get<MqttSettings>();
            mqttSettings?.ValidateSettings();
        }

        public static void AddConnectionString(
            this IServiceCollection services,
            IConfiguration config,
            IHostEnvironment env
        )
        {
            var connectionString = ConnectionStringResolver.Resolve(config, env);
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention();
            });
        }
    }

    public static class ConnectionStringResolver
    {
        public static string Resolve(IConfiguration config, IHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                return config.GetConnectionString("DefaultConnection");
            }

            if (env.IsProduction())
            {
                return config.GetConnectionString("NeonConnection");
            }

            throw new InvalidOperationException("Unsupported environment");
        }
    }
}
