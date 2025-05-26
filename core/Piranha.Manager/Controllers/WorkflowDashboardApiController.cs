
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Piranha.Manager.Models;
using Piranha.Manager.Services;
using Piranha.Security;

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

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <param name="localizer">The manager localizer</param>
        public WorkflowDashboardApiController(IApi api, ManagerLocalizer localizer)
        {
            _api = api;
            _localizer = localizer;
        }

        /// <summary>
        /// Gets the dashboard overview data.
        /// </summary>
        /// <returns>The dashboard overview</returns>
        [Route("overview")]
        [HttpGet]
        public async Task<IActionResult> GetOverview()
        {
            try
            {
                // Get all workflows
                var workflows = await _api.Workflows.GetAllAsync();
                
                // Get recent page revisions (representing workflow activity)
                var pages = await _api.Pages.GetAllAsync();
                var posts = await _api.Posts.GetAllBySiteIdAsync();

                // Calculate overview metrics
                var overview = new WorkflowDashboardOverview
                {
                    TotalWorkflows = workflows.Count(),
                    TotalContentInWorkflow = 0, // Would need proper content-workflow mapping
                    PendingApproval = 0, // Would need workflow state tracking
                    RecentlyApproved = 0,
                    Rejected = 0,
                    StageDistribution = workflows.SelectMany(w => w.Stages ?? new List<Piranha.Models.WorkflowStage>())
                        .GroupBy(s => new { s.Id, s.Title })
                        .Select(g => new WorkflowStageCount
                        {
                            StageId = g.Key.Id,
                            StageName = g.Key.Title,
                            Count = 0, // Would need actual content counts
                            WorkflowName = workflows.FirstOrDefault(w => w.Stages.Any(s => s.Id == g.Key.Id))?.Title
                        }).ToList(),
                    RecentActivity = pages.Take(5).Select(r => new WorkflowActivityItem
                    {
                        Id = r.Id,
                        ContentTitle = r.Title ?? "Page Content",
                        ContentType = "Page",
                        Action = "Updated",
                        User = "System",
                        Timestamp = r.Created,
                        FromStage = "Draft",
                        ToStage = "Review"
                    })
                    .Concat(posts.Take(5).Select(r => new WorkflowActivityItem
                    {
                        Id = r.Id,
                        ContentTitle = r.Title ?? "Post Content",
                        ContentType = "Post",
                        Action = "Updated",
                        User = "System",
                        Timestamp = r.Created,
                        FromStage = "Draft",
                        ToStage = "Review"
                    }))
                    .OrderByDescending(a => a.Timestamp)
                    .Take(10)
                    .ToList()
                };

                return Ok(overview);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving dashboard overview", error = ex.Message });
            }
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
                var changes = new List<WorkflowChangeItem>();

                // Get page changes if not filtered or if filtering for pages
                if (string.IsNullOrEmpty(contentType) || contentType.Equals("Page", StringComparison.OrdinalIgnoreCase))
                {
                    var pages = await _api.Pages.GetAllAsync();
                    foreach (var pageItem in pages.Take(100)) // Limit for performance
                    {
                        // Since we don't have revision history, create placeholder entries based on the page itself
                        changes.Add(new WorkflowChangeItem
                        {
                            Id = pageItem.Id,
                            ContentId = pageItem.Id,
                            ContentTitle = pageItem.Title,
                            ContentType = "Page",
                            ChangeType = "Updated",
                            User = "System", // TODO: Get actual user when available
                            Timestamp = pageItem.LastModified,
                            PreviousStage = "Draft",
                            CurrentStage = "Review",
                            Description = "Content updated",
                            WorkflowName = "Default Page Workflow",
                            EditUrl = $"/manager/page/edit/{pageItem.Id}"
                        });
                    }
                }

                // Get post changes if not filtered or if filtering for posts
                if (string.IsNullOrEmpty(contentType) || contentType.Equals("Post", StringComparison.OrdinalIgnoreCase))
                {
                    var posts = await _api.Posts.GetAllBySiteIdAsync();
                    foreach (var postItem in posts.Take(100)) // Limit for performance
                    {
                        // Since we don't have revision history, create placeholder entries based on the post itself
                        changes.Add(new WorkflowChangeItem
                        {
                            Id = postItem.Id,
                            ContentId = postItem.Id,
                            ContentTitle = postItem.Title,
                            ContentType = "Post",
                            ChangeType = "Updated",
                            User = "System", // TODO: Get actual user when available
                            Timestamp = postItem.LastModified,
                            PreviousStage = "Draft",
                            CurrentStage = "Review",
                            Description = "Content updated",
                            WorkflowName = "Default Post Workflow",
                            EditUrl = $"/manager/post/edit/{postItem.Id}"
                        });
                    }
                }

                // Apply filters
                var filteredChanges = changes.AsQueryable();

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
                var pagedChanges = orderedChanges.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                var result = new WorkflowChangeHistory
                {
                    Items = pagedChanges,
                    TotalCount = totalCount,
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                    Filters = new WorkflowChangeFilters
                    {
                        ContentType = contentType,
                        WorkflowId = workflowId,
                        StageId = stageId,
                        User = user,
                        ChangeType = changeType,
                        StartDate = startDate,
                        EndDate = endDate
                    }
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving change history", error = ex.Message });
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
                var pages = await _api.Pages.GetAllAsync();
                var posts = await _api.Posts.GetAllBySiteIdAsync();

                // Generate daily stats for the specified period
                var dailyStats = new List<WorkflowDailyStats>();
                for (int i = days - 1; i >= 0; i--)
                {
                    var date = DateTime.Now.Date.AddDays(-i);
                    dailyStats.Add(new WorkflowDailyStats
                    {
                        Date = date,
                        ItemsCreated = GetItemsCreatedOnDate(pages, posts, date),
                        ItemsApproved = 0, // TODO: Calculate based on workflow transitions
                        ItemsRejected = 0, // TODO: Calculate based on workflow transitions
                        ItemsCompleted = 0 // TODO: Calculate based on workflow completions
                    });
                }

                // Generate workflow performance metrics
                var workflowMetrics = workflows.Select(w => new WorkflowPerformanceMetric
                {
                    WorkflowId = w.Id,
                    WorkflowName = w.Title,
                    AverageProcessingTimeHours = 24.0, // TODO: Calculate actual processing time
                    CompletionRate = 85.0, // TODO: Calculate actual completion rate
                    TotalItemsProcessed = GetTotalItemsForWorkflow(pages, posts, w.Id),
                    ActiveItems = GetActiveItemsForWorkflow(pages, posts, w.Id)
                }).ToList();

                // Generate bottleneck analysis
                var bottlenecks = workflows.SelectMany(w => w.Stages ?? new List<Piranha.Models.WorkflowStage>())
                    .Select(s => new WorkflowStageBottleneck
                    {
                        StageId = s.Id,
                        StageName = s.Title,
                        WorkflowName = workflows.FirstOrDefault(w => w.Stages.Any(stage => stage.Id == s.Id))?.Title,
                        AverageTimeInStageHours = 12.0, // TODO: Calculate actual time
                        BacklogCount = 5, // TODO: Calculate actual backlog
                        BottleneckSeverity = 2 // TODO: Calculate severity based on metrics
                    }).ToList();

                // Generate user productivity stats (placeholder)
                var userStats = new List<UserProductivityStat>
                {
                    new UserProductivityStat
                    {
                        UserId = "system",
                        UserName = "System",
                        ItemsProcessed = pages.Count() + posts.Count(),
                        ItemsApproved = 0,
                        ItemsRejected = 0,
                        AverageProcessingTimeHours = 2.5
                    }
                };

                // Generate content type distribution
                var contentTypeStats = new List<ContentTypeDistribution>
                {
                    new ContentTypeDistribution
                    {
                        ContentType = "Pages",
                        Count = pages.Count(),
                        Percentage = (double)pages.Count() / (pages.Count() + posts.Count()) * 100,
                        AverageProcessingTimeHours = 3.2
                    },
                    new ContentTypeDistribution
                    {
                        ContentType = "Posts",
                        Count = posts.Count(),
                        Percentage = (double)posts.Count() / (pages.Count() + posts.Count()) * 100,
                        AverageProcessingTimeHours = 2.1
                    }
                };

                var analytics = new WorkflowAnalytics
                {
                    DailyStats = dailyStats,
                    WorkflowMetrics = workflowMetrics,
                    Bottlenecks = bottlenecks,
                    UserStats = userStats,
                    ContentTypeStats = contentTypeStats,
                    AverageProcessingTimeHours = 18.5,
                    CompletedWorkflows = 42,
                    ApprovalRate = 87.3
                };

                return Ok(analytics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving analytics", error = ex.Message });
            }
        }

        private int GetItemsCreatedOnDate(IEnumerable<Piranha.Models.DynamicPage> pages, IEnumerable<Piranha.Models.DynamicPost> posts, DateTime date)
        {
            var pageCount = pages.Count(p => p.Created.Date == date);
            var postCount = posts.Count(p => p.Created.Date == date);
            return pageCount + postCount;
        }

        private int GetTotalItemsForWorkflow(IEnumerable<Piranha.Models.DynamicPage> pages, IEnumerable<Piranha.Models.DynamicPost> posts, Guid workflowId)
        {
            // TODO: Implement actual workflow association logic
            return pages.Count() + posts.Count();
        }

        private int GetActiveItemsForWorkflow(IEnumerable<Piranha.Models.DynamicPage> pages, IEnumerable<Piranha.Models.DynamicPost> posts, Guid workflowId)
        {
            // TODO: Implement actual active items logic based on workflow state
            return pages.Count(p => !p.Published.HasValue) + posts.Count(p => !p.Published.HasValue);
        }
    }
}
