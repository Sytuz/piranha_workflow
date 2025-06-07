using Piranha.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Piranha.Services
{
    /// <summary>
    /// Service interface for change requests.
    /// </summary>
    public interface IChangeRequestService
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
        Task<IEnumerable<ChangeRequest>> GetByStageIdAsync(Guid stageId);        /// <summary>
        /// Gets the change request with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The change request</returns>
        Task<ChangeRequest> GetByIdAsync(Guid id);

        /// <summary>
        /// Gets all change requests for the specified content.
        /// </summary>
        /// <param name="contentId">The content id</param>
        /// <returns>The change requests</returns>
        Task<IEnumerable<ChangeRequest>> GetByContentIdAsync(Guid contentId);

        /// <summary>
        /// Gets all change requests.
        /// </summary>
        /// <returns>All change requests</returns>
        Task<IEnumerable<ChangeRequest>> GetAllAsync();

        /// <summary>
        /// Creates a new change request.
        /// </summary>
        /// <param name="title">The title</param>
        /// <param name="workflowId">The workflow id</param>
        /// <param name="createdById">The creator user id</param>
        /// <param name="contentId">Content id (required)</param>
        /// <param name="contentSnapshot">Serialized snapshot of content (required)</param>
        /// <param name="notes">Optional notes</param>
        /// <returns>The created change request</returns>
        Task<ChangeRequest> CreateAsync(string title, Guid workflowId, Guid createdById, Guid contentId, string contentSnapshot, string notes = null);

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
        /// Submits the change request to the next workflow stage.
        /// </summary>
        /// <param name="id">The change request id</param>
        /// <param name="userId">The user performing the submission</param>
        /// <returns>The updated change request</returns>
        Task<ChangeRequest> SubmitAsync(Guid id, Guid userId);

        /// <summary>
        /// Moves the change request to the specified stage.
        /// </summary>
        /// <param name="id">The change request id</param>
        /// <param name="stageId">The target stage id</param>
        /// <param name="userId">The user performing the transition</param>
        /// <returns>The updated change request</returns>
        Task<ChangeRequest> MoveToStageAsync(Guid id, Guid stageId, Guid userId);

        /// <summary>
        /// Validates if a user can create change requests for the specified workflow.
        /// </summary>
        /// <param name="userId">The user id</param>
        /// <param name="workflowId">The workflow id</param>
        /// <returns>True if the user can create change requests</returns>
        Task<bool> CanCreateAsync(Guid userId, Guid workflowId);

        /// <summary>
        /// Validates if a user can transition a change request to the specified stage.
        /// </summary>
        /// <param name="userId">The user id</param>
        /// <param name="changeRequestId">The change request id</param>
        /// <param name="targetStageId">The target stage id</param>
        /// <returns>True if the user can perform the transition</returns>
        Task<bool> CanTransitionAsync(Guid userId, Guid changeRequestId, Guid targetStageId);

        /// <summary>
        /// Approves a change request.
        /// </summary>
        /// <param name="id">The change request id</param>
        /// <param name="userId">The user performing the approval</param>
        /// <param name="comments">Optional approval comments</param>
        /// <returns>The updated change request</returns>
        Task<ChangeRequest> ApproveAsync(Guid id, Guid userId, string comments = null);

        /// <summary>
        /// Rejects a change request.
        /// </summary>
        /// <param name="id">The change request id</param>
        /// <param name="userId">The user performing the rejection</param>
        /// <param name="reason">The reason for rejection</param>
        /// <returns>The updated change request</returns>
        Task<ChangeRequest> RejectAsync(Guid id, Guid userId, string reason);

        /// <summary>
        /// Gets detailed information about a change request including metadata and content diff.
        /// </summary>
        /// <param name="id">The change request id</param>
        /// <returns>The detailed change request information</returns>
        Task<object> GetDetailsAsync(Guid id);
    }
}
