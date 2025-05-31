using Piranha.Models;

namespace Piranha.Repositories
{
    /// <summary>
    /// Repository for handling workflow stage roles.
    /// </summary>
    public interface IWorkflowStageRoleRepository
    {
        /// <summary>
        /// Gets all roles for a specific workflow stage.
        /// </summary>
        /// <param name="workflowStageId">The workflow stage id</param>
        /// <returns>The workflow stage roles</returns>
        Task<IEnumerable<WorkflowStageRole>> GetByWorkflowStageIdAsync(Guid workflowStageId);

        /// <summary>
        /// Saves the given workflow stage role.
        /// </summary>
        /// <param name="model">The workflow stage role</param>
        Task SaveAsync(WorkflowStageRole model);

        /// <summary>
        /// Deletes the workflow stage role with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        Task DeleteAsync(Guid id);

        /// <summary>
        /// Deletes all workflow stage roles for the given workflow stage.
        /// </summary>
        /// <param name="workflowStageId">The workflow stage id</param>
        Task DeleteByWorkflowStageIdAsync(Guid workflowStageId);

        /// <summary>
        /// Checks if the given role is assigned to the workflow stage.
        /// </summary>
        /// <param name="workflowStageId">The workflow stage id</param>
        /// <param name="roleId">The role id</param>
        /// <returns>True if the role is assigned</returns>
        Task<bool> IsRoleAssignedAsync(Guid workflowStageId, string roleId);
    }
}
