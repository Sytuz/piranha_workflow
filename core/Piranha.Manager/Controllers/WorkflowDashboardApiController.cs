using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Piranha.Manager.Models;
using Piranha.Manager.Services;
using Piranha.Security;
using Piranha.Services;
using Piranha.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Piranha.Manager.Controllers
{
    /// <summary>
    /// Api controller for workflow dashboard management.
    /// </summary>
    [Area("Manager")]
    [Route("manager/api/workflow-dashboard")]
    [Authorize(Policy = Permission.Admin)]
    [ApiController]
    public class WorkflowDashboardApiController : Controller
    {
        private readonly IApi _api;
        private readonly ManagerLocalizer _localizer;
        private readonly IUserResolutionService _userResolutionService;
        private readonly IContentTypeResolutionService _contentTypeResolutionService;
        private readonly ITelemetryService _telemetryService;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <param name="localizer">The manager localizer</param>
        /// <param name="userResolutionService">The user resolution service</param>
        /// <param name="contentTypeResolutionService">The content type resolution service</param>
        /// <param name="telemetryService">The telemetry service</param>
        public WorkflowDashboardApiController(IApi api, ManagerLocalizer localizer, 
            IUserResolutionService userResolutionService, 
            IContentTypeResolutionService contentTypeResolutionService,
            ITelemetryService telemetryService)
        {
            _api = api;
            _localizer = localizer;
            _userResolutionService = userResolutionService;
            _contentTypeResolutionService = contentTypeResolutionService;
            _telemetryService = telemetryService;
        }

        /// <summary>
        /// Gets the dashboard overview data.
        /// </summary>
        /// <returns>The dashboard overview</returns>
        [Route("overview")]
        [HttpGet]
        public async Task<IActionResult> GetOverview()
        {
            var stopwatch = Stopwatch.StartNew();
            using var activity = _telemetryService.GetActivitySource().StartActivity("GetWorkflowOverview");
            
            try
            {
                _telemetryService.IncrementWorkflowOperationCounter("get_overview");
                
                // Get all workflows and change requests
                var workflows = await _api.Workflows.GetAllAsync();
                var changeRequests = await _api.ChangeRequests.GetAllAsync();
                
                // Update workflow metrics
                var totalContentInWorkflow = changeRequests.Count();
                var pendingApproval = changeRequests.Count(cr => 
                    cr.Status == Piranha.Models.ChangeRequestStatus.InReview);
                var recentlyApproved = changeRequests.Count(cr => 
                    cr.Status != Piranha.Models.ChangeRequestStatus.Draft && 
                    cr.LastModified >= DateTime.Now.AddDays(-7));
                var rejected = changeRequests.Count(cr => cr.Status == Piranha.Models.ChangeRequestStatus.Rejected);

                // Record telemetry metrics
                _telemetryService.RecordActiveWorkflowItems(totalContentInWorkflow);
                _telemetryService.RecordPendingWorkflowItems(pendingApproval);

                activity?.SetTag("total_workflows", workflows.Count());
                activity?.SetTag("total_content", totalContentInWorkflow);
                activity?.SetTag("pending_approval", pendingApproval);

                // Calculate stage distribution from actual change requests
                var stageDistribution = new List<WorkflowStageCount>();
                foreach (var workflow in workflows)
                {
                    if (workflow.Stages != null)
                    {
                        foreach (var stage in workflow.Stages)
                        {
                            var count = changeRequests.Count(cr => cr.StageId == stage.Id);
                            stageDistribution.Add(new WorkflowStageCount
                            {
                                StageId = stage.Id,
                                StageName = stage.Title,
                                StageColor = stage.Color,
                                Count = count,
                                WorkflowName = workflow.Title,
                                WorkflowId = workflow.Id
                            });
                        }
                    }
                }

                // Get recent activity from change requests
                var recentActivity = new List<WorkflowActivityItem>();
                var recentChangeRequests = changeRequests
                    .OrderByDescending(cr => cr.LastModified)
                    .Take(10)
                    .ToList();

                // Resolve user names for recent activity
                var userIds = recentChangeRequests.Select(cr => cr.CreatedById).Distinct();
                var userNames = await _userResolutionService.GetUserNamesByIdsAsync(userIds);

                foreach (var cr in recentChangeRequests)
                {
                    var userName = userNames.ContainsKey(cr.CreatedById) ? userNames[cr.CreatedById] : "Unknown User";
                    var contentType = await _contentTypeResolutionService.GetContentTypeByIdAsync(cr.ContentId);
                    var contentTitle = await _contentTypeResolutionService.GetContentTitleByIdAsync(cr.ContentId);

                    recentActivity.Add(new WorkflowActivityItem
                    {
                        Id = cr.Id,
                        ContentTitle = !string.IsNullOrEmpty(contentTitle) ? contentTitle : cr.Title,
                        ContentType = contentType,
                        Action = GetActionFromStatus(cr.Status),
                        User = userName,
                        Timestamp = cr.LastModified,
                        FromStage = GetPreviousStage(cr, workflows),
                        ToStage = GetCurrentStage(cr, workflows)
                    });
                }

                var overview = new WorkflowDashboardOverview
                {
                    TotalWorkflows = workflows.Count(),
                    TotalContentInWorkflow = totalContentInWorkflow,
                    PendingApproval = pendingApproval,
                    RecentlyApproved = recentlyApproved,
                    Rejected = rejected,
                    StageDistribution = stageDistribution,
                    RecentActivity = recentActivity
                };

                stopwatch.Stop();
                _telemetryService.RecordWorkflowOperation("get_overview", "success", stopwatch.Elapsed);
                activity?.SetStatus(ActivityStatusCode.Ok);

                return Ok(overview);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _telemetryService.RecordWorkflowOperation("get_overview", "error", stopwatch.Elapsed);
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                
                return StatusCode(500, new { message = "Unable to load change requests", error = ex.Message });
            }
        }

        /// <summary>
        /// Helper method to get action text from change request status.
        /// </summary>
        private string GetActionFromStatus(Piranha.Models.ChangeRequestStatus status)
        {
            return status switch
            {
                Piranha.Models.ChangeRequestStatus.Draft => "Created",
                Piranha.Models.ChangeRequestStatus.InReview => "Under Review",
                Piranha.Models.ChangeRequestStatus.Rejected => "Rejected",
                Piranha.Models.ChangeRequestStatus.Published => "Published",
                _ => "Updated"
            };
        }

        /// <summary>
        /// Helper method to get current stage name from change request.
        /// </summary>
        private string GetCurrentStage(Piranha.Models.ChangeRequest changeRequest, IEnumerable<Piranha.Models.Workflow> workflows)
        {
            var workflow = workflows.FirstOrDefault(w => w.Id == changeRequest.WorkflowId);
            var stage = workflow?.Stages?.FirstOrDefault(s => s.Id == changeRequest.StageId);
            return stage?.Title ?? "Unknown Stage";
        }

        /// <summary>
        /// Helper method to get previous stage name (simplified for now).
        /// </summary>
        private string GetPreviousStage(Piranha.Models.ChangeRequest changeRequest, IEnumerable<Piranha.Models.Workflow> workflows)
        {
            var workflow = workflows.FirstOrDefault(w => w.Id == changeRequest.WorkflowId);
            var currentStage = workflow?.Stages?.FirstOrDefault(s => s.Id == changeRequest.StageId);
            
            if (currentStage != null && workflow?.Stages != null)
            {
                var previousStage = workflow.Stages
                    .Where(s => s.SortOrder < currentStage.SortOrder)
                    .OrderByDescending(s => s.SortOrder)
                    .FirstOrDefault();
                return previousStage?.Title ?? "Initial";
            }
            
            return "Initial";
        }

        /// <summary>
        /// Gets available actions for a change request based on user permissions and status.
        /// </summary>
        private List<object> GetAvailableActionsForChangeRequest(Piranha.Models.ChangeRequest changeRequest, Guid userId, List<Guid> accessibleStageIds)
        {
            var actions = new List<object>();

            // Check if user can perform actions on this change request
            // User can act if they have access to the current stage or created the request
            var canAct = changeRequest.CreatedById == userId || accessibleStageIds.Contains(changeRequest.StageId);

            if (!canAct)
            {
                return actions; // No actions available
            }

            switch (changeRequest.Status)
            {
                case Piranha.Models.ChangeRequestStatus.InReview:
                    // Only allow approve/reject if user has access to current stage
                    if (accessibleStageIds.Contains(changeRequest.StageId))
                    {
                        actions.Add(new
                        {
                            Type = "approve",
                            Label = "Approve",
                            Enabled = true,
                            Icon = "fas fa-check",
                            Data = new { }
                        });
                        actions.Add(new
                        {
                            Type = "reject",
                            Label = "Reject",
                            Enabled = true,
                            Icon = "fas fa-times",
                            Data = new { }
                        });
                    }
                    break;
                case Piranha.Models.ChangeRequestStatus.Draft:
                    // Allow edit for creator or stage users
                    actions.Add(new
                    {
                        Type = "edit",
                        Label = "Edit",
                        Enabled = true,
                        Icon = "fas fa-edit",
                        Data = new { }
                    });
                    break;
                case Piranha.Models.ChangeRequestStatus.Rejected:
                    // Could add reopen action here if needed
                    break;
            }

            return actions;
        }

        /// <summary>
        /// Gets detailed change history with filtering options.
        /// </summary>
        /// <param name="contentType">Filter by content type (Page/Post)</param>
        /// <param name="workflowId">Filter by workflow ID</param>
        /// <param name="stageId">Filter by stage ID</param>
        /// <param name="user">Filter by user</param>
        /// <param name="changeType">Filter by change type</param>
        /// <param name="startDate">Filter from date</param>
        /// <param name="endDate">Filter to date</param>
        /// <param name="page">Page number for pagination</param>
        /// <param name="pageSize">Items per page</param>
        /// <returns>Paginated change history</returns>
        [Route("changes")]
        [HttpGet]
        public async Task<IActionResult> GetChangeHistory(
            string contentType = null,
            Guid? workflowId = null,
            Guid? stageId = null,
            string user = null,
            string changeType = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            int page = 1,
            int pageSize = 20)
        {
            try
            {
                // Get all change requests and workflows for reference
                var changeRequests = await _api.ChangeRequests.GetAllAsync();
                var workflows = await _api.Workflows.GetAllAsync();

                // Resolve user names for all change requests
                var userIds = changeRequests.Select(cr => cr.CreatedById).Distinct();
                var userNames = await _userResolutionService.GetUserNamesByIdsAsync(userIds);

                // Convert change requests to change items with resolved data
                var changes = new List<WorkflowChangeItem>();
                foreach (var cr in changeRequests)
                {
                    var userName = userNames.ContainsKey(cr.CreatedById) ? userNames[cr.CreatedById] : "Unknown User";
                    var resolvedContentType = await _contentTypeResolutionService.GetContentTypeByIdAsync(cr.ContentId);
                    var contentTitle = await _contentTypeResolutionService.GetContentTitleByIdAsync(cr.ContentId);
                    var editUrl = await _contentTypeResolutionService.GetEditUrlByContentIdAsync(cr.ContentId, resolvedContentType);

                    changes.Add(new WorkflowChangeItem
                    {
                        Id = cr.Id,
                        ContentId = cr.ContentId,
                        ContentTitle = !string.IsNullOrEmpty(contentTitle) ? contentTitle : cr.Title,
                        ContentType = resolvedContentType,
                        ChangeType = GetActionFromStatus(cr.Status),
                        User = userName,
                        Timestamp = cr.LastModified,
                        PreviousStage = GetPreviousStage(cr, workflows),
                        CurrentStage = GetCurrentStage(cr, workflows),
                        Description = !string.IsNullOrEmpty(cr.Notes) ? cr.Notes : "Change request update",
                        WorkflowName = workflows.FirstOrDefault(w => w.Id == cr.WorkflowId)?.Title ?? "Unknown Workflow",
                        EditUrl = editUrl
                    });
                }

                // Apply filters
                var filteredChanges = changes.AsQueryable();

                if (workflowId.HasValue)
                    filteredChanges = filteredChanges.Where(c => workflows.Any(w => w.Id == workflowId.Value && w.Title == c.WorkflowName));

                if (stageId.HasValue)
                {
                    var stageName = workflows.SelectMany(w => w.Stages ?? new List<Piranha.Models.WorkflowStage>())
                        .FirstOrDefault(s => s.Id == stageId.Value)?.Title;
                    if (!string.IsNullOrEmpty(stageName))
                        filteredChanges = filteredChanges.Where(c => c.CurrentStage == stageName);
                }

                if (!string.IsNullOrEmpty(user))
                    filteredChanges = filteredChanges.Where(c => c.User.Contains(user, StringComparison.OrdinalIgnoreCase));

                if (!string.IsNullOrEmpty(changeType))
                    filteredChanges = filteredChanges.Where(c => c.ChangeType.Equals(changeType, StringComparison.OrdinalIgnoreCase));

                if (startDate.HasValue)
                    filteredChanges = filteredChanges.Where(c => c.Timestamp >= startDate.Value);

                if (endDate.HasValue)
                    filteredChanges = filteredChanges.Where(c => c.Timestamp <= endDate.Value);

                // Order by most recent first
                var orderedChanges = filteredChanges.OrderByDescending(c => c.Timestamp).ToList();

                // Apply pagination
                var totalCount = orderedChanges.Count;
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
                var pagedChanges = orderedChanges.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                var result = new WorkflowChangeHistory
                {
                    Items = pagedChanges,
                    TotalCount = totalCount,
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalPages = totalPages
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Unable to load change requests", error = ex.Message });
            }
        }

        /// <summary>
        /// Gets workflow analytics data.
        /// </summary>
        /// <param name="days">Number of days to include in analytics (default: 30)</param>
        /// <returns>Workflow analytics</returns>
        [Route("analytics")]
        [HttpGet]
        public async Task<IActionResult> GetAnalytics(int days = 30)
        {
            try
            {
                var workflows = await _api.Workflows.GetAllAsync();
                var changeRequests = await _api.ChangeRequests.GetAllAsync();
                var startDate = DateTime.Now.Date.AddDays(-days);

                // Gather all transitions for all change requests
                var allTransitions = new List<Piranha.Models.ChangeRequestTransition>();
                foreach (var cr in changeRequests)
                {
                    var transitions = await _api.ChangeRequests.GetTransitionsAsync(cr.Id);
                    allTransitions.AddRange(transitions);
                }

                // Count accepted (approved) and rejected requests in the period
                var acceptedTransitions = allTransitions.Where(t => t.ActionType == "Approve" && t.Timestamp >= startDate).ToList();
                var rejectedTransitions = allTransitions.Where(t => t.ActionType == "Reject" && t.Timestamp >= startDate).ToList();
                int acceptedCount = acceptedTransitions.Count;
                int rejectedCount = rejectedTransitions.Count;

                // Calculate average acceptance time (from creation to first Approve transition)
                var acceptanceTimes = new List<double>();
                foreach (var cr in changeRequests)
                {
                    var approve = allTransitions.Where(t => t.ChangeRequestId == cr.Id && t.ActionType == "Approve").OrderBy(t => t.Timestamp).FirstOrDefault();
                    if (approve != null)
                    {
                        var hours = (approve.Timestamp - cr.CreatedAt).TotalHours;
                        if (hours >= 0) acceptanceTimes.Add(hours);
                    }
                }
                double avgAcceptanceTime = acceptanceTimes.Count > 0 ? acceptanceTimes.Average() : 0;

                // Generate daily stats for the specified period using actual change request data
                var dailyStats = new List<WorkflowDailyStats>();
                for (int i = days - 1; i >= 0; i--)
                {
                    var date = DateTime.Now.Date.AddDays(-i);
                    dailyStats.Add(new WorkflowDailyStats
                    {
                        Date = date,
                        ItemsCreated = changeRequests.Count(cr => cr.CreatedAt.Date == date),
                        // TODO: Adapt this to use other entities to get approval data
                        //ItemsApproved = changeRequests.Count(cr => 
                        //    cr.Status == Piranha.Models.ChangeRequestStatus.Approved && 
                        //    cr.LastModified.Date == date),
                        ItemsApproved = 0,
                        ItemsRejected = changeRequests.Count(cr => 
                            cr.Status == Piranha.Models.ChangeRequestStatus.Rejected && 
                            cr.LastModified.Date == date),
                        ItemsCompleted = changeRequests.Count(cr => 
                            cr.Status == Piranha.Models.ChangeRequestStatus.Published && 
                            cr.LastModified.Date == date)
                    });
                }

                // Generate workflow performance metrics using real data
                var workflowMetrics = workflows.Select(w => new WorkflowPerformanceMetric
                {
                    WorkflowId = w.Id,
                    WorkflowName = w.Title,
                    AverageProcessingTimeHours = CalculateAverageProcessingTime(changeRequests, w.Id),
                    CompletionRate = CalculateCompletionRate(changeRequests, w.Id),
                    TotalItemsProcessed = changeRequests.Count(cr => cr.WorkflowId == w.Id),
                    ActiveItems = changeRequests.Count(cr => cr.WorkflowId == w.Id && 
                        cr.Status != Piranha.Models.ChangeRequestStatus.Published &&
                        cr.Status != Piranha.Models.ChangeRequestStatus.Rejected)
                }).ToList();

                // Generate user productivity stats based on change request data
                var userIds = changeRequests.Select(cr => cr.CreatedById).Distinct();
                var userNameMapping = await _userResolutionService.GetUserNamesByIdsAsync(userIds);
                
                var userStats = changeRequests
                    .GroupBy(cr => cr.CreatedById)
                    .Select(g => new UserProductivityStat
                    {
                        UserId = g.Key.ToString(),
                        UserName = userNameMapping.ContainsKey(g.Key) ? userNameMapping[g.Key] : "Unknown User",
                        ItemsProcessed = g.Count(),
                        //ItemsApproved = g.Count(cr => cr.Status == Piranha.Models.ChangeRequestStatus.Approved),
                        ItemsApproved = 0, // TODO: Adapt this to use other entities to get approval data
                        ItemsRejected = g.Count(cr => cr.Status == Piranha.Models.ChangeRequestStatus.Rejected),
                        AverageProcessingTimeHours = CalculateUserAverageProcessingTime(g)
                    }).ToList();

                // Generate content type distribution based on change requests
                var contentTypeStats = new List<ContentTypeDistribution>
                {
                    new ContentTypeDistribution
                    {
                        ContentType = "Change Requests",
                        Count = changeRequests.Count(),
                        Percentage = 100.0,
                        AverageProcessingTimeHours = CalculateOverallAverageProcessingTime(changeRequests)
                    }
                };

                var completedWorkflows = changeRequests.Count(cr => cr.Status == Piranha.Models.ChangeRequestStatus.Published);
                var totalWorkflows = changeRequests.Count(cr => 
                    cr.Status != Piranha.Models.ChangeRequestStatus.Draft);

                // TODO: Review this approvalRate calculation
                //var approvalRate = totalWorkflows > 0 ? 
                //    (double)changeRequests.Count(cr => cr.Status == Piranha.Models.ChangeRequestStatus.Approved) / totalWorkflows * 100 : 0;
                var approvalRate = 0;

                var analytics = new WorkflowAnalytics
                {
                    DailyStats = dailyStats,
                    WorkflowMetrics = workflowMetrics,
                    UserStats = userStats,
                    ContentTypeStats = contentTypeStats,
                    AverageProcessingTimeHours = CalculateOverallAverageProcessingTime(changeRequests),
                    CompletedWorkflows = completedWorkflows,
                    ApprovalRate = approvalRate,
                    AcceptedCount = acceptedCount,
                    RejectedCount = rejectedCount,
                    AverageAcceptanceTimeHours = avgAcceptanceTime
                };

                return Ok(analytics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Unable to load change requests", error = ex.Message });
            }
        }

        /// <summary>
        /// Calculates average processing time for a workflow.
        /// </summary>
        private double CalculateAverageProcessingTime(IEnumerable<Piranha.Models.ChangeRequest> changeRequests, Guid workflowId)
        {
            var workflowRequests = changeRequests.Where(cr => cr.WorkflowId == workflowId);
            if (!workflowRequests.Any()) return 0;

            var totalHours = workflowRequests
                .Where(cr => cr.Status == Piranha.Models.ChangeRequestStatus.Published)
                // || cr.Status == Piranha.Models.ChangeRequestStatus.Approved
                .Sum(cr => (cr.LastModified - cr.CreatedAt).TotalHours);

            var completedCount = workflowRequests.Count(cr =>
                cr.Status == Piranha.Models.ChangeRequestStatus.Published);
                // || cr.Status == Piranha.Models.ChangeRequestStatus.Approved);

            return completedCount > 0 ? totalHours / completedCount : 0;
        }

        /// <summary>
        /// Calculates completion rate for a workflow.
        /// </summary>
        private double CalculateCompletionRate(IEnumerable<Piranha.Models.ChangeRequest> changeRequests, Guid workflowId)
        {
            var workflowRequests = changeRequests.Where(cr => cr.WorkflowId == workflowId);
            if (!workflowRequests.Any()) return 0;

            var completed = workflowRequests.Count(cr => cr.Status == Piranha.Models.ChangeRequestStatus.Published);
            return (double)completed / workflowRequests.Count() * 100;
        }

        /// <summary>
        /// Calculates average processing time for a user.
        /// </summary>
        private double CalculateUserAverageProcessingTime(IEnumerable<Piranha.Models.ChangeRequest> userRequests)
        {
            if (!userRequests.Any()) return 0;

            var totalHours = userRequests
                .Where(cr => cr.Status == Piranha.Models.ChangeRequestStatus.Published)
                        // || cr.Status == Piranha.Models.ChangeRequestStatus.Approved)
                .Sum(cr => (cr.LastModified - cr.CreatedAt).TotalHours);

            var completedCount = userRequests.Count(cr =>
                cr.Status == Piranha.Models.ChangeRequestStatus.Published);
                // || cr.Status == Piranha.Models.ChangeRequestStatus.Approved);

            return completedCount > 0 ? totalHours / completedCount : 0;
        }

        /// <summary>
        /// Calculates overall average processing time.
        /// </summary>
        private double CalculateOverallAverageProcessingTime(IEnumerable<Piranha.Models.ChangeRequest> changeRequests)
        {
            if (!changeRequests.Any()) return 0;

            var completedRequests = changeRequests.Where(cr =>
                cr.Status == Piranha.Models.ChangeRequestStatus.Published);
                // || cr.Status == Piranha.Models.ChangeRequestStatus.Approved);

            if (!completedRequests.Any()) return 0;

            var totalHours = completedRequests.Sum(cr => (cr.LastModified - cr.CreatedAt).TotalHours);
            return totalHours / completedRequests.Count();
        }
    }
}
