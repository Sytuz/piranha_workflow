using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Piranha.Manager.Services;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Piranha.Manager.Middleware
{
    /// <summary>
    /// Middleware to capture HTTP request metrics for observability
    /// </summary>
    public class TelemetryMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ITelemetryService _telemetryService;
        private readonly ILogger<TelemetryMiddleware> _logger;

        public TelemetryMiddleware(RequestDelegate next, ITelemetryService telemetryService, ILogger<TelemetryMiddleware> logger)
        {
            _next = next;
            _telemetryService = telemetryService;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var activitySource = _telemetryService.GetActivitySource();
            
            using var activity = activitySource.StartActivity($"{context.Request.Method} {context.Request.Path}");
            
            try
            {
                // Add trace context to activity
                activity?.SetTag("http.method", context.Request.Method);
                activity?.SetTag("http.url", context.Request.Path);
                activity?.SetTag("http.scheme", context.Request.Scheme);
                activity?.SetTag("http.host", context.Request.Host.ToString());
                
                // Add user context if available
                if (context.User?.Identity?.IsAuthenticated == true)
                {
                    activity?.SetTag("user.id", context.User.Identity.Name);
                }

                await _next(context);

                stopwatch.Stop();

                // Record successful request
                activity?.SetTag("http.status_code", context.Response.StatusCode);
                activity?.SetStatus(context.Response.StatusCode >= 400 ? ActivityStatusCode.Error : ActivityStatusCode.Ok);

                _telemetryService.RecordHttpRequest(
                    context.Request.Method,
                    GetNormalizedPath(context.Request.Path),
                    context.Response.StatusCode,
                    stopwatch.Elapsed
                );
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                activity?.SetTag("exception.type", ex.GetType().Name);
                activity?.SetTag("exception.message", ex.Message);

                _telemetryService.RecordHttpRequest(
                    context.Request.Method,
                    GetNormalizedPath(context.Request.Path),
                    500,
                    stopwatch.Elapsed
                );

                _logger.LogError(ex, "Error occurred during request processing: {Method} {Path}", 
                    context.Request.Method, context.Request.Path);
                
                throw;
            }
        }

        private static string GetNormalizedPath(PathString path)
        {
            // Normalize paths to avoid high cardinality metrics
            var pathValue = path.Value?.ToLowerInvariant() ?? "/";
            
            // Replace dynamic segments with placeholders
            if (pathValue.Contains("/manager/workflow/"))
            {
                // Replace workflow IDs with placeholder
                pathValue = System.Text.RegularExpressions.Regex.Replace(
                    pathValue, 
                    @"/manager/workflow/[0-9a-f-]+", 
                    "/manager/workflow/{id}"
                );
            }
            
            if (pathValue.Contains("/api/workflow/"))
            {
                pathValue = System.Text.RegularExpressions.Regex.Replace(
                    pathValue, 
                    @"/api/workflow/[0-9a-f-]+", 
                    "/api/workflow/{id}"
                );
            }

            return pathValue;
        }
    }
}
