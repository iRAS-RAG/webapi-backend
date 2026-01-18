using IRasRag.Application.Common.Interfaces;
using IRasRag.Application.Common.Settings;
using IRasRag.Infrastructure.Data;
using IRasRag.Infrastructure.Persistence;
using IRasRag.Infrastructure.Repositories;
using IRasRag.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IRasRag.Infrastructure.DI
{
    public static class DependencyInjection 
    {
        public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration config, IHostEnvironment env)
        {
            services.AddRepositories();
            services.AddServices();
            services.AddConnectionString(config, env);
        }
        
        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }

        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
        }

        public static void AddConnectionString(this IServiceCollection services, IConfiguration config, IHostEnvironment env)
        {
            var connectionString = "";
            if(env.IsDevelopment())
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
