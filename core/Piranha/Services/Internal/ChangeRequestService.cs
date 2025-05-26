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
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ChangeRequest>> GetByStageIdAsync(Guid stageId)
        {
            return await _repo.GetByStageIdAsync(stageId).ConfigureAwait(false);
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
        public async Task<ChangeRequest> CreateAsync(string title, string content, Guid workflowId, Guid createdById, Guid? contentId = null)
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ValidationException("Title is required");
            }

            if (string.IsNullOrWhiteSpace(content))
            {
                throw new ValidationException("Content is required");
            }

            if (workflowId == Guid.Empty)
            {
                throw new ValidationException("WorkflowId is required");
            }

            if (createdById == Guid.Empty)
            {
                throw new ValidationException("CreatedById is required");
            }

            // Validate that the workflow exists
            var workflow = await _workflowRepo.GetByIdAsync(workflowId).ConfigureAwait(false);
            if (workflow == null)
            {
                throw new ValidationException("Specified workflow does not exist");
            }

            // Get the first stage of the workflow
            var firstStage = workflow.Stages?.OrderBy(s => s.SortOrder).FirstOrDefault();
            if (firstStage == null)
            {
                throw new ValidationException("Workflow has no stages defined");
            }

            // Create the change request
            var changeRequest = new ChangeRequest
            {
                Id = Guid.NewGuid(),
                Title = title,
                Content = content,
                WorkflowId = workflowId,
                StageId = firstStage.Id,
                CreatedById = createdById,
                ContentId = contentId,
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

            if (string.IsNullOrWhiteSpace(model.Content))
            {
                throw new ValidationException("Content is required");
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
    }
}