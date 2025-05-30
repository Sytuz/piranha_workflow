using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Piranha.Models;

namespace Piranha.Manager.Services
{
    /// <summary>
    /// Service for managing change requests via the manager API.
    /// </summary>
    public class ChangeRequestService
    {
        private readonly IApi _api;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        public ChangeRequestService(IApi api)
        {
            _api = api;
        }

        /// <summary>
        /// Gets all change requests.
        /// </summary>
        public async Task<IEnumerable<ChangeRequest>> GetAllAsync()
        {
            return await _api.ChangeRequests.GetAllAsync();
        }

        /// <summary>
        /// Gets a change request by id.
        /// </summary>
        public async Task<ChangeRequest> GetByIdAsync(Guid id)
        {
            return await _api.ChangeRequests.GetByIdAsync(id);
        }

        /// <summary>
        /// Gets change requests by workflow id.
        /// </summary>
        public async Task<IEnumerable<ChangeRequest>> GetByWorkflowIdAsync(Guid workflowId)
        {
            return await _api.ChangeRequests.GetByWorkflowIdAsync(workflowId);
        }

        /// <summary>
        /// Gets change requests by creator user id.
        /// </summary>
        public async Task<IEnumerable<ChangeRequest>> GetByCreatedByIdAsync(Guid userId)
        {
            return await _api.ChangeRequests.GetByCreatedByIdAsync(userId);
        }

        /// <summary>
        /// Gets change requests by stage id.
        /// </summary>
        public async Task<IEnumerable<ChangeRequest>> GetByStageIdAsync(Guid stageId)
        {
            return await _api.ChangeRequests.GetByStageIdAsync(stageId);
        }

        /// <summary>
        /// Creates a new change request.
        /// </summary>
        public async Task<ChangeRequest> CreateAsync(string title, Guid workflowId, Guid createdById, Guid contentId, string contentSnapshot, string notes = null)
        {
            return await _api.ChangeRequests.CreateAsync(title, workflowId, createdById, contentId, contentSnapshot);
        }

        /// <summary>
        /// Saves a change request.
        /// </summary>
        public async Task SaveAsync(ChangeRequest model)
        {
            await _api.ChangeRequests.SaveAsync(model);
        }

        /// <summary>
        /// Deletes a change request.
        /// </summary>
        public async Task DeleteAsync(Guid id)
        {
            await _api.ChangeRequests.DeleteAsync(id);
        }

        /// <summary>
        /// Submits a change request.
        /// </summary>
        public async Task<ChangeRequest> SubmitAsync(Guid id, Guid userId)
        {
            return await _api.ChangeRequests.SubmitAsync(id, userId);
        }

        /// <summary>
        /// Moves a change request to a stage.
        /// </summary>
        public async Task<ChangeRequest> MoveToStageAsync(Guid id, Guid stageId, Guid userId)
        {
            return await _api.ChangeRequests.MoveToStageAsync(id, stageId, userId);
        }

        /// <summary>
        /// Checks if a user can create a change request for a workflow.
        /// </summary>
        public async Task<bool> CanCreateAsync(Guid userId, Guid workflowId)
        {
            return await _api.ChangeRequests.CanCreateAsync(userId, workflowId);
        }

        /// <summary>
        /// Checks if a user can transition a change request to a stage.
        /// </summary>
        public async Task<bool> CanTransitionAsync(Guid userId, Guid changeRequestId, Guid targetStageId)
        {
            return await _api.ChangeRequests.CanTransitionAsync(userId, changeRequestId, targetStageId);
        }
    }
}
