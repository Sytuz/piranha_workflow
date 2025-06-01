using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Piranha;
using Piranha.AspNetCore;
using Piranha.Manager.Middleware;
using Piranha.Manager.Services;
using Prometheus;
using System;

namespace Piranha.Manager.Extensions
{
    /// <summary>
    /// Extension methods for configuring observability and telemetry in Piranha CMS.
    /// </summary>
    public static class ObservabilityExtensions
    {
        /// <summary>
        /// Adds observability services to the service collection.
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="serviceName">The service name for tracing</param>
        /// <param name="serviceVersion">The service version</param>
        /// <returns>The service collection</returns>
        public static IServiceCollection AddPiranhaObservability(this IServiceCollection services, 
            string serviceName = "Piranha.CMS", 
            string serviceVersion = "1.0.0")
        {
            // Register telemetry service
            services.AddSingleton<ITelemetryService, TelemetryService>();

            // Configure OpenTelemetry
            services.AddOpenTelemetry()
                .ConfigureResource(resource => 
                    resource.AddService(serviceName, serviceVersion))
                .WithTracing(tracing =>
                {
                    tracing
                        .AddAspNetCoreInstrumentation(options =>
                        {
                            options.RecordException = true;
                            options.Filter = (httpContext) =>
                            {
                                // Filter out health check endpoints and static files
                                var path = httpContext.Request.Path.Value?.ToLowerInvariant();
                                return !(path?.Contains("/health") == true || 
                                        path?.Contains("/favicon") == true ||
                                        path?.Contains("/assets") == true ||
                                        path?.Contains(".css") == true ||
                                        path?.Contains(".js") == true ||
                                        path?.Contains(".png") == true ||
                                        path?.Contains(".jpg") == true);
                            };
                        })
                        .AddHttpClientInstrumentation()
                        .AddSource("Piranha.Workflow")
                        .AddJaegerExporter(options =>
                        {
                            options.AgentHost = "localhost";
                            options.AgentPort = 6831;
                        });
                })
                .WithMetrics(metrics =>
                {
                    metrics
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddMeter("Piranha.Workflow")
                        .AddPrometheusExporter();
                });

            return services;
        }

        /// <summary>
        /// Configures the application to use Piranha observability middleware and endpoints.
        /// </summary>
        /// <param name="app">The application builder</param>
        /// <returns>The application builder</returns>
        public static IApplicationBuilder UsePiranhaObservability(this IApplicationBuilder app)
        {
            // Add telemetry middleware early in the pipeline
            app.UseMiddleware<TelemetryMiddleware>();

            // Add Prometheus metrics endpoint
            app.UseMetricServer("/metrics");

            // Add OpenTelemetry Prometheus scraping endpoint
            app.UseOpenTelemetryPrometheusScrapingEndpoint();

            return app;
        }

        /// <summary>
        /// Adds observability to a Piranha service builder.
        /// </summary>
        /// <param name="serviceBuilder">The Piranha service builder</param>
        /// <param name="serviceName">The service name for tracing</param>
        /// <param name="serviceVersion">The service version</param>
        /// <returns>The Piranha service builder</returns>
        public static PiranhaServiceBuilder AddObservability(this PiranhaServiceBuilder serviceBuilder,
            string serviceName = "Piranha.CMS",
            string serviceVersion = "1.0.0")
        {
            serviceBuilder.Services.AddPiranhaObservability(serviceName, serviceVersion);
            return serviceBuilder;
        }

        /// <summary>
        /// Adds observability middleware to a Piranha application builder.
        /// </summary>
        /// <param name="builder">The Piranha application builder</param>
        /// <returns>The Piranha application builder</returns>
        public static PiranhaApplicationBuilder UseObservability(this PiranhaApplicationBuilder builder)
        {
            builder.Builder.UsePiranhaObservability();
            return builder;
        }
    }
}
