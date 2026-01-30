namespace IRasRag.API.Middlewares
{
    public class ApiKeyMiddleware(
        RequestDelegate next,
        ILogger<ApiKeyMiddleware> logger
    )
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<ApiKeyMiddleware> _logger = logger;

        public async Task Invoke(HttpContext context, IConfiguration config)
        {
            try
            {
                var clientKey = context.Request.Headers["x-api-key"].FirstOrDefault();
                var serverKey = config.GetValue<string>("ApiKey");

                _logger.LogInformation($"Server key: {serverKey}");

                if (string.IsNullOrEmpty(serverKey))
                {
                    _logger.LogError("API key is not configured.");
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    return;
                }

                if (clientKey == null || clientKey != serverKey)
                {
                    _logger.LogInformation($"client key: {clientKey}");
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsJsonAsync(new
                    {
                        error = "Không có quyền truy cập."
                    });
                    return;
                }
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception. TraceId: {TraceId}", context.TraceIdentifier);

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(new
                {
                    error = "Xảy ra lỗi không xác định.",
                    traceId = context.TraceIdentifier
                });
            }
        }
    }
}
