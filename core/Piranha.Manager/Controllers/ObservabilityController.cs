using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Piranha.Manager.Services;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.Diagnostics;

namespace Piranha.Manager.Controllers
{
    /// <summary>
    /// Observability controller for Manager area - provides comprehensive monitoring dashboard.
    /// </summary>
    [Area("Manager")]
    [Route("manager/observability")]
    [AllowAnonymous]
    public class ObservabilityController : Controller
    {
        private readonly ITelemetryService _telemetryService;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="telemetryService">The telemetry service</param>
        public ObservabilityController(ITelemetryService telemetryService)
        {
            _telemetryService = telemetryService;
        }

        /// <summary>
        /// Gets the observability dashboard page.
        /// </summary>
        /// <returns>The dashboard view</returns>
        [HttpGet("")]
        public IActionResult Index()
        {
            // Return the Manager view for observability
            return View();
        }

        /// <summary>
        /// Gets the health status of the application and observability stack.
        /// </summary>
        /// <returns>Health status information</returns>
        [HttpGet("api/health")]
        [Produces("application/json")]
        public async Task<IActionResult> GetHealth()
        {
            try
            {
                var healthData = new
                {
                    status = "healthy",
                    timestamp = DateTime.UtcNow,
                    services = new
                    {
                        application = "healthy",
                        prometheus = await CheckServiceHealth("http://localhost:9090/-/healthy"),
                        grafana = await CheckServiceHealth("http://localhost:3000/api/health"),
                        jaeger = await CheckServiceHealth("http://localhost:16686/")
                    },
                    metrics = new
                    {
                        totalRequests = GetMetricValue("piranha_workflow_requests_total"),
                        activeItems = GetMetricValue("piranha_workflow_active_items"),
                        pendingItems = GetMetricValue("piranha_workflow_pending_items")
                    }
                };

                return Ok(healthData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Gets metrics summary information.
        /// </summary>
        /// <returns>Metrics summary</returns>
        [HttpGet("api/metrics/summary")]
        [Produces("application/json")]
        public IActionResult GetMetricsSummary()
        {
            try
            {
                var metricsData = new
                {
                    timestamp = DateTime.UtcNow,
                    totalRequests = GetMetricValue("piranha_workflow_requests_total"),
                    activeWorkflowItems = GetMetricValue("piranha_workflow_active_items"),
                    pendingWorkflowItems = GetMetricValue("piranha_workflow_pending_items"),
                    availableMetrics = new[]
                    {
                        "piranha_workflow_operations_total",
                        "piranha_workflow_operation_duration_seconds", 
                        "piranha_workflow_active_items",
                        "piranha_workflow_pending_items",
                        "piranha_workflow_requests_total",
                        "piranha_workflow_request_duration_seconds"
                    },
                    endpoints = new
                    {
                        prometheus = "http://localhost:9090",
                        grafana = "http://localhost:3000",
                        jaeger = "http://localhost:16686",
                        metrics = "/metrics"
                    }
                };

                return Ok(metricsData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Gets tracing information.
        /// </summary>
        /// <returns>Tracing information</returns>
        [HttpGet("api/tracing/info")]
        [Produces("application/json")]
        public IActionResult GetTracingInfo()
        {
            try
            {
                var tracingData = new
                {
                    enabled = true,
                    jaegerUrl = "http://localhost:16686",
                    serviceName = "Piranha.RazorWeb",
                    activeTraces = GetActiveTracesCount(),
                    timestamp = DateTime.UtcNow
                };

                return Ok(tracingData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Test endpoint for creating sample traces.
        /// </summary>
        /// <returns>Test trace result</returns>
        [HttpPost("test-trace")]
        [Produces("application/json")]
        public IActionResult CreateTestTrace()
        {
            try
            {
                using var activity = new Activity("test-operation");
                activity.Start();
                
                activity.SetTag("operation.type", "test");
                activity.SetTag("user.id", "test-user");
                
                // Simulate some work
                System.Threading.Thread.Sleep(100);
                
                var result = new
                {
                    traceId = activity.TraceId.ToString(),
                    spanId = activity.SpanId.ToString(),
                    operation = "test-operation",
                    duration = "100ms",
                    timestamp = DateTime.UtcNow
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        private async Task<string> CheckServiceHealth(string url)
        {
            try
            {
                using var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(5);
                var response = await client.GetAsync(url);
                return response.IsSuccessStatusCode ? "healthy" : "unhealthy";
            }
            catch
            {
                return "unreachable";
            }
        }

        private string GetMetricValue(string metricName)
        {
            try
            {
                // This would normally query your metrics store
                // For now, return a placeholder
                return "N/A";
            }
            catch
            {
                return "error";
            }
        }

        private int GetActiveTracesCount()
        {
            try
            {
                // This would normally query your tracing system
                // For now, return a sample value
                return new Random().Next(1, 10);
            }
            catch
            {
                return 0;
            }
        }
    }
}
