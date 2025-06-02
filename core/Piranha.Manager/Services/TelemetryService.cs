using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using Microsoft.Extensions.Logging;
using Prometheus;

namespace Piranha.Manager.Services
{
    /// <summary>
    /// Service for collecting telemetry metrics for workflow operations
    /// </summary>
    public interface ITelemetryService
    {
        void RecordWorkflowOperation(string operationType, string status, TimeSpan duration);
        void RecordActiveWorkflowItems(int count);
        void RecordPendingWorkflowItems(int count);
        void RecordHttpRequest(string method, string endpoint, int statusCode, TimeSpan duration);
        void IncrementWorkflowOperationCounter(string operationType);
        ActivitySource GetActivitySource();
    }

    public class TelemetryService : ITelemetryService
    {
        private readonly ILogger<TelemetryService> _logger;
        private readonly Meter _meter;
        private readonly ActivitySource _activitySource;
        
        // Prometheus metrics
        private readonly Counter _workflowOperationsTotal;
        private readonly Histogram _workflowOperationDuration;
        private readonly Gauge _activeWorkflowItems;
        private readonly Gauge _pendingWorkflowItems;
        private readonly Counter _httpRequestsTotal;
        private readonly Histogram _httpRequestDuration;

        // OpenTelemetry metrics
        private readonly Counter<long> _operationCounter;
        private readonly Histogram<double> _operationDurationHistogram;

        private int _currentActiveItems = 0;
        private int _currentPendingItems = 0;

        public TelemetryService(ILogger<TelemetryService> logger)
        {
            _logger = logger;
            _meter = new Meter("Piranha.Workflow", "1.0.0");
            _activitySource = new ActivitySource("Piranha.Workflow");

            // Initialize Prometheus metrics
            _workflowOperationsTotal = Metrics
                .CreateCounter("piranha_workflow_operations_total", "Total number of workflow operations", 
                new string[] { "operation_type", "status" });

            _workflowOperationDuration = Metrics
                .CreateHistogram("piranha_workflow_operation_duration_seconds", "Duration of workflow operations in seconds",
                new string[] { "operation_type" });

            _activeWorkflowItems = Metrics
                .CreateGauge("piranha_workflow_active_items", "Number of active workflow items");

            _pendingWorkflowItems = Metrics
                .CreateGauge("piranha_workflow_pending_items", "Number of pending workflow items");

            _httpRequestsTotal = Metrics
                .CreateCounter("piranha_workflow_requests_total", "Total HTTP requests", 
                new string[] { "method", "endpoint", "status_code" });

            _httpRequestDuration = Metrics
                .CreateHistogram("piranha_workflow_request_duration_seconds", "HTTP request duration in seconds",
                new string[] { "method", "endpoint" });

            // Initialize OpenTelemetry metrics
            _operationCounter = _meter.CreateCounter<long>("workflow.operations.count", "operations", "Total workflow operations");
            _operationDurationHistogram = _meter.CreateHistogram<double>("workflow.operations.duration", "seconds", "Workflow operation duration");
        }

        public void RecordWorkflowOperation(string operationType, string status, TimeSpan duration)
        {
            try
            {
                // Prometheus metrics
                _workflowOperationsTotal.WithLabels(operationType, status).Inc();
                _workflowOperationDuration.WithLabels(operationType).Observe(duration.TotalSeconds);

                // OpenTelemetry metrics
                _operationCounter.Add(1, 
                    new KeyValuePair<string, object>("operation_type", operationType),
                    new KeyValuePair<string, object>("status", status));
                _operationDurationHistogram.Record(duration.TotalSeconds, 
                    new KeyValuePair<string, object>("operation_type", operationType));

                _logger.LogDebug("Recorded workflow operation: {OperationType}, Status: {Status}, Duration: {Duration}ms", 
                    operationType, status, duration.TotalMilliseconds);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to record workflow operation metrics");
            }
        }

        public void RecordActiveWorkflowItems(int count)
        {
            try
            {
                _currentActiveItems = count;
                _activeWorkflowItems.Set(count);
                _logger.LogDebug("Updated active workflow items count: {Count}", count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to record active workflow items");
            }
        }

        public void RecordPendingWorkflowItems(int count)
        {
            try
            {
                _currentPendingItems = count;
                _pendingWorkflowItems.Set(count);
                _logger.LogDebug("Updated pending workflow items count: {Count}", count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to record pending workflow items");
            }
        }

        public void RecordHttpRequest(string method, string endpoint, int statusCode, TimeSpan duration)
        {
            try
            {
                _httpRequestsTotal.WithLabels(method, endpoint, statusCode.ToString()).Inc();
                _httpRequestDuration.WithLabels(method, endpoint).Observe(duration.TotalSeconds);

                _logger.LogDebug("Recorded HTTP request: {Method} {Endpoint}, Status: {StatusCode}, Duration: {Duration}ms", 
                    method, endpoint, statusCode, duration.TotalMilliseconds);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to record HTTP request metrics");
            }
        }

        public void IncrementWorkflowOperationCounter(string operationType)
        {
            try
            {
                _workflowOperationsTotal.WithLabels(operationType, "started").Inc();
                _operationCounter.Add(1, new("operation_type", operationType), new("status", "started"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to increment workflow operation counter");
            }
        }

        public ActivitySource GetActivitySource()
        {
            return _activitySource;
        }

        public void Dispose()
        {
            _meter?.Dispose();
            _activitySource?.Dispose();
        }
    }
}
