using System.Text.Json.Serialization;
using Hangfire;
using Hangfire.Common;
using IRasRag.API.DI;
using IRasRag.API.Hubs;
using IRasRag.API.Middlewares;
using IRasRag.Application.Common.Interfaces.Advisory;
using IRasRag.Application.Common.Interfaces.BackgroundJobs;
using IRasRag.Application.DI;
using IRasRag.Infrastructure.DI;
using IRasRag.Infrastructure.Filters;
using IRasRag.Infrastructure.HangFireJobFilters;
using SupervisorMetricsHubType = IRasRag.API.Hubs.SupervisorMetricsHub;

namespace IRasRag.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder
                .Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddApiServices(builder.Configuration, builder.Environment);
            builder.Services.AddApplicationServices(builder.Configuration);
            builder.Services.AddInfrastructureServices(builder.Configuration, builder.Environment);
            builder.Services.AddMemoryCache();

            var app = builder.Build();

            app.Lifetime.ApplicationStarted.Register(() =>
            {
                using var scope = app.Services.CreateScope();
                var jobs = scope.ServiceProvider.GetRequiredService<IBackgroundJobService>();
                jobs.Enqueue<ICatalogSyncJob>(j => j.SyncAllAsync());
            });

            GlobalJobFilters.Filters.Add(
                new DocumentIngestFailedFilter(
                    app.Services.GetRequiredService<IServiceScopeFactory>(),
                    app.Services.GetRequiredService<ILoggerFactory>()
                )
            );
            JobFilterProviders.Providers.Add(new MethodRetryFilterProvider());

            // Configure the HTTP request pipeline.
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseCors("CorsPolicy");
            app.UseMiddleware<ExceptionMiddleware>();

            if (!app.Environment.IsDevelopment())
            {
                app.UseHttpsRedirection();
            }

            //app.UseRateLimiter();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.MapHub<TelemetryHub>("/hubs/telemetry");
            app.MapHub<SupervisorMetricsHubType>("/hubs/supervisor-metrics");

            app.Run();
        }
    }
}
