using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Piranha.Manager.Services;
using Piranha.Security;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Piranha.Manager.Controllers
{
    /// <summary>
    /// API controller for observability and telemetry data.
    /// </summary>
    [Area("Manager")]
    [Route("manager/api/observability")]
    [AllowAnonymous] // Allow anonymous access for observability monitoring
    [ApiController]
    public class ObservabilityApiController : Controller
    {
        private readonly ITelemetryService _telemetryService;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="telemetryService">The telemetry service</param>
        public ObservabilityApiController(ITelemetryService telemetryService)
        {
            _telemetryService = telemetryService;
        }

        /// <summary>
        /// Gets system health status.
        /// </summary>
        /// <returns>Health status information</returns>
        [Route("health")]
        [HttpGet]
        public IActionResult GetHealth()
        {
            using var activity = _telemetryService.GetActivitySource().StartActivity("GetSystemHealth");
            
            try
            {
                var health = new
                {
                    Status = "Healthy",
                    Timestamp = DateTime.UtcNow,
                    Services = new
                    {
                        Prometheus = CheckPrometheusHealth(),
                        Jaeger = CheckJaegerHealth(),
                        Database = CheckDatabaseHealth()
                    },
                    SystemInfo = new
                    {
                        Environment.MachineName,
                        Environment.ProcessorCount,
                        Environment.OSVersion,
                        WorkingSet = Environment.WorkingSet,
                        TickCount = Environment.TickCount64
                    }
                };

                activity?.SetTag("health.status", "healthy");
                return Ok(health);
            }
            catch (Exception ex)
            {
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                return StatusCode(500, new { Status = "Unhealthy", Error = ex.Message });
            }
        }

        /// <summary>
        /// Gets current metrics summary.
        /// </summary>
        /// <returns>Metrics summary</returns>
        [Route("metrics/summary")]
        [HttpGet]
        public IActionResult GetMetricsSummary()
        {
            using var activity = _telemetryService.GetActivitySource().StartActivity("GetMetricsSummary");
            
            try
            {
                var summary = new
                {
                    Timestamp = DateTime.UtcNow,
                    PrometheusUrl = "http://localhost:9090",
                    GrafanaUrl = "http://localhost:3000",
                    JaegerUrl = "http://localhost:16686",
                    MetricsEndpoint = "/metrics",
                    AvailableMetrics = new[]
                    {
                        "piranha_workflow_operations_total",
                        "piranha_workflow_operation_duration_seconds",
                        "piranha_workflow_active_items",
                        "piranha_workflow_pending_items",
                        "piranha_workflow_requests_total",
                        "piranha_workflow_request_duration_seconds"
                    }
                };

                activity?.SetTag("metrics.count", summary.AvailableMetrics.Length);
                return Ok(summary);
            }
            catch (Exception ex)
            {
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        /// <summary>
        /// Gets tracing information.
        /// </summary>
        /// <returns>Tracing information</returns>
        [Route("tracing/info")]
        [HttpGet]
        public IActionResult GetTracingInfo()
        {
            using var activity = _telemetryService.GetActivitySource().StartActivity("GetTracingInfo");
            
            try
            {
                var tracingInfo = new
                {
                    ActivitySource = "Piranha.Workflow",
                    JaegerEndpoint = "http://localhost:16686",
                    CurrentTraceId = Activity.Current?.TraceId.ToString(),
                    CurrentSpanId = Activity.Current?.SpanId.ToString(),
                    SamplingRate = 1.0, // 100% sampling for demo
                    AvailableServices = new[]
                    {
                        "piranha-workflow-api",
                        "piranha-manager",
                        "piranha-cms"
                    }
                };

                activity?.SetTag("trace.id", tracingInfo.CurrentTraceId);
                return Ok(tracingInfo);
            }
            catch (Exception ex)
            {
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        /// <summary>
        /// Triggers a test trace for demonstration purposes.
        /// </summary>
        /// <returns>Test trace information</returns>
        [Route("test-trace")]
        [HttpPost]
        public async Task<IActionResult> CreateTestTrace()
        {
            using var activity = _telemetryService.GetActivitySource().StartActivity("TestTrace");
            
            try
            {
                activity?.SetTag("test.type", "manual");
                activity?.SetTag("test.user", User?.Identity?.Name ?? "anonymous");

                // Simulate some work with nested activities
                using var childActivity1 = _telemetryService.GetActivitySource().StartActivity("TestChildOperation1");
                childActivity1?.SetTag("operation.type", "database_query");
                await Task.Delay(50); // Simulate DB query
                
                using var childActivity2 = _telemetryService.GetActivitySource().StartActivity("TestChildOperation2");
                childActivity2?.SetTag("operation.type", "external_api_call");
                await Task.Delay(100); // Simulate API call

                _telemetryService.RecordWorkflowOperation("test_trace", "success", TimeSpan.FromMilliseconds(150));

                var result = new
                {
                    TraceId = activity?.TraceId.ToString(),
                    SpanId = activity?.SpanId.ToString(),
                    Message = "Test trace created successfully",
                    JaegerUrl = $"http://localhost:16686/trace/{activity?.TraceId}",
                    Duration = "~150ms"
                };

                activity?.SetStatus(ActivityStatusCode.Ok);
                return Ok(result);
            }
            catch (Exception ex)
            {
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        private static string CheckPrometheusHealth()
        {
            try
            {
                // In a real implementation, you would make an HTTP call to Prometheus
                return "Connected";
            }
            catch
            {
                return "Disconnected";
            }
        }

        private static string CheckJaegerHealth()
        {
            try
            {
                // In a real implementation, you would check Jaeger availability
                return "Connected";
            }
            catch
            {
                return "Disconnected";
            }
        }

        private static string CheckDatabaseHealth()
        {
            try
            {
                // In a real implementation, you would check database connectivity
                return "Connected";
            }
            catch
            {
                return "Disconnected";
            }
        }
    }
}
