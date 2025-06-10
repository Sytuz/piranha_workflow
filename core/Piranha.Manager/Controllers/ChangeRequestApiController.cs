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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Piranha.Models;
using Piranha.Manager.Models;
using Piranha.Manager.Services;
using Piranha.Services;

namespace Piranha.Manager.Controllers
{
    /// <summary>
    /// Api controller for change request management.
    /// </summary>
    [Area("Manager")]
    [Route("manager/api/changerequest")]
    [Authorize(Policy = Permission.Admin)]
    [ApiController]
    [AutoValidateAntiforgeryToken]
    public class ChangeRequestApiController : Controller
    {
        private readonly IChangeRequestService _service;
        private readonly IWorkflowStageService _stageService;
        private readonly IUserResolutionService _userResolutionService;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="service">The change request service</param>
        /// <param name="stageService">The workflow stage service</param>
        /// <param name="userResolutionService">The user resolution service</param>
        public ChangeRequestApiController(IChangeRequestService service, IWorkflowStageService stageService, IUserResolutionService userResolutionService)
        {
            _service = service;
            _stageService = stageService;
            _userResolutionService = userResolutionService;
        }

        /// <summary>
        /// Gets all available change requests.
        /// </summary>
        /// <returns>The change requests</returns>
        [HttpGet]
        [Route("list")]
        [Authorize(Policy = Permission.ChangeRequests)]
        public async Task<IActionResult> GetList()
        {
            try
            {
                return Ok(await _service.GetAllAsync());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorMessage
                {
                    Body = $"Error retrieving change requests: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Gets change requests by workflow id.
        /// </summary>
        /// <param name="workflowId">The workflow id</param>
        /// <returns>The change requests</returns>
        [HttpGet]
        [Route("workflow/{workflowId}")]
        [Authorize(Policy = Permission.ChangeRequests)]
        public async Task<IActionResult> GetByWorkflowId(Guid workflowId)
        {
            try
            {
                return Ok(await _service.GetByWorkflowIdAsync(workflowId));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorMessage
                {
                    Body = $"Error retrieving change requests for workflow: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Gets change requests by stage id.
        /// </summary>
        /// <param name="stageId">The stage id</param>
        /// <returns>The change requests</returns>
        [HttpGet]
        [Route("stage/{stageId}")]
        [Authorize(Policy = Permission.ChangeRequests)]
        public async Task<IActionResult> GetByStageId(Guid stageId)
        {
            try
            {
                return Ok(await _service.GetByStageIdAsync(stageId));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorMessage
                {
                    Body = $"Error retrieving change requests for stage: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Gets change requests by created by user id.
        /// </summary>
        /// <param name="userId">The user id</param>
        /// <returns>The change requests</returns>
        [HttpGet]
        [Route("user/{userId}")]
        [Authorize(Policy = Permission.ChangeRequests)]
        public async Task<IActionResult> GetByCreatedByUserId(Guid userId)
        {
            try
            {
                return Ok(await _service.GetByCreatedByIdAsync(userId));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorMessage
                {
                    Body = $"Error retrieving change requests for user: {ex.Message}"
                });
            }        }

        /// <summary>
        /// Gets change requests by content id.
        /// </summary>
        /// <param name="contentId">The content id</param>
        /// <returns>The change requests</returns>
        [HttpGet]
        [Route("content/{contentId}")]
        [Authorize(Policy = Permission.ChangeRequests)]
        public async Task<IActionResult> GetByContentId(Guid contentId)
        {
            try
            {
                return Ok(await _service.GetByContentIdAsync(contentId));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorMessage
                {
                    Body = $"Error retrieving change requests for content: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Gets the change request with the specified id.
        /// </summary>
        /// <param name="id">The change request id</param>
        /// <returns>The change request</returns>
        [HttpGet]
        [Route("{id}")]
        [Authorize(Policy = Permission.ChangeRequestsEdit)]
        public async Task<IActionResult> Get(Guid id)
        {
            try
            {
                var changeRequest = await _service.GetByIdAsync(id);
                if (changeRequest == null)
                {
                    return NotFound(new ErrorMessage
                    {
                        Body = "Change request not found"
                    });
                }
                return Ok(changeRequest);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorMessage
                {
                    Body = $"Error retrieving change request: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Creates a new change request.
        /// </summary>
        /// <param name="model">The create change request model</param>
        /// <returns>The created change request</returns>
        [HttpPost]
        [Route("create")]
        [Authorize(Policy = Permission.ChangeRequestsAdd)]
        public async Task<IActionResult> Create([FromBody] CreateChangeRequestModel model)
        {
            try
            {
                if (!await _service.CanCreateAsync(model.CreatedById, model.WorkflowId))
                {
                    return BadRequest(new ErrorMessage
                    {
                        Body = "User does not have permission to create change requests for this workflow"
                    });
                }

                if (string.IsNullOrWhiteSpace(model.ContentSnapshot))
                {
                    return BadRequest(new ErrorMessage
                    {
                        Body = "Content snapshot is required"
                    });
                }

                var changeRequest = await _service.CreateAsync(
                    model.Title,
                    model.WorkflowId,
                    model.CreatedById,
                    model.ContentId,
                    model.ContentSnapshot,
                    model.Notes);

                return Ok(changeRequest);
            }
            catch (ValidationException e)
            {
                return BadRequest(new ErrorMessage { Body = e.Message });
            }
            catch (Exception e)
            {
                return StatusCode(500, new ErrorMessage
                {
                    Body = $"Error creating change request: {e.Message}"
                });
            }
        }

        /// <summary>
        /// Creates or updates the given change request.
        /// </summary>
        /// <param name="model">The change request</param>
        /// <returns>The saved change request</returns>
        [HttpPost]
        [Route("save")]
        [Authorize(Policy = Permission.ChangeRequestsSave)]
        public async Task<IActionResult> Save([FromBody] ChangeRequest model)
        {
            try
            {
                model.LastModified = DateTime.UtcNow;
                await _service.SaveAsync(model);
                return Ok(await _service.GetByIdAsync(model.Id));
            }
            catch (ValidationException e)
            {
                return BadRequest(new ErrorMessage { Body = e.Message });
            }
            catch (Exception e)
            {
                return StatusCode(500, new ErrorMessage
                {
                    Body = $"Error saving change request: {e.Message}"
                });
            }
        }

        /// <summary>
        /// Submits the change request for review.
        /// </summary>
        /// <param name="id">The change request id</param>
        /// <param name="model">The submit model</param>
        /// <returns>The updated change request</returns>
        [HttpPost]
        [Route("{id}/submit")]
        [Authorize(Policy = Permission.ChangeRequestsSubmit)]
        public async Task<IActionResult> Submit(Guid id, [FromBody] SubmitChangeRequestModel model)
        {
            try
            {
                var changeRequest = await _service.GetByIdAsync(id);
                if (changeRequest == null)
                {
                    return NotFound(new ErrorMessage
                    {
                        Body = "Change request not found"
                    });
                }

                var updatedChangeRequest = await _service.SubmitAsync(id, model.UserId);
                return Ok(updatedChangeRequest);
            }
            catch (ValidationException e)
            {
                return BadRequest(new ErrorMessage { Body = e.Message });
            }
            catch (Exception e)
            {
                return StatusCode(500, new ErrorMessage
                {
                    Body = $"Error submitting change request: {e.Message}"
                });
            }
        }

        /// <summary>
        /// Moves the change request to a specific stage.
        /// </summary>
        /// <param name="id">The change request id</param>
        /// <param name="model">The move to stage model</param>
        /// <returns>The updated change request</returns>
        [HttpPost]
        [Route("{id}/move-to-stage")]
        [Authorize(Policy = Permission.ChangeRequestsEdit)]
        public async Task<IActionResult> MoveToStage(Guid id, [FromBody] MoveToStageModel model)
        {
            try
            {
                var changeRequest = await _service.GetByIdAsync(id);
                if (changeRequest == null)
                {
                    return NotFound(new ErrorMessage
                    {
                        Body = "Change request not found"
                    });
                }

                if (!await _service.CanTransitionAsync(model.UserId, id, model.StageId))
                {
                    return BadRequest(new ErrorMessage
                    {
                        Body = "User does not have permission to move this change request to the specified stage"
                    });
                }

                var updatedChangeRequest = await _service.MoveToStageAsync(id, model.StageId, model.UserId);
                return Ok(updatedChangeRequest);
            }
            catch (ValidationException e)
            {
                return BadRequest(new ErrorMessage { Body = e.Message });
            }
            catch (Exception e)
            {
                return StatusCode(500, new ErrorMessage
                {
                    Body = $"Error moving change request to stage: {e.Message}"
                });
            }
        }

        /// <summary>
        /// Checks if a user can transition a change request to a specific stage.
        /// </summary>
        /// <param name="id">The change request id</param>
        /// <param name="stageId">The target stage id</param>
        /// <param name="userId">The user id</param>
        /// <returns>True if the transition is allowed</returns>
        [HttpGet]
        [Route("{id}/can-transition/{stageId}/{userId}")]
        [Authorize(Policy = Permission.ChangeRequests)]
        public async Task<IActionResult> CanTransition(Guid id, Guid stageId, Guid userId)
        {
            try
            {
                var canTransition = await _service.CanTransitionAsync(userId, id, stageId);
                return Ok(new { CanTransition = canTransition });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorMessage
                {
                    Body = $"Error checking transition permission: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Deletes the change request with the specified id.
        /// </summary>
        /// <param name="id">The change request id</param>
        /// <returns>The result of the operation</returns>
        [HttpDelete]
        [Route("{id}")]
        [Authorize(Policy = Permission.ChangeRequestsDelete)]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var changeRequest = await _service.GetByIdAsync(id);
                if (changeRequest == null)
                {
                    return NotFound(new ErrorMessage
                    {
                        Body = "Change request not found"
                    });
                }

                await _service.DeleteAsync(id);
                return NoContent();
            }
            catch (ValidationException e)
            {
                return BadRequest(new ErrorMessage { Body = e.Message });
            }
            catch (Exception e)
            {
                return StatusCode(500, new ErrorMessage
                {
                    Body = $"Error deleting change request: {e.Message}"
                });
            }
        }

        /// <summary>
        /// Approves the change request with the specified id.
        /// </summary>
        /// <param name="id">The change request id</param>
        /// <param name="model">The approval model</param>
        /// <returns>The approved change request</returns>
        [HttpPost]
        [Route("{id}/approve")]
        [Authorize(Policy = Permission.ChangeRequestsEdit)]
        public async Task<IActionResult> Approve(Guid id, [FromBody] ApproveChangeRequestModel model)
        {
            try
            {
                var changeRequest = await _service.GetByIdAsync(id);
                if (changeRequest == null)
                {
                    return NotFound(new ErrorMessage
                    {
                        Body = "Change request not found"
                    });
                }

                var updatedChangeRequest = await _service.ApproveAsync(id, model.UserId, model.Comments);
                return Ok(updatedChangeRequest);
            }
            catch (ValidationException e)
            {
                return BadRequest(new ErrorMessage { Body = e.Message });
            }
            catch (Exception e)
            {
                return StatusCode(500, new ErrorMessage
                {
                    Body = $"Error approving change request: {e.Message}"
                });
            }
        }

        /// <summary>
        /// Rejects the change request with the specified id.
        /// </summary>
        /// <param name="id">The change request id</param>
        /// <param name="model">The rejection model</param>
        /// <returns>The rejected change request</returns>
        [HttpPost]
        [Route("{id}/reject")]
        [Authorize(Policy = Permission.ChangeRequestsEdit)]
        public async Task<IActionResult> Reject(Guid id, [FromBody] RejectChangeRequestModel model)
        {
            try
            {
                var changeRequest = await _service.GetByIdAsync(id);
                if (changeRequest == null)
                {
                    return NotFound(new ErrorMessage
                    {
                        Body = "Change request not found"
                    });
                }

                var updatedChangeRequest = await _service.RejectAsync(id, model.UserId, model.Reason);
                return Ok(updatedChangeRequest);
            }
            catch (ValidationException e)
            {
                return BadRequest(new ErrorMessage { Body = e.Message });
            }
            catch (Exception e)
            {
                return StatusCode(500, new ErrorMessage
                {
                    Body = $"Error rejecting change request: {e.Message}"
                });
            }
        }

        /// <summary>
        /// Gets detailed information about a change request including metadata and content diff.
        /// </summary>
        /// <param name="id">The change request id</param>
        /// <returns>The detailed change request information</returns>
        [HttpGet]
        [Route("{id}/details")]
        [Authorize(Policy = Permission.ChangeRequests)]
        public async Task<IActionResult> GetDetails(Guid id)
        {
            try
            {
                var changeRequest = await _service.GetByIdAsync(id);
                if (changeRequest == null)
                {
                    return NotFound(new ErrorMessage
                    {
                        Body = "Change request not found"
                    });
                }

                var details = await _service.GetDetailsAsync(id);
                return Ok(details);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorMessage
                {
                    Body = $"Error retrieving change request details: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Gets the transition history for a change request.
        /// </summary>
        /// <param name="id">The change request id</param>
        /// <returns>The transition history</returns>
        [HttpGet]
        [Route("{id}/transitions")]
        [Authorize(Policy = Permission.ChangeRequests)]
        public async Task<IActionResult> GetTransitions(Guid id)
        {
            try
            {
                var transitions = (await _service.GetTransitionsAsync(id)).ToList();
                var stageIds = transitions.SelectMany(t => new[] { t.FromStageId, t.ToStageId }).Distinct().ToList();
                var userIds = transitions.Select(t => t.UserId).Distinct().ToList();

                // Resolve stage titles
                var stageTitles = new Dictionary<Guid, string>();
                foreach (var stageId in stageIds)
                {
                    var stage = await _stageService.GetByIdAsync(stageId);
                    stageTitles[stageId] = stage?.Title ?? "Unknown";
                }

                // Resolve user names
                var userNames = await _userResolutionService.GetUserNamesByIdsAsync(userIds);

                // Map to view model
                var result = transitions.Select(t => new ChangeRequestTransitionViewModel
                {
                    TransitionedAt = t.Timestamp,
                    FromStageTitle = stageTitles.TryGetValue(t.FromStageId, out var fromTitle) ? fromTitle : "Unknown",
                    ToStageTitle = stageTitles.TryGetValue(t.ToStageId, out var toTitle) ? toTitle : "Unknown",
                    UserName = userNames.TryGetValue(t.UserId, out var userName) ? userName : "Unknown",
                    Notes = t.CommentId.HasValue ? $"Comment: {t.CommentId}" : string.Empty, // TODO: Optionally resolve comment text
                    ActionType = t.ActionType
                }).ToList();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorMessage
                {
                    Body = $"Error retrieving transition history: {ex.Message}"
                });
            }
        }

        // Comment-related endpoints

        /// <summary>
        /// Gets all comments for a change request.
        /// </summary>
        /// <param name="id">The change request id</param>
        /// <returns>The comments for the change request</returns>
        [HttpGet]
        [Route("{id}/comments")]
        [Authorize(Policy = Permission.ChangeRequests)]
        public async Task<IActionResult> GetComments(Guid id)
        {
            try
            {
                var changeRequest = await _service.GetByIdAsync(id);
                if (changeRequest == null)
                {
                    return NotFound(new ErrorMessage
                    {
                        Body = "Change request not found"
                    });
                }

                var comments = await _service.GetCommentsAsync(id);
                return Ok(comments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorMessage
                {
                    Body = $"Error retrieving comments: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Adds a regular comment to a change request.
        /// </summary>
        /// <param name="id">The change request id</param>
        /// <param name="model">The comment model</param>
        /// <returns>The created comment</returns>
        [HttpPost]
        [Route("{id}/comments")]
        [Authorize(Policy = Permission.ChangeRequestsEdit)]
        public async Task<IActionResult> AddComment(Guid id, [FromBody] AddCommentModel model)
        {
            try
            {
                var changeRequest = await _service.GetByIdAsync(id);
                if (changeRequest == null)
                {
                    return NotFound(new ErrorMessage
                    {
                        Body = "Change request not found"
                    });
                }

                var comment = await _service.AddCommentAsync(id, model.AuthorId, model.AuthorName, model.Content);
                return Ok(comment);
            }
            catch (ValidationException e)
            {
                return BadRequest(new ErrorMessage { Body = e.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorMessage
                {
                    Body = $"Error adding comment: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Adds an approval comment to a change request.
        /// </summary>
        /// <param name="id">The change request id</param>
        /// <param name="model">The approval comment model</param>
        /// <returns>The created approval comment</returns>
        [HttpPost]
        [Route("{id}/comments/approval")]
        [Authorize(Policy = Permission.ChangeRequestsEdit)]
        public async Task<IActionResult> AddApprovalComment(Guid id, [FromBody] AddApprovalCommentModel model)
        {
            try
            {
                var changeRequest = await _service.GetByIdAsync(id);
                if (changeRequest == null)
                {
                    return NotFound(new ErrorMessage
                    {
                        Body = "Change request not found"
                    });
                }

                var comment = await _service.AddApprovalCommentAsync(id, model.AuthorId, model.AuthorName, model.Content, model.StageId);
                return Ok(comment);
            }
            catch (ValidationException e)
            {
                return BadRequest(new ErrorMessage { Body = e.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorMessage
                {
                    Body = $"Error adding approval comment: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Adds a rejection comment to a change request.
        /// </summary>
        /// <param name="id">The change request id</param>
        /// <param name="model">The rejection comment model</param>
        /// <returns>The created rejection comment</returns>
        [HttpPost]
        [Route("{id}/comments/rejection")]
        [Authorize(Policy = Permission.ChangeRequestsEdit)]
        public async Task<IActionResult> AddRejectionComment(Guid id, [FromBody] AddRejectionCommentModel model)
        {
            try
            {
                var changeRequest = await _service.GetByIdAsync(id);
                if (changeRequest == null)
                {
                    return NotFound(new ErrorMessage
                    {
                        Body = "Change request not found"
                    });
                }

                var comment = await _service.AddRejectionCommentAsync(id, model.AuthorId, model.AuthorName, model.Content, model.StageId);
                return Ok(comment);
            }
            catch (ValidationException e)
            {
                return BadRequest(new ErrorMessage { Body = e.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorMessage
                {
                    Body = $"Error adding rejection comment: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Deletes a comment by its id.
        /// </summary>
        /// <param name="commentId">The comment id</param>
        /// <returns>Success response</returns>
        [HttpDelete]
        [Route("comments/{commentId}")]
        [Authorize(Policy = Permission.ChangeRequestsEdit)]
        public async Task<IActionResult> DeleteComment(Guid commentId)
        {
            try
            {
                await _service.DeleteCommentAsync(commentId);
                return Ok(new { message = "Comment deleted successfully" });
            }
            catch (ValidationException e)
            {
                return BadRequest(new ErrorMessage { Body = e.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorMessage
                {
                    Body = $"Error deleting comment: {ex.Message}"
                });
            }
        }
    }

    /// <summary>
    /// Model for creating a new change request.
    /// </summary>
    public class CreateChangeRequestModel
    {
        /// <summary>
        /// Gets/sets the change request title.
        /// </summary>
        [Required]
        public string Title { get; set; }

        /// <summary>
        /// Gets/sets the workflow id.
        /// </summary>
        [Required]
        public Guid WorkflowId { get; set; }

        /// <summary>
        /// Gets/sets the user id who created the request.
        /// </summary>
        [Required]
        public Guid CreatedById { get; set; }

        /// <summary>
        /// Gets/sets the content id this change request is for.
        /// </summary>
        [Required]
        public Guid ContentId { get; set; }

        /// <summary>
        /// Gets/sets optional notes.
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// Gets/sets the content snapshot.
        /// </summary>
        [Required]
        public string ContentSnapshot { get; set; }
    }

    /// <summary>
    /// Model for submitting a change request.
    /// </summary>
    public class SubmitChangeRequestModel
    {
        /// <summary>
        /// Gets/sets the user id performing the submission.
        /// </summary>
        [Required]
        public Guid UserId { get; set; }
    }

    /// <summary>
    /// Model for moving a change request to a specific stage.
    /// </summary>
    public class MoveToStageModel
    {
        /// <summary>
        /// Gets/sets the target stage id.
        /// </summary>
        [Required]
        public Guid StageId { get; set; }

        /// <summary>
        /// Gets/sets the user id performing the move.
        /// </summary>
        [Required]
        public Guid UserId { get; set; }
    }

    /// <summary>
    /// Model for approving a change request.
    /// </summary>
    public class ApproveChangeRequestModel
    {
        /// <summary>
        /// Gets/sets the user id performing the approval.
        /// </summary>
        [Required]
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets/sets optional approval comments.
        /// </summary>
        public string Comments { get; set; }
    }

    /// <summary>
    /// Model for rejecting a change request.
    /// </summary>
    public class RejectChangeRequestModel
    {
        /// <summary>
        /// Gets/sets the user id performing the rejection.
        /// </summary>
        [Required]
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets/sets the reason for rejection.
        /// </summary>
        [Required]
        public string Reason { get; set; }
    }

    /// <summary>
    /// Model for detailed change request information.
    /// </summary>
    public class ChangeRequestDetailsModel
    {
        /// <summary>
        /// Gets/sets the change request.
        /// </summary>
        public ChangeRequest ChangeRequest { get; set; }

        /// <summary>
        /// Gets/sets the workflow information.
        /// </summary>
        public WorkflowInfo Workflow { get; set; }

        /// <summary>
        /// Gets/sets the current stage information.
        /// </summary>
        public StageInfo CurrentStage { get; set; }

        /// <summary>
        /// Gets/sets the content information.
        /// </summary>
        public ContentInfo Content { get; set; }

        /// <summary>
        /// Gets/sets the creator information.
        /// </summary>
        public UserInfo Creator { get; set; }

        /// <summary>
        /// Gets/sets the content diff information.
        /// </summary>
        public ContentDiffInfo ContentDiff { get; set; }

        /// <summary>
        /// Gets/sets available actions for this change request.
        /// </summary>
        public IList<AvailableAction> AvailableActions { get; set; } = new List<AvailableAction>();
    }

    /// <summary>
    /// Model for workflow information.
    /// </summary>
    public class WorkflowInfo
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }

    /// <summary>
    /// Model for stage information.
    /// </summary>
    public class StageInfo
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public IList<string> AllowedRoles { get; set; } = new List<string>();
    }

    /// <summary>
    /// Model for content information.
    /// </summary>
    public class ContentInfo
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string ContentType { get; set; }
        public string EditUrl { get; set; }
        public DateTime LastModified { get; set; }
    }

    /// <summary>
    /// Model for user information.
    /// </summary>
    public class UserInfo
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }

    /// <summary>
    /// Model for content diff information.
    /// </summary>
    public class ContentDiffInfo
    {
        public string OriginalContent { get; set; }
        public string ModifiedContent { get; set; }
        public IList<DiffChange> Changes { get; set; } = new List<DiffChange>();
    }

    /// <summary>
    /// Model for a single diff change.
    /// </summary>
    public class DiffChange
    {
        public string Type { get; set; } // "added", "removed", "modified"
        public string Field { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
    }

    /// <summary>
    /// Model for available actions.
    /// </summary>
    public class AvailableAction
    {
        public string Type { get; set; } // "approve", "reject", "move", "edit"
        public string Label { get; set; }
        public bool Enabled { get; set; }
        public string Icon { get; set; }
        public object Data { get; set; } // Additional data for the action
    }

    // Comment request/response models

    /// <summary>
    /// Model for adding a regular comment to a change request.
    /// </summary>
    public class AddCommentModel
    {
        /// <summary>
        /// Gets/sets the author id.
        /// </summary>
        [Required]
        public Guid AuthorId { get; set; }

        /// <summary>
        /// Gets/sets the author name.
        /// </summary>
        [Required]
        public string AuthorName { get; set; }

        /// <summary>
        /// Gets/sets the comment content.
        /// </summary>
        [Required]
        public string Content { get; set; }
    }

    /// <summary>
    /// Model for adding an approval comment to a change request.
    /// </summary>
    public class AddApprovalCommentModel
    {
        /// <summary>
        /// Gets/sets the author id.
        /// </summary>
        [Required]
        public Guid AuthorId { get; set; }

        /// <summary>
        /// Gets/sets the author name.
        /// </summary>
        [Required]
        public string AuthorName { get; set; }

        /// <summary>
        /// Gets/sets the comment content.
        /// </summary>
        [Required]
        public string Content { get; set; }

        /// <summary>
        /// Gets/sets the stage id where the approval was made.
        /// </summary>
        [Required]
        public Guid StageId { get; set; }
    }

    /// <summary>
    /// Model for adding a rejection comment to a change request.
    /// </summary>
    public class AddRejectionCommentModel
    {
        /// <summary>
        /// Gets/sets the author id.
        /// </summary>
        [Required]
        public Guid AuthorId { get; set; }

        /// <summary>
        /// Gets/sets the author name.
        /// </summary>
        [Required]
        public string AuthorName { get; set; }

        /// <summary>
        /// Gets/sets the comment content.
        /// </summary>
        [Required]
        public string Content { get; set; }

        /// <summary>
        /// Gets/sets the stage id where the rejection was made.
        /// </summary>
        [Required]
        public Guid StageId { get; set; }
    }

    /// <summary>
    /// Model for change request transition view.
    /// </summary>
    public class ChangeRequestTransitionViewModel
    {
        /// <summary>
        /// Gets/sets the transition timestamp.
        /// </summary>
        public DateTime TransitionedAt { get; set; }

        /// <summary>
        /// Gets/sets the title of the stage from which the request was transitioned.
        /// </summary>
        public string FromStageTitle { get; set; }

        /// <summary>
        /// Gets/sets the title of the stage to which the request was transitioned.
        /// </summary>
        public string ToStageTitle { get; set; }

        /// <summary>
        /// Gets/sets the name of the user who performed the transition.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets/sets any notes associated with the transition.
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// Gets/sets the type of action performed (e.g., approve, reject, move).
        /// </summary>
        public string ActionType { get; set; }
    }
}
