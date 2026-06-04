
using System.Text.Json.Serialization;
using Hangfire;
using IRasRag.API.DI;
using IRasRag.API.Hubs;
using IRasRag.API.Middlewares;
using IRasRag.Application.DI;
using IRasRag.Infrastructure.DI;

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

            // 1. C?u h�nh Services
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.Configure<IRasRag.Infrastructure.Settings.CloudinarySettings>(
                builder.Configuration.GetSection("Cloudinary")
            );
            // ??m b?o c�c d?ch v? n�y nh?n ?�ng Configuration v� Environment
            builder.Services.AddApiServices(builder.Configuration, builder.Environment);
            builder.Services.AddApplicationServices(builder.Configuration);
            builder.Services.AddInfrastructureServices(builder.Configuration, builder.Environment);
            builder.Services.AddMemoryCache();

            // 2. C?u h�nh CORS - Cho ph�p Mobile App truy c?p
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", policy =>
                {
                    policy.WithOrigins(
                          "http://localhost:5173",       // React Web
            "http://localhost:8081",       // Expo Web
            "http://192.168.1.6:8081",     // Expo Mobile c?
            "http://192.168.1.6",          // Localhost LAN
            "https://iras-rag.vercel.app", // Prod
            "http://192.168.1.72:8081",    // Expo on physical device
             "http://192.168.1.10:8081",
            "http://10.0.2.2:8081"         // Expo on Android Emulator

                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            var app = builder.Build();

            // 3. C?u h�nh Pipeline (Th? t? r?t quan tr?ng)
            app.UseSwagger();
            app.UseSwaggerUI();

            // CORS ph?i ??t tr??c Authentication/Authorization v� ApiKeyMiddleware
            app.UseCors("CorsPolicy");

            app.UseMiddleware<ExceptionMiddleware>();

            // L?u �: N?u b?t ApiKeyMiddleware, FE ph?i g?i X-Api-Key trong Header
            // app.UseMiddleware<ApiKeyMiddleware>();

            if (!app.Environment.IsDevelopment())
            {
                app.UseHttpsRedirection();
            }

            app.UseRateLimiter();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.MapHub<TelemetryHub>("/hubs/telemetry");
            app.MapHub<AlertHub>("/hubs/alerts");

            app.Run();
        }
    }
}