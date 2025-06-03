using System;
using System.Collections.Generic;
using System.Linq;
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
        }        /// <summary>
        /// Gets change requests by stage id.
        /// </summary>
        public async Task<IEnumerable<ChangeRequest>> GetByStageIdAsync(Guid stageId)
        {
            return await _api.ChangeRequests.GetByStageIdAsync(stageId);
        }

        /// <summary>
        /// Gets change requests by content id.
        /// </summary>
        public async Task<IEnumerable<ChangeRequest>> GetByContentIdAsync(Guid contentId)
        {
            return await _api.ChangeRequests.GetByContentIdAsync(contentId);
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

        /// <summary>
        /// Approves a change request.
        /// </summary>
        public async Task<ChangeRequest> ApproveAsync(Guid id, Guid userId, string comments = null)
        {
            var changeRequest = await _api.ChangeRequests.GetByIdAsync(id);
            if (changeRequest == null)
                throw new ArgumentException("Change request not found");

            changeRequest.Status = ChangeRequestStatus.Approved;
            changeRequest.LastModified = DateTime.UtcNow;
            
            // Add approval comments to notes if provided
            if (!string.IsNullOrEmpty(comments))
            {
                changeRequest.Notes = string.IsNullOrEmpty(changeRequest.Notes) 
                    ? $"Approved: {comments}" 
                    : $"{changeRequest.Notes}\n\nApproved: {comments}";
            }

            await _api.ChangeRequests.SaveAsync(changeRequest);
            return changeRequest;
        }

        /// <summary>
        /// Rejects a change request.
        /// </summary>
        public async Task<ChangeRequest> RejectAsync(Guid id, Guid userId, string reason)
        {
            var changeRequest = await _api.ChangeRequests.GetByIdAsync(id);
            if (changeRequest == null)
                throw new ArgumentException("Change request not found");

            changeRequest.Status = ChangeRequestStatus.Rejected;
            changeRequest.LastModified = DateTime.UtcNow;
            
            // Add rejection reason to notes
            changeRequest.Notes = string.IsNullOrEmpty(changeRequest.Notes) 
                ? $"Rejected: {reason}" 
                : $"{changeRequest.Notes}\n\nRejected: {reason}";

            await _api.ChangeRequests.SaveAsync(changeRequest);
            return changeRequest;
        }

        /// <summary>
        /// Gets detailed information about a change request.
        /// </summary>
        public async Task<object> GetDetailsAsync(Guid id)
        {
            var changeRequest = await _api.ChangeRequests.GetByIdAsync(id);
            if (changeRequest == null)
                return null;

            // Get workflow information
            var workflow = await _api.Workflows.GetByIdAsync(changeRequest.WorkflowId);
            var stage = workflow?.Stages?.FirstOrDefault(s => s.Id == changeRequest.StageId);

            // Build a simplified details object
            // Note: In a real implementation, you'd want to resolve actual content diffs,
            // user information, etc. For now, we'll return basic information.
            return new
            {
                ChangeRequest = changeRequest,
                Workflow = workflow != null ? new
                {
                    Id = workflow.Id,
                    Title = workflow.Title,
                    Description = workflow.Description
                } : null,
                CurrentStage = stage != null ? new
                {
                    Id = stage.Id,
                    Title = stage.Title,
                    Description = stage.Description
                } : null,
                Content = new
                {
                    Id = changeRequest.ContentId,
                    Title = changeRequest.Title,
                    ContentType = "Unknown", // Would need content type resolution
                    EditUrl = $"/manager/page/edit/{changeRequest.ContentId}", // Basic URL pattern
                    LastModified = changeRequest.LastModified
                },
                Creator = new
                {
                    Id = changeRequest.CreatedById,
                    Name = "Unknown User", // Would need user resolution
                    Email = ""
                },
                ContentDiff = new
                {
                    OriginalContent = "", // Would need to resolve original content
                    ModifiedContent = changeRequest.ContentSnapshot,
                    Changes = new List<object>() // Would need diff computation
                },
                AvailableActions = GetAvailableActions(changeRequest)
            };
        }

        /// <summary>
        /// Gets available actions for a change request based on its current status.
        /// </summary>
        private List<object> GetAvailableActions(ChangeRequest changeRequest)
        {
            var actions = new List<object>();

            switch (changeRequest.Status)
            {
                case ChangeRequestStatus.Submitted:
                case ChangeRequestStatus.InReview:
                    actions.Add(new
                    {
                        Type = "approve",
                        Label = "Approve",
                        Enabled = true,
                        Icon = "fas fa-check",
                        Data = new { }
                    });
                    actions.Add(new
                    {
                        Type = "reject",
                        Label = "Reject",
                        Enabled = true,
                        Icon = "fas fa-times",
                        Data = new { }
                    });
                    break;
                case ChangeRequestStatus.Draft:
                    actions.Add(new
                    {
                        Type = "edit",
                        Label = "Edit",
                        Enabled = true,
                        Icon = "fas fa-edit",
                        Data = new { }
                    });
                    break;
            }

            return actions;
        }
    }
}
