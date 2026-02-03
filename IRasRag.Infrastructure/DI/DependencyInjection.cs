using IRasRag.Application.Common.Interfaces.Auth;
using IRasRag.Application.Common.Interfaces.Email;
using IRasRag.Application.Common.Interfaces.Persistence;
using IRasRag.Infrastructure.Persistence;
using IRasRag.Infrastructure.Repositories;
using IRasRag.Infrastructure.Services.Auth;
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
            services.AddScoped<IHashingService, BCryptHashingService>();
            services.AddScoped<IEmailService, EmailService>();
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
            var connectionString = "";
            if (env.IsDevelopment())
            {
                connectionString = config.GetConnectionString("DefaultConnection");
            }
            else if (env.IsProduction())
            {
                connectionString = config.GetConnectionString("NeonConnection");
            }
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention();
            });
        }
    }
}
