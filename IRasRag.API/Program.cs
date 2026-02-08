using IRasRag.API.DI;
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
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddApiServices(builder.Configuration);
            builder.Services.AddApplicationServices(builder.Configuration);
            builder.Services.AddInfrastructureServices(builder.Configuration, builder.Environment);

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(
                    "CorsPolicy",
                    builder =>
                    {
                        builder
                            .WithOrigins(
                                "http://localhost:5173",
                                "https://iras-rag.vercel.app",
                                "https://iras-rag-dev.vercel.app"
                            )
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials()
                            .WithExposedHeaders("Content-Disposition", "content-disposition");
                    }
                );
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseCors("CorsPolicy");
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseMiddleware<ApiKeyMiddleware>();

            if (!app.Environment.IsDevelopment())
            {
                app.UseHttpsRedirection();
            }

            app.UseRateLimiter();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
