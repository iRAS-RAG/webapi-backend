using System.Text;
using System.Threading.RateLimiting;
using Hangfire;
using Hangfire.PostgreSql;
using IRasRag.API.Hubs;
using IRasRag.API.Utils;
using IRasRag.Application.Common.Interfaces.Auth;
using IRasRag.Application.Common.Interfaces.Realtime;
using IRasRag.Infrastructure.DI;
using IRasRag.Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace IRasRag.API.DI
{
    public static class DependencyInjection
    {
        public static void AddApiServices(
            this IServiceCollection services,
            IConfiguration config,
            IHostEnvironment env
        )
        {
            services.AddAuthorization();
            services.AddJwtAuthencation(config);
            services.AddSwagger();
            services.AddCors();
            services.AddCustomRateLimiter();
            services.AddHangfireSetup(config, env);
            services.AddHttpContextAccessor();
            services.AddScoped<HttpContextUtils>();
            services.AddScoped<ICurrentUserAccessor>(sp => sp.GetRequiredService<HttpContextUtils>());
            services.AddSignalR();
            services.AddSingleton<ILiveDataNotifier, SignalRLiveDataNotifier>();
            services.AddHostedService<TelemetryPushWorker>();
        }

        public static void AddJwtAuthencation(
            this IServiceCollection services,
            IConfiguration config
        )
        {
            var jwtSettings = config.GetSection("JwtSettings").Get<JwtSettings>();
            jwtSettings?.ValidateSettings();
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtSettings.SecretKey)
                        ),
                        ClockSkew = TimeSpan.Zero,
                    };

                    // SignalR sends the JWT via query string because WebSockets don't support headers
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;
                            if (
                                !string.IsNullOrEmpty(accessToken)
                                && path.StartsWithSegments("/hubs/telemetry")
                            )
                            {
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        },
                    };
                });
        }

        public static void AddSwagger(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "IRasRag API", Version = "v1" });
                c.AddSecurityDefinition(
                    "Bearer",
                    new OpenApiSecurityScheme
                    {
                        Description =
                            "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                    }
                );

                c.AddSecurityRequirement(
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer",
                                },
                            },
                            Array.Empty<string>()
                        },
                    }
                );
            });
        }

        public static void AddCors(this IServiceCollection services)
        {
            services.AddCors(options =>
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
        }

        public static void AddCustomRateLimiter(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(
                    httpContext =>
                        RateLimitPartition.GetFixedWindowLimiter(
                            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString()
                                ?? "global",
                            factory: partition => new FixedWindowRateLimiterOptions
                            {
                                PermitLimit = 100,
                                Window = TimeSpan.FromMinutes(1),
                                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                                QueueLimit = 0,
                            }
                        )
                );

                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                options.OnRejected = async (context, cancellationToken) =>
                {
                    context.HttpContext.Response.ContentType = "application/json";

                    var response = new { message = "Quá nhiều yêu cầu. Vui lòng thử lại sau." };
                    await context.HttpContext.Response.WriteAsJsonAsync(
                        response,
                        cancellationToken
                    );
                };
            });
        }

        public static void AddHangfireSetup(
            this IServiceCollection services,
            IConfiguration config,
            IHostEnvironment env
        )
        {
            var connectionString = ConnectionStringResolver.Resolve(config, env);

            services.AddHangfire(configuration =>
                configuration
                    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                    .UseFilter(
                        new AutomaticRetryAttribute
                        {
                            Attempts = 5,
                            OnAttemptsExceeded = AttemptsExceededAction.Fail,
                        }
                    )
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UsePostgreSqlStorage(
                        c => c.UseNpgsqlConnection(connectionString),
                        new PostgreSqlStorageOptions
                        {
                            SchemaName = "hangfire",
                            // Create the schema if it doesn't exist
                            PrepareSchemaIfNecessary = true,
                            QueuePollInterval = TimeSpan.FromSeconds(15),
                        }
                    )
            );
            services.AddHangfireServer();
        }
    }
}
