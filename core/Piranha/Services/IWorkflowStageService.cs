using Piranha.Models;

namespace Piranha.Services;

/// <summary>
/// Interface for the workflow stage service.
/// </summary>
public interface IWorkflowStageService
{
    /// <summary>
    /// Gets all workflow stages for the specified workflow.
    /// </summary>
    /// <param name="workflowId">The workflow id</param>
    /// <returns>The available workflow stages</returns>
    Task<IEnumerable<WorkflowStage>> GetAllAsync(Guid workflowId);

    /// <summary>
    /// Gets the workflow stage with the specified id.
    /// </summary>
    /// <param name="id">The unique id</param>
    /// <returns>The workflow stage</returns>
    Task<WorkflowStage> GetByIdAsync(Guid id);

    /// <summary>
    /// Adds or updates the given workflow stage.
    /// </summary>
    /// <param name="stage">The workflow stage</param>
    Task SaveAsync(WorkflowStage stage);

    /// <summary>
    /// Deletes the workflow stage with the specified id.
    /// </summary>
    /// <param name="id">The unique id</param>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// Deletes the given workflow stage.
    /// </summary>
    /// <param name="stage">The workflow stage</param>
    Task DeleteAsync(WorkflowStage stage);
    
    /// <summary>
    /// Reorders the stages with the given ids to match the order of the array.
    /// </summary>
    /// <param name="workflowId">The workflow id</param>
    /// <param name="stageIds">The array of stage ids in the desired order</param>
    Task ReorderAsync(Guid workflowId, Guid[] stageIds);

    /// <summary>
    /// Assigns roles to a workflow stage.
    /// </summary>
    /// <param name="stageId">The stage id</param>
    /// <param name="roleIds">The role ids to assign</param>
    Task AssignRolesAsync(Guid stageId, IEnumerable<string> roleIds);

    /// <summary>
    /// Gets the roles assigned to a workflow stage.
    /// </summary>
    /// <param name="stageId">The stage id</param>
    /// <returns>The assigned role ids</returns>
    Task<IEnumerable<string>> GetStageRolesAsync(Guid stageId);
}
