using IRasRag.Application.Common.Interfaces.Auth;
using IRasRag.Application.Common.Interfaces.BackgroundJobs;
using IRasRag.Application.Common.Interfaces.Email;
using IRasRag.Application.Common.Interfaces.Persistence;
using IRasRag.Application.Common.Interfaces.Persistence.Repositories;
using IRasRag.Infrastructure.Persistence;
using IRasRag.Infrastructure.Repositories;
using IRasRag.Infrastructure.Services.Auth;
using IRasRag.Infrastructure.Services.BackgroundJobs;
using IRasRag.Infrastructure.Services.Email;
using IRasRag.Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
        }

        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }

        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IHashingService, HashingService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IBackgroundJobService, HangfireBackgroundJobService>();
        }

        public static void AddSettings(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<EmailSettings>(config.GetSection("Email"));
            services.Configure<JwtSettings>(config.GetSection("JwtSettings"));
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
