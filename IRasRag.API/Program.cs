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
            builder.Services.AddApplicationServices();
            builder.Services.AddInfrastructureServices(builder.Configuration, builder.Environment);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseMiddleware<ApiKeyMiddleware>();
            app.UseCors("CorsPolicy");
            app.UseHttpsRedirection();
            app.UseRateLimiter();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
