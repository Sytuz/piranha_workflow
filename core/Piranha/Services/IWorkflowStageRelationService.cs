using Piranha.Models;

namespace Piranha.Services;

/// <summary>
/// Interface for the workflow stage relation service.
/// </summary>
public interface IWorkflowStageRelationService
{
    /// <summary>
    /// Gets all workflow stage relations for the specified workflow.
    /// </summary>
    /// <param name="workflowId">The workflow id</param>
    /// <returns>The available workflow stage relations</returns>
    Task<IEnumerable<WorkflowStageRelation>> GetAllAsync(Guid workflowId);

    /// <summary>
    /// Gets the workflow stage relation with the specified id.
    /// </summary>
    /// <param name="id">The unique id</param>
    /// <returns>The workflow stage relation</returns>
    Task<WorkflowStageRelation> GetByIdAsync(Guid id);
    
    /// <summary>
    /// Gets all relations for a specific source stage.
    /// </summary>
    /// <param name="sourceStageId">The source stage id</param>
    /// <returns>The workflow stage relations</returns>
    Task<IEnumerable<WorkflowStageRelation>> GetBySourceStageIdAsync(Guid sourceStageId);
    
    /// <summary>
    /// Gets all relations for a specific target stage.
    /// </summary>
    /// <param name="targetStageId">The target stage id</param>
    /// <returns>The workflow stage relations</returns>
    Task<IEnumerable<WorkflowStageRelation>> GetByTargetStageIdAsync(Guid targetStageId);

    /// <summary>
    /// Adds or updates the given workflow stage relation.
    /// </summary>
    /// <param name="relation">The workflow stage relation</param>
    Task SaveAsync(WorkflowStageRelation relation);

    /// <summary>
    /// Deletes the workflow stage relation with the specified id.
    /// </summary>
    /// <param name="id">The unique id</param>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// Deletes the given workflow stage relation.
    /// </summary>
    /// <param name="relation">The workflow stage relation</param>
    Task DeleteAsync(WorkflowStageRelation relation);
    
    /// <summary>
    /// Deletes all relations for the specified workflow.
    /// </summary>
    /// <param name="workflowId">The workflow id</param>
    Task DeleteByWorkflowIdAsync(Guid workflowId);
    
    /// <summary>
    /// Deletes all relations for the specified stage.
    /// </summary>
    /// <param name="stageId">The stage id</param>
    Task DeleteByStageIdAsync(Guid stageId);
}
