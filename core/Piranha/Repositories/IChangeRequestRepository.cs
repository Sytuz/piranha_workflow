using Piranha.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Piranha.Repositories
{
    /// <summary>
    /// Repository interface for change requests.
    /// </summary>
    public interface IChangeRequestRepository
    {
        /// <summary>
        /// Gets all change requests for the specified workflow.
        /// </summary>
        /// <param name="workflowId">The workflow id</param>
        /// <returns>The change requests</returns>
        Task<IEnumerable<ChangeRequest>> GetByWorkflowIdAsync(Guid workflowId);

        /// <summary>
        /// Gets all change requests created by the specified user.
        /// </summary>
        /// <param name="userId">The user id</param>
        /// <returns>The change requests</returns>
        Task<IEnumerable<ChangeRequest>> GetByCreatedByIdAsync(Guid userId);

        /// <summary>
        /// Gets all change requests in the specified stage.
        /// </summary>
        /// <param name="stageId">The stage id</param>
        /// <returns>The change requests</returns>
        Task<IEnumerable<ChangeRequest>> GetByStageIdAsync(Guid stageId);

        /// <summary>
        /// Gets the change request with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The change request</returns>
        Task<ChangeRequest> GetByIdAsync(Guid id);

        /// <summary>
        /// Gets all change requests.
        /// </summary>
        /// <returns>All change requests</returns>
        Task<IEnumerable<ChangeRequest>> GetAllAsync();

        /// <summary>
        /// Saves the given change request.
        /// </summary>
        /// <param name="model">The change request</param>
        Task SaveAsync(ChangeRequest model);

        /// <summary>
        /// Deletes the change request with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        Task DeleteAsync(Guid id);

        /// <summary>
        /// Deletes the given change request.
        /// </summary>
        /// <param name="model">The change request</param>
        Task DeleteAsync(ChangeRequest model);

        /// <summary>
        /// Moves the change request to the specified stage.
        /// </summary>
        /// <param name="id">The change request id</param>
        /// <param name="stageId">The target stage id</param>
        /// <returns>The updated change request</returns>
        Task<ChangeRequest> MoveToStageAsync(Guid id, Guid stageId);
    }
}
