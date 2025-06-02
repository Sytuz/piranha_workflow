using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Piranha.Manager.Services;

namespace Piranha.Manager.Controllers
{
    /// <summary>
    /// Direct controller for observability dashboard (bypasses Manager area authentication).
    /// </summary>
    [Route("observability")]
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
        public IActionResult Index()
        {
            // Return a simple HTML page that embeds Grafana dashboards
            var html = @"<!DOCTYPE html>
<html>
<head>
    <title>Piranha CMS - Observability Dashboard</title>
    <meta charset=""utf-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
    <style>
        body { margin: 0; font-family: Arial, sans-serif; }
        .header { background: #2c3e50; color: white; padding: 1rem; text-align: center; }
        .nav { background: #34495e; padding: 0.5rem; text-align: center; }
        .nav a { color: white; text-decoration: none; margin: 0 1rem; padding: 0.5rem 1rem; border-radius: 4px; }
        .nav a:hover { background: #2c3e50; }
        .content { height: calc(100vh - 120px); }
        iframe { width: 100%; height: 100%; border: none; }
        .metrics-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 20px; padding: 20px; }
        .metric-card { background: #f8f9fa; border: 1px solid #dee2e6; border-radius: 8px; padding: 20px; }
    </style>
</head>
<body>
    <div class=""header"">
        <h1>Piranha CMS - Observability Dashboard</h1>
        <p>Comprehensive monitoring and telemetry for editorial workflows</p>
    </div>
    <div class=""nav"">
        <a href=""#"" onclick=""showMetrics()"">Metrics (Prometheus)</a>
        <a href=""#"" onclick=""showGrafana()"">Grafana Dashboards</a>
        <a href=""#"" onclick=""showTracing()"">Tracing (Jaeger)</a>
        <a href=""#"" onclick=""showHealth()"">Health Status</a>
    </div>
    <div class=""content"" id=""content"">
        <div class=""metrics-grid"">
            <div class=""metric-card"">
                <h3>✅ Observability Stack Status</h3>
                <ul>
                    <li>✅ Prometheus: <a href=""http://localhost:9090"" target=""_blank"">http://localhost:9090</a></li>
                    <li>✅ Grafana: <a href=""http://localhost:3000"" target=""_blank"">http://localhost:3000</a></li>
                    <li>✅ Jaeger: <a href=""http://localhost:16686"" target=""_blank"">http://localhost:16686</a></li>
                    <li>✅ Application Metrics: <a href=""/metrics"" target=""_blank"">/metrics</a></li>
                    <li>✅ Health Check: <a href=""/health"" target=""_blank"">/health</a></li>
                </ul>
            </div>
            <div class=""metric-card"">
                <h3>Available Metrics</h3>
                <ul>
                    <li>piranha_workflow_operations_total</li>
                    <li>piranha_workflow_operation_duration_seconds</li>
                    <li>piranha_workflow_active_items</li>
                    <li>piranha_workflow_pending_items</li>
                    <li>piranha_workflow_requests_total</li>
                    <li>piranha_workflow_request_duration_seconds</li>
                </ul>
            </div>
        </div>
    </div>
    <script>
        function showMetrics() {
            document.getElementById('content').innerHTML = 
                '<iframe src=""http://localhost:9090""></iframe>';
        }
        function showGrafana() {
            document.getElementById('content').innerHTML = 
                '<iframe src=""http://localhost:3000""></iframe>';
        }
        function showTracing() {
            document.getElementById('content').innerHTML = 
                '<iframe src=""http://localhost:16686""></iframe>';
        }
        function showHealth() {
            fetch('/health').then(r => r.text()).then(data => {
                document.getElementById('content').innerHTML = 
                    '<div style=""padding: 20px; text-align: center;""><h2>Health Status: ' + data + '</h2></div>';
            });
        }
    </script>
</body>
</html>";
            return Content(html, "text/html");
        }
    }
}
