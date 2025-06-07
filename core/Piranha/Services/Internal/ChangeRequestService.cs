/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Piranha.Models;
using Piranha.Repositories;

namespace Piranha.Services
{
    public class ChangeRequestService : IChangeRequestService
    {
        private readonly IChangeRequestRepository _repo;
        private readonly IWorkflowRepository _workflowRepo;
        private readonly IWorkflowStageRepository _stageRepo;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="repo">The change request repository</param>
        /// <param name="workflowRepo">The workflow repository</param>
        /// <param name="stageRepo">The workflow stage repository</param>
        public ChangeRequestService(IChangeRequestRepository repo, IWorkflowRepository workflowRepo, IWorkflowStageRepository stageRepo)
        {
            _repo = repo;
            _workflowRepo = workflowRepo;
            _stageRepo = stageRepo;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ChangeRequest>> GetAllAsync()
        {
            return await _repo.GetAllAsync().ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<ChangeRequest> GetByIdAsync(Guid id)
        {
            return await _repo.GetByIdAsync(id).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ChangeRequest>> GetByWorkflowIdAsync(Guid workflowId)
        {
            return await _repo.GetByWorkflowIdAsync(workflowId).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ChangeRequest>> GetByCreatedByIdAsync(Guid userId)
        {
            return await _repo.GetByCreatedByIdAsync(userId).ConfigureAwait(false);
        }        /// <inheritdoc />
        public async Task<IEnumerable<ChangeRequest>> GetByStageIdAsync(Guid stageId)
        {
            return await _repo.GetByStageIdAsync(stageId).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ChangeRequest>> GetByContentIdAsync(Guid contentId)
        {
            return await _repo.GetByContentIdAsync(contentId).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<bool> CanTransitionAsync(Guid userId, Guid changeRequestId, Guid targetStageId)
        {
            var changeRequest = await _repo.GetByIdAsync(changeRequestId).ConfigureAwait(false);
            if (changeRequest == null)
            {
                return false;
            }

            // Get the workflow to validate the stage transition
            var workflow = await _workflowRepo.GetByIdAsync(changeRequest.WorkflowId).ConfigureAwait(false);
            if (workflow == null || !workflow.IsEnabled)
            {
                return false;
            }

            // Validate that the target stage belongs to the workflow
            var targetStage = workflow.Stages?.FirstOrDefault(s => s.Id == targetStageId);
            if (targetStage == null)
            {
                return false;
            }

            // Validate that the transition is allowed
            var allowedTransition = workflow.Relations?
                .Any(r => r.SourceStageId == changeRequest.StageId && r.TargetStageId == targetStageId);

            return allowedTransition == true;
        }

        /// <inheritdoc />
        public async Task<ChangeRequest> CreateAsync(string title, Guid workflowId, Guid createdById, Guid contentId, string contentSnapshot, string notes = null)
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(title))
                throw new ValidationException("Title is required");
            if (workflowId == Guid.Empty)
                throw new ValidationException("WorkflowId is required");
            if (createdById == Guid.Empty)
                throw new ValidationException("CreatedById is required");
            if (contentId == Guid.Empty)
                throw new ValidationException("ContentId is required");
            if (string.IsNullOrWhiteSpace(contentSnapshot))
                throw new ValidationException("ContentSnapshot is required");

            // Validate that the workflow exists
            var workflow = await _workflowRepo.GetByIdAsync(workflowId).ConfigureAwait(false);
            if (workflow == null)
                throw new ValidationException("Specified workflow does not exist");

            var firstStage = workflow.Stages?.OrderBy(s => s.SortOrder).FirstOrDefault();
            if (firstStage == null)
                throw new ValidationException("Workflow has no stages defined");

            var changeRequest = new ChangeRequest
            {
                Id = Guid.NewGuid(),
                Title = title,
                WorkflowId = workflowId,
                StageId = firstStage.Id,
                CreatedById = createdById,
                ContentId = contentId,
                ContentSnapshot = contentSnapshot,
                Notes = notes,
                Status = ChangeRequestStatus.Draft,
                CreatedAt = DateTime.Now,
                LastModified = DateTime.Now
            };

            await _repo.SaveAsync(changeRequest).ConfigureAwait(false);
            return changeRequest;
        }

        /// <inheritdoc />
        public async Task SaveAsync(ChangeRequest model)
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(model.Title))
            {
                throw new ValidationException("Title is required");
            }

            if (string.IsNullOrWhiteSpace(model.ContentSnapshot))
            {
                throw new ValidationException("ContentSnapshot is required");
            }

            await _repo.SaveAsync(model).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task DeleteAsync(Guid id)
        {
            var changeRequest = await _repo.GetByIdAsync(id).ConfigureAwait(false);
            if (changeRequest == null)
            {
                throw new ValidationException("Change request not found");
            }

            // Only allow deletion of draft or rejected change requests
            if (changeRequest.Status != ChangeRequestStatus.Draft && changeRequest.Status != ChangeRequestStatus.Rejected)
            {
                throw new ValidationException("Only draft or rejected change requests can be deleted");
            }

            await _repo.DeleteAsync(id).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<ChangeRequest> SubmitAsync(Guid id, Guid userId)
        {
            var changeRequest = await _repo.GetByIdAsync(id).ConfigureAwait(false);
            if (changeRequest == null)
            {
                throw new ValidationException("Change request not found");
            }

            if (changeRequest.Status != ChangeRequestStatus.Draft)
            {
                throw new ValidationException("Only draft change requests can be submitted");
            }

            // Check if user can submit this change request
            if (changeRequest.CreatedById != userId)
            {
                throw new ValidationException("Only the creator can submit a change request");
            }

            // Update status to submitted
            changeRequest.Status = ChangeRequestStatus.Submitted;
            await _repo.SaveAsync(changeRequest).ConfigureAwait(false);
            
            return changeRequest;
        }

        /// <inheritdoc />
        public async Task<ChangeRequest> MoveToStageAsync(Guid id, Guid stageId, Guid userId)
        {
            var changeRequest = await _repo.GetByIdAsync(id).ConfigureAwait(false);
            if (changeRequest == null)
            {
                throw new ValidationException("Change request not found");
            }

            // Get the workflow to validate the stage transition
            var workflow = await _workflowRepo.GetByIdAsync(changeRequest.WorkflowId).ConfigureAwait(false);
            if (workflow == null)
            {
                throw new ValidationException("Workflow not found");
            }

            // Validate that the target stage belongs to the workflow
            var targetStage = workflow.Stages?.FirstOrDefault(s => s.Id == stageId);
            if (targetStage == null)
            {
                throw new ValidationException("Target stage does not belong to the workflow");
            }

            // Validate that the transition is allowed
            var currentStage = workflow.Stages?.FirstOrDefault(s => s.Id == changeRequest.StageId);
            if (currentStage != null)
            {
                var allowedTransition = workflow.Relations?
                    .Any(r => r.SourceStageId == changeRequest.StageId && r.TargetStageId == stageId);
                
                if (allowedTransition != true)
                {
                    throw new ValidationException($"Transition from '{currentStage.Title}' to '{targetStage.Title}' is not allowed");
                }
            }

            // Move to the target stage
            return await _repo.MoveToStageAsync(id, stageId).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<bool> CanCreateAsync(Guid userId, Guid workflowId)
        {
            // Basic validation - workflow must exist and be enabled
            var workflow = await _workflowRepo.GetByIdAsync(workflowId).ConfigureAwait(false);
            return workflow != null && workflow.IsEnabled;
        }

        /// <inheritdoc />
        public async Task<ChangeRequest> ApproveAsync(Guid id, Guid userId, string comments = null)
        {
            var changeRequest = await _repo.GetByIdAsync(id).ConfigureAwait(false);
            if (changeRequest == null)
            {
                throw new ValidationException("Change request not found");
            }

            // Validate that the change request can be approved
            if (changeRequest.Status != ChangeRequestStatus.Submitted && 
                changeRequest.Status != ChangeRequestStatus.InReview)
            {
                throw new ValidationException("Only submitted or in-review change requests can be approved");
            }

            // Update status to approved
            changeRequest.Status = ChangeRequestStatus.Approved;
            changeRequest.LastModified = DateTime.UtcNow;

            // Add approval comments to notes if provided
            if (!string.IsNullOrEmpty(comments))
            {
                changeRequest.Notes = string.IsNullOrEmpty(changeRequest.Notes) 
                    ? $"Approved: {comments}" 
                    : $"{changeRequest.Notes}\n\nApproved: {comments}";
            }

            await _repo.SaveAsync(changeRequest).ConfigureAwait(false);
            return changeRequest;
        }

        /// <inheritdoc />
        public async Task<ChangeRequest> RejectAsync(Guid id, Guid userId, string reason)
        {
            var changeRequest = await _repo.GetByIdAsync(id).ConfigureAwait(false);
            if (changeRequest == null)
            {
                throw new ValidationException("Change request not found");
            }

            // Validate that the change request can be rejected
            if (changeRequest.Status != ChangeRequestStatus.Submitted && 
                changeRequest.Status != ChangeRequestStatus.InReview)
            {
                throw new ValidationException("Only submitted or in-review change requests can be rejected");
            }

            if (string.IsNullOrWhiteSpace(reason))
            {
                throw new ValidationException("Rejection reason is required");
            }

            // Update status to rejected
            changeRequest.Status = ChangeRequestStatus.Rejected;
            changeRequest.LastModified = DateTime.UtcNow;

            // Add rejection reason to notes
            changeRequest.Notes = string.IsNullOrEmpty(changeRequest.Notes) 
                ? $"Rejected: {reason}" 
                : $"{changeRequest.Notes}\n\nRejected: {reason}";

            await _repo.SaveAsync(changeRequest).ConfigureAwait(false);
            return changeRequest;
        }

        /// <inheritdoc />
        public async Task<object> GetDetailsAsync(Guid id)
        {
            var changeRequest = await _repo.GetByIdAsync(id).ConfigureAwait(false);
            if (changeRequest == null)
            {
                return null;
            }

            // Get workflow information
            var workflow = await _workflowRepo.GetByIdAsync(changeRequest.WorkflowId).ConfigureAwait(false);
            var stage = workflow?.Stages?.FirstOrDefault(s => s.Id == changeRequest.StageId);

            // Build detailed information object
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