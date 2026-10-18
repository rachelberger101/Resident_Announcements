using System.Diagnostics;

namespace RA_MuniAnnouncements.Api.Extensions
{
    /// <summary>
    /// Extension methods for configuring logging and middleware in the application.
    /// </summary>
    public static class LoggingExtensions
    {
        /// <summary>
        /// Configures structured logging with console and file outputs.
        /// </summary>
        public static WebApplicationBuilder AddApplicationLogging(this WebApplicationBuilder builder)
        {
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();

            // Configure log levels
            builder.Logging.SetMinimumLevel(LogLevel.Information);

            if (builder.Environment.IsDevelopment())
            {
                builder.Logging.SetMinimumLevel(LogLevel.Debug);
            }

            // Add structured logging configuration
            builder.Services.AddLogging(configure =>
            {
                configure.AddSimpleConsole(options =>
                {
                    options.IncludeScopes = true;
                    options.SingleLine = true;
                    options.TimestampFormat = "yyyy-MM-dd HH:mm:ss.fff zzz";
                });

                configure.AddDebug();
            });

            return builder;
        }

        /// <summary>
        /// Adds custom middleware for exception handling and request/response logging.
        /// </summary>
        public static WebApplication UseApplicationMiddleware(this WebApplication app)
        {
            // Add global exception handling middleware
            app.UseMiddleware<RA_MuniAnnouncements.Api.Middleware.GlobalExceptionMiddleware>();

            // Add request logging middleware
            app.UseRequestLoggingMiddleware();

            return app;
        }

        /// <summary>
        /// Middleware to log incoming requests and outgoing responses.
        /// </summary>
        private static WebApplication UseRequestLoggingMiddleware(this WebApplication app)
        {
            app.Use(async (context, next) =>
            {
                var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
                var stopwatch = Stopwatch.StartNew();

                var request = context.Request;
                var requestLog = $"? [{request.Method}] {request.Path}{request.QueryString}";

                logger.LogInformation("Request: {RequestLog}", requestLog);

                try
                {
                    await next(context);
                }
                finally
                {
                    stopwatch.Stop();
                    var response = context.Response;
                    var responseLog = $"? [{response.StatusCode}] {request.Path} - {stopwatch.ElapsedMilliseconds}ms";

                    var logLevel = response.StatusCode >= 400 ? LogLevel.Warning : LogLevel.Information;
                    logger.Log(logLevel, "Response: {ResponseLog}", responseLog);
                }
            });

            return app;
        }
    }
}
