using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RazorWeb.Controllers
{
    /// <summary>
    /// Direct controller for observability dashboard.
    /// </summary>
    [Route("observability")]
    [AllowAnonymous]
    public class ObservabilityController : Controller
    {
        /// <summary>
        /// Gets the observability dashboard page.
        /// </summary>
        /// <returns>The dashboard view</returns>
        public IActionResult Index()
        {
            var html = @"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Piranha CMS - Observability Dashboard</title>
    <link href=""https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/css/bootstrap.min.css"" rel=""stylesheet"">
    <script src=""https://unpkg.com/vue@3/dist/vue.global.js""></script>
    <script src=""https://cdn.jsdelivr.net/npm/chart.js""></script>
    <style>
        .metric-card {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            border-radius: 10px;
            padding: 20px;
            margin-bottom: 20px;
        }
        .metric-value {
            font-size: 2rem;
            font-weight: bold;
        }
        .metric-label {
            font-size: 0.9rem;
            opacity: 0.8;
        }
        .status-up {
            color: #28a745;
        }
        .status-down {
            color: #dc3545;
        }
        .nav-tabs .nav-link.active {
            background-color: #667eea;
            border-color: #667eea;
            color: white;
        }
        .iframe-container {
            border: 1px solid #dee2e6;
            border-radius: 5px;
            overflow: hidden;
        }
    </style>
</head>
<body>
    <div id=""app"" class=""container-fluid mt-4"">
        <h1 class=""mb-4"">
            <i class=""bi bi-speedometer2""></i>
            Piranha CMS Observability Dashboard
        </h1>
        
        <!-- Status Overview -->
        <div class=""row mb-4"">
            <div class=""col-md-3"">
                <div class=""metric-card"">
                    <div class=""metric-value"">{{ healthStatus.status }}</div>
                    <div class=""metric-label"">System Status</div>
                </div>
            </div>
            <div class=""col-md-3"">
                <div class=""metric-card"">
                    <div class=""metric-value"">{{ healthStatus.uptime }}</div>
                    <div class=""metric-label"">Uptime</div>
                </div>
            </div>
            <div class=""col-md-3"">
                <div class=""metric-card"">
                    <div class=""metric-value"">{{ metrics.total_requests || 0 }}</div>
                    <div class=""metric-label"">Total Requests</div>
                </div>
            </div>
            <div class=""col-md-3"">
                <div class=""metric-card"">
                    <div class=""metric-value"">{{ metrics.active_workflows || 0 }}</div>
                    <div class=""metric-label"">Active Workflows</div>
                </div>
            </div>
        </div>

        <!-- Navigation Tabs -->
        <ul class=""nav nav-tabs"" id=""observabilityTabs"" role=""tablist"">
            <li class=""nav-item"" role=""presentation"">
                <button class=""nav-link active"" id=""metrics-tab"" data-bs-toggle=""tab"" data-bs-target=""#metrics"" type=""button"" role=""tab"">
                    Metrics (Prometheus)
                </button>
            </li>
            <li class=""nav-item"" role=""presentation"">
                <button class=""nav-link"" id=""grafana-tab"" data-bs-toggle=""tab"" data-bs-target=""#grafana"" type=""button"" role=""tab"">
                    Grafana Dashboards
                </button>
            </li>
            <li class=""nav-item"" role=""presentation"">
                <button class=""nav-link"" id=""traces-tab"" data-bs-toggle=""tab"" data-bs-target=""#traces"" type=""button"" role=""tab"">
                    Traces (Jaeger)
                </button>
            </li>
            <li class=""nav-item"" role=""presentation"">
                <button class=""nav-link"" id=""health-tab"" data-bs-toggle=""tab"" data-bs-target=""#health"" type=""button"" role=""tab"">
                    Health Checks
                </button>
            </li>
        </ul>

        <!-- Tab Content -->
        <div class=""tab-content mt-4"" id=""observabilityTabContent"">
            <!-- Metrics Tab -->
            <div class=""tab-pane fade show active"" id=""metrics"" role=""tabpanel"">
                <div class=""row"">
                    <div class=""col-12"">
                        <h4>Prometheus Metrics</h4>
                        <div class=""iframe-container"">
                            <iframe src=""http://localhost:9090/targets"" 
                                    width=""100%"" 
                                    height=""600""
                                    frameborder=""0"">
                            </iframe>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Grafana Tab -->
            <div class=""tab-pane fade"" id=""grafana"" role=""tabpanel"">
                <div class=""row"">
                    <div class=""col-12"">
                        <h4>Grafana Dashboards</h4>
                        <div class=""iframe-container"">
                            <iframe src=""http://localhost:3000"" 
                                    width=""100%"" 
                                    height=""600""
                                    frameborder=""0"">
                            </iframe>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Traces Tab -->
            <div class=""tab-pane fade"" id=""traces"" role=""tabpanel"">
                <div class=""row"">
                    <div class=""col-12"">
                        <h4>Distributed Tracing</h4>
                        <div class=""iframe-container"">
                            <iframe src=""http://localhost:16686"" 
                                    width=""100%"" 
                                    height=""600""
                                    frameborder=""0"">
                            </iframe>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Health Tab -->
            <div class=""tab-pane fade"" id=""health"" role=""tabpanel"">
                <div class=""row"">
                    <div class=""col-12"">
                        <h4>System Health</h4>
                        <div class=""card"">
                            <div class=""card-body"">
                                <h5>Service Status</h5>
                                <ul class=""list-unstyled"">
                                    <li v-for=""service in healthStatus.services"" :key=""service.name"">
                                        <span :class=""service.status === 'Healthy' ? 'status-up' : 'status-down'"">
                                            {{ service.status === 'Healthy' ? '✓' : '✗' }}
                                        </span>
                                        {{ service.name }}: {{ service.status }}
                                        <small class=""text-muted"">{{ service.description }}</small>
                                    </li>
                                </ul>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <script src=""https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/js/bootstrap.bundle.min.js""></script>
    <script>
        const { createApp } = Vue;
        
        createApp({
            data() {
                return {
                    healthStatus: {
                        status: 'Loading...',
                        uptime: 'Loading...',
                        services: []
                    },
                    metrics: {
                        total_requests: 0,
                        active_workflows: 0
                    }
                };
            },
            async mounted() {
                await this.loadHealthStatus();
                await this.loadMetrics();
                
                // Auto-refresh every 30 seconds
                setInterval(() => {
                    this.loadHealthStatus();
                    this.loadMetrics();
                }, 30000);
            },
            methods: {
                async loadHealthStatus() {
                    try {
                        const response = await fetch('/health');
                        if (response.ok) {
                            const data = await response.json();
                            this.healthStatus = {
                                status: data.status || 'Healthy',
                                uptime: this.formatUptime(data.totalDuration),
                                services: data.entries ? Object.entries(data.entries).map(([name, entry]) => ({
                                    name,
                                    status: entry.status,
                                    description: entry.description || ''
                                })) : []
                            };
                        }
                    } catch (error) {
                        console.error('Failed to load health status:', error);
                        this.healthStatus.status = 'Error';
                    }
                },
                async loadMetrics() {
                    try {
                        const response = await fetch('/metrics');
                        if (response.ok) {
                            const metricsText = await response.text();
                            this.parseMetrics(metricsText);
                        }
                    } catch (error) {
                        console.error('Failed to load metrics:', error);
                    }
                },
                parseMetrics(metricsText) {
                    const lines = metricsText.split('\n');
                    let totalRequests = 0;
                    let activeWorkflows = 0;
                    
                    lines.forEach(line => {
                        if (line.startsWith('piranha_workflow_requests_total{')) {
                            const match = line.match(/}\s+(\d+)/);
                            if (match) {
                                totalRequests += parseInt(match[1]);
                            }
                        }
                        if (line.startsWith('piranha_workflow_active_items')) {
                            const match = line.match(/piranha_workflow_active_items\s+(\d+)/);
                            if (match) {
                                activeWorkflows = parseInt(match[1]);
                            }
                        }
                    });
                    
                    this.metrics = {
                        total_requests: totalRequests,
                        active_workflows: activeWorkflows
                    };
                },
                formatUptime(duration) {
                    if (!duration) return 'Unknown';
                    const match = duration.match(/(\d+):(\d+):(\d+)/);
                    if (match) {
                        const [, hours, minutes, seconds] = match;
                        return `${hours}h ${minutes}m ${seconds}s`;
                    }
                    return duration;
                }
            }
        }).mount('#app');
    </script>
</body>
</html>";
            
            return Content(html, "text/html");
        }
    }
}
