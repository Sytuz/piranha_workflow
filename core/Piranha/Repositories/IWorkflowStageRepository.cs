using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Piranha.Models;

namespace Piranha.Repositories
{
    public interface IWorkflowStageRepository
    {
        /// <summary>
        /// Gets all workflow stages.
        /// </summary>
        /// <returns>The workflow stages</returns>
        Task<IEnumerable<WorkflowStage>> GetAll();

        /// <summary>
        /// Gets the workflow stage with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The workflow stage</returns>
        Task<WorkflowStage> GetById(Guid id);

        /// <summary>
        /// Gets all workflow stages for the specified workflow.
        /// </summary>
        /// <param name="workflowId">The workflow id</param>
        /// <returns>The workflow stages</returns>
        Task<IEnumerable<WorkflowStage>> GetByWorkflowId(Guid workflowId);

        /// <summary>
        /// Adds or updates the given workflow stage.
        /// </summary>
        /// <param name="stage">The workflow stage</param>
        Task Save(WorkflowStage stage);

        /// <summary>
        /// Deletes the workflow stage with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        Task Delete(Guid id);

        /// <summary>
        /// Deletes the given workflow stage.
        /// </summary>
        /// <param name="stage">The workflow stage</param>
        Task Delete(WorkflowStage stage);

        /// <summary>
        /// Deletes all stages for the specified workflow.
        /// </summary>
        /// <param name="workflowId">The workflow id</param>
        Task DeleteByWorkflow(Guid workflowId);
    }
}
