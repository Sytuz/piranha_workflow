
namespace Piranha.Manager.Models;

/// <summary>
/// Workflow dashboard overview model containing key metrics and summary information.
/// </summary>
public class WorkflowDashboardOverview
{
    /// <summary>
    /// Gets/sets the total number of active workflows.
    /// </summary>
    public int TotalWorkflows { get; set; }

    /// <summary>
    /// Gets/sets the total number of content items in workflow.
    /// </summary>
    public int TotalContentInWorkflow { get; set; }

    /// <summary>
    /// Gets/sets the number of content items pending approval.
    /// </summary>
    public int PendingApproval { get; set; }

    /// <summary>
    /// Gets/sets the number of content items recently approved.
    /// </summary>
    public int RecentlyApproved { get; set; }

    /// <summary>
    /// Gets/sets the number of content items rejected.
    /// </summary>
    public int Rejected { get; set; }

    /// <summary>
    /// Gets/sets the workflow stages with their respective content counts.
    /// </summary>
    public IList<WorkflowStageCount> StageDistribution { get; set; } = new List<WorkflowStageCount>();

    /// <summary>
    /// Gets/sets the recent activity items.
    /// </summary>
    public IList<WorkflowActivityItem> RecentActivity { get; set; } = new List<WorkflowActivityItem>();
}

/// <summary>
/// Represents the count of content items in a workflow stage.
/// </summary>
public class WorkflowStageCount
{
    /// <summary>
    /// Gets/sets the stage identifier.
    /// </summary>
    public Guid StageId { get; set; }

    /// <summary>
    /// Gets/sets the stage name.
    /// </summary>
    public string StageName { get; set; }

    /// <summary>
    /// Gets/sets the number of content items in this stage.
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// Gets/sets the workflow name this stage belongs to.
    /// </summary>
    public string WorkflowName { get; set; }
}

/// <summary>
/// Represents a recent workflow activity item.
/// </summary>
public class WorkflowActivityItem
{
    /// <summary>
    /// Gets/sets the activity identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets/sets the content title.
    /// </summary>
    public string ContentTitle { get; set; }

    /// <summary>
    /// Gets/sets the content type (Page, Post, etc.).
    /// </summary>
    public string ContentType { get; set; }

    /// <summary>
    /// Gets/sets the action performed (Created, Approved, Rejected, etc.).
    /// </summary>
    public string Action { get; set; }

    /// <summary>
    /// Gets/sets the user who performed the action.
    /// </summary>
    public string User { get; set; }

    /// <summary>
    /// Gets/sets the timestamp of the activity.
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Gets/sets the source stage name.
    /// </summary>
    public string FromStage { get; set; }

    /// <summary>
    /// Gets/sets the target stage name.
    /// </summary>
    public string ToStage { get; set; }
}

/// <summary>
/// Model for workflow change history with filtering and pagination support.
/// </summary>
public class WorkflowChangeHistory
{
    /// <summary>
    /// Gets/sets the list of change items.
    /// </summary>
    public IList<WorkflowChangeItem> Items { get; set; } = new List<WorkflowChangeItem>();

    /// <summary>
    /// Gets/sets the total number of changes.
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Gets/sets the current page number.
    /// </summary>
    public int CurrentPage { get; set; }

    /// <summary>
    /// Gets/sets the total number of pages.
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Gets/sets the page size.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Gets/sets the applied filters.
    /// </summary>
    public WorkflowChangeFilters Filters { get; set; } = new WorkflowChangeFilters();
}

/// <summary>
/// Represents an individual workflow change item.
/// </summary>
public class WorkflowChangeItem
{
    /// <summary>
    /// Gets/sets the change identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets/sets the content identifier.
    /// </summary>
    public Guid ContentId { get; set; }

    /// <summary>
    /// Gets/sets the content title.
    /// </summary>
    public string ContentTitle { get; set; }

    /// <summary>
    /// Gets/sets the content type.
    /// </summary>
    public string ContentType { get; set; }

    /// <summary>
    /// Gets/sets the change type (Created, Updated, Approved, etc.).
    /// </summary>
    public string ChangeType { get; set; }

    /// <summary>
    /// Gets/sets the user who made the change.
    /// </summary>
    public string User { get; set; }

    /// <summary>
    /// Gets/sets the timestamp of the change.
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Gets/sets the previous stage name.
    /// </summary>
    public string PreviousStage { get; set; }

    /// <summary>
    /// Gets/sets the current stage name.
    /// </summary>
    public string CurrentStage { get; set; }

    /// <summary>
    /// Gets/sets optional change description or comments.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets/sets the workflow name.
    /// </summary>
    public string WorkflowName { get; set; }

    /// <summary>
    /// Gets/sets the content edit URL.
    /// </summary>
    public string EditUrl { get; set; }
}

/// <summary>
/// Filters for workflow change history.
/// </summary>
public class WorkflowChangeFilters
{
    /// <summary>
    /// Gets/sets the start date filter.
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Gets/sets the end date filter.
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Gets/sets the content type filter.
    /// </summary>
    public string ContentType { get; set; }

    /// <summary>
    /// Gets/sets the workflow filter.
    /// </summary>
    public Guid? WorkflowId { get; set; }

    /// <summary>
    /// Gets/sets the stage filter.
    /// </summary>
    public Guid? StageId { get; set; }

    /// <summary>
    /// Gets/sets the user filter.
    /// </summary>
    public string User { get; set; }

    /// <summary>
    /// Gets/sets the change type filter.
    /// </summary>
    public string ChangeType { get; set; }
}

/// <summary>
/// Workflow analytics model containing performance metrics and statistics.
/// </summary>
public class WorkflowAnalytics
{
    /// <summary>
    /// Gets/sets the daily statistics for the specified period.
    /// </summary>
    public IList<WorkflowDailyStats> DailyStats { get; set; } = new List<WorkflowDailyStats>();

    /// <summary>
    /// Gets/sets the workflow performance metrics.
    /// </summary>
    public IList<WorkflowPerformanceMetric> WorkflowMetrics { get; set; } = new List<WorkflowPerformanceMetric>();

    /// <summary>
    /// Gets/sets the stage bottleneck analysis.
    /// </summary>
    public IList<WorkflowStageBottleneck> Bottlenecks { get; set; } = new List<WorkflowStageBottleneck>();

    /// <summary>
    /// Gets/sets the user productivity statistics.
    /// </summary>
    public IList<UserProductivityStat> UserStats { get; set; } = new List<UserProductivityStat>();

    /// <summary>
    /// Gets/sets the content type distribution.
    /// </summary>
    public IList<ContentTypeDistribution> ContentTypeStats { get; set; } = new List<ContentTypeDistribution>();

    /// <summary>
    /// Gets/sets the average processing time across all workflows.
    /// </summary>
    public double AverageProcessingTimeHours { get; set; }

    /// <summary>
    /// Gets/sets the total number of completed workflows in the period.
    /// </summary>
    public int CompletedWorkflows { get; set; }

    /// <summary>
    /// Gets/sets the approval rate percentage.
    /// </summary>
    public double ApprovalRate { get; set; }
}

/// <summary>
/// Daily statistics for workflow activities.
/// </summary>
public class WorkflowDailyStats
{
    /// <summary>
    /// Gets/sets the date.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Gets/sets the number of items created.
    /// </summary>
    public int ItemsCreated { get; set; }

    /// <summary>
    /// Gets/sets the number of items approved.
    /// </summary>
    public int ItemsApproved { get; set; }

    /// <summary>
    /// Gets/sets the number of items rejected.
    /// </summary>
    public int ItemsRejected { get; set; }

    /// <summary>
    /// Gets/sets the number of items completed.
    /// </summary>
    public int ItemsCompleted { get; set; }
}

/// <summary>
/// Performance metrics for individual workflows.
/// </summary>
public class WorkflowPerformanceMetric
{
    /// <summary>
    /// Gets/sets the workflow identifier.
    /// </summary>
    public Guid WorkflowId { get; set; }

    /// <summary>
    /// Gets/sets the workflow name.
    /// </summary>
    public string WorkflowName { get; set; }

    /// <summary>
    /// Gets/sets the average processing time in hours.
    /// </summary>
    public double AverageProcessingTimeHours { get; set; }

    /// <summary>
    /// Gets/sets the completion rate percentage.
    /// </summary>
    public double CompletionRate { get; set; }

    /// <summary>
    /// Gets/sets the total items processed.
    /// </summary>
    public int TotalItemsProcessed { get; set; }

    /// <summary>
    /// Gets/sets the number of active items.
    /// </summary>
    public int ActiveItems { get; set; }
}

/// <summary>
/// Bottleneck analysis for workflow stages.
/// </summary>
public class WorkflowStageBottleneck
{
    /// <summary>
    /// Gets/sets the stage identifier.
    /// </summary>
    public Guid StageId { get; set; }

    /// <summary>
    /// Gets/sets the stage name.
    /// </summary>
    public string StageName { get; set; }

    /// <summary>
    /// Gets/sets the workflow name.
    /// </summary>
    public string WorkflowName { get; set; }

    /// <summary>
    /// Gets/sets the average time spent in this stage (in hours).
    /// </summary>
    public double AverageTimeInStageHours { get; set; }

    /// <summary>
    /// Gets/sets the current backlog count.
    /// </summary>
    public int BacklogCount { get; set; }

    /// <summary>
    /// Gets/sets the bottleneck severity (1-5, where 5 is most severe).
    /// </summary>
    public int BottleneckSeverity { get; set; }
}

/// <summary>
/// User productivity statistics.
/// </summary>
public class UserProductivityStat
{
    /// <summary>
    /// Gets/sets the user identifier.
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// Gets/sets the user name.
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// Gets/sets the number of items processed.
    /// </summary>
    public int ItemsProcessed { get; set; }

    /// <summary>
    /// Gets/sets the number of items approved.
    /// </summary>
    public int ItemsApproved { get; set; }

    /// <summary>
    /// Gets/sets the number of items rejected.
    /// </summary>
    public int ItemsRejected { get; set; }

    /// <summary>
    /// Gets/sets the average processing time per item (in hours).
    /// </summary>
    public double AverageProcessingTimeHours { get; set; }
}

/// <summary>
/// Content type distribution statistics.
/// </summary>
public class ContentTypeDistribution
{
    /// <summary>
    /// Gets/sets the content type name.
    /// </summary>
    public string ContentType { get; set; }

    /// <summary>
    /// Gets/sets the number of items.
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// Gets/sets the percentage of total.
    /// </summary>
    public double Percentage { get; set; }

    /// <summary>
    /// Gets/sets the average processing time for this content type.
    /// </summary>
    public double AverageProcessingTimeHours { get; set; }
}
