using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Piranha.Models;

namespace Piranha.Repositories
{
    public interface IWorkflowStageRelationRepository
    {
        /// <summary>
        /// Gets all workflow stage relations.
        /// </summary>
        /// <returns>The workflow stage relations</returns>
        Task<IEnumerable<WorkflowStageRelation>> GetAll();

        /// <summary>
        /// Gets the workflow stage relation with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The workflow stage relation</returns>
        Task<WorkflowStageRelation> GetById(Guid id);

        /// <summary>
        /// Gets all workflow stage relations for the specified workflow.
        /// </summary>
        /// <param name="workflowId">The workflow id</param>
        /// <returns>The workflow stage relations</returns>
        Task<IEnumerable<WorkflowStageRelation>> GetByWorkflowId(Guid workflowId);

        /// <summary>
        /// Gets all workflow stage relations for the specified source stage.
        /// </summary>
        /// <param name="stageId">The source stage id</param>
        /// <returns>The workflow stage relations</returns>
        Task<IEnumerable<WorkflowStageRelation>> GetBySourceStageId(Guid stageId);

        /// <summary>
        /// Gets all workflow stage relations for the specified target stage.
        /// </summary>
        /// <param name="stageId">The target stage id</param>
        /// <returns>The workflow stage relations</returns>
        Task<IEnumerable<WorkflowStageRelation>> GetByTargetStageId(Guid stageId);

        /// <summary>
        /// Adds or updates the given workflow stage relation.
        /// </summary>
        /// <param name="relation">The workflow stage relation</param>
        Task Save(WorkflowStageRelation relation);

        /// <summary>
        /// Deletes the workflow stage relation with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        Task Delete(Guid id);

        /// <summary>
        /// Deletes the given workflow stage relation.
        /// </summary>
        /// <param name="relation">The workflow stage relation</param>
        Task Delete(WorkflowStageRelation relation);

        /// <summary>
        /// Deletes all relations for the specified workflow.
        /// </summary>
        /// <param name="workflowId">The workflow id</param>
        Task DeleteByWorkflow(Guid workflowId);

        /// <summary>
        /// Deletes all relations involving the specified stage (as source or target).
        /// </summary>
        /// <param name="stageId">The stage id</param>
        Task DeleteByStage(Guid stageId);
    }
}
