/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Piranha.Models;
using Piranha.Security;
using System.Security.Claims;

namespace Piranha.Manager.Controllers
{
    /// <summary>
    /// API controller for my tasks.
    /// </summary>
    [Area("Manager")]
    [Route("manager/api/mytasks")]
    [Authorize(Policy = Permission.ChangeRequests)]
    [ApiController]
    [AutoValidateAntiforgeryToken]
    public class MyTasksApiController : Controller
    {
        private readonly IApi _api;
        private readonly IRoleProvider _roleProvider;
        private readonly ILogger<MyTasksApiController> _logger;

        public MyTasksApiController(IApi api, IRoleProvider roleProvider, ILogger<MyTasksApiController> logger)
        {
            _api = api;
            _roleProvider = roleProvider;
            _logger = logger;
        }

        /// <summary>
        /// Gets the tasks assigned to the current user.
        /// </summary>
        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetMyTasks()
        {
            _logger.LogDebug("Starting GetMyTasks method");

            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("User ID not found in claims");
                    return Ok(new { tasks = new List<object>() });
                }

                _logger.LogDebug($"Getting tasks for user: {userId}");

                // Get all workflows
                _logger.LogDebug("Fetching all workflows");
                var workflows = await _api.Workflows.GetAllAsync();
                _logger.LogDebug($"Retrieved {workflows?.Count() ?? 0} workflows");

                var userTasks = new List<object>();

                foreach (var workflow in workflows)
                {
                    _logger.LogDebug($"Processing workflow: {workflow.Id} - {workflow.Title}");

                    // Get change requests for this workflow
                    var changeRequests = await _api.ChangeRequests.GetByWorkflowIdAsync(workflow.Id);
                    _logger.LogDebug($"Found {changeRequests?.Count() ?? 0} change requests for workflow {workflow.Id}");

                    foreach (var changeRequest in changeRequests)
                    {
                        _logger.LogDebug($"Processing change request: {changeRequest.Id} with status {changeRequest.Status}");

                        // Skip if change request is completed
                        if (changeRequest.Status == ChangeRequestStatus.Approved)
                        {
                            _logger.LogDebug($"Skipping approved change request {changeRequest.Id}");
                            continue;
                        }

                        var isUserTask = false;
                        var availableActions = new List<string>();

                        // Check if user created this change request
                        if (changeRequest.CreatedById.ToString() == userId)
                        {
                            _logger.LogDebug($"User {userId} is creator of change request {changeRequest.Id}");
                            isUserTask = true;
                            // Creator can view details
                            availableActions.Add("view");
                        }

                        // Check if user has access to current stage
                        var currentStage = workflow.Stages.FirstOrDefault(s => s.Id == changeRequest.StageId);
                        if (currentStage != null)
                        {
                            _logger.LogDebug($"Current stage for change request {changeRequest.Id}: {currentStage.Id} - {currentStage.Title}");
                            _logger.LogDebug($"Checking {currentStage.Roles?.Count() ?? 0} roles for current stage");

                            foreach (var stageRole in currentStage.Roles)
                            {
                                try
                                {
                                    _logger.LogDebug($"Checking role {stageRole.RoleId} for user {userId}");
                                    var role = await _roleProvider.GetByIdAsync(stageRole.RoleId);
                                    if (role != null)
                                    {
                                        _logger.LogDebug($"Role {stageRole.RoleId} resolved to {role.Name}");
                                        if (User.IsInRole(role.Name))
                                        {
                                            _logger.LogDebug($"User {userId} has role {role.Name} for stage {currentStage.Id}");
                                            isUserTask = true;

                                            // Add available actions - all users with stage access can view and perform actions
                                            availableActions.Add("approve");
                                            availableActions.Add("reject");
                                            availableActions.Add("edit");
                                            availableActions.Add("view");
                                            break;
                                        }
                                        else
                                        {
                                            _logger.LogDebug($"User {userId} does not have role {role.Name}");
                                        }
                                    }
                                    else
                                    {
                                        _logger.LogWarning($"Role {stageRole.RoleId} not found");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError(ex, $"Error resolving role {stageRole.RoleId} for stage {currentStage.Id}");
                                }
                            }
                        }
                        else
                        {
                            _logger.LogWarning($"Current stage {changeRequest.StageId} not found in workflow {workflow.Id}");
                        }

                        if (isUserTask)
                        {
                            _logger.LogDebug($"Change request {changeRequest.Id} is a user task, loading content details");

                            // Get content details
                            var contentTitle = "Untitled";
                            var contentType = "Unknown";
                            var editUrl = "";

                            try
                            {
                                _logger.LogDebug($"Attempting to load page with ID {changeRequest.ContentId}");
                                // Try to determine content type by loading the content
                                var page = await _api.Pages.GetByIdAsync(changeRequest.ContentId);
                                if (page != null)
                                {
                                    _logger.LogDebug($"Content {changeRequest.ContentId} is a page: {page.Title}");
                                    contentTitle = !string.IsNullOrWhiteSpace(page.Title) ? page.Title : "Untitled Page";
                                    contentType = "Page";
                                    editUrl = $"/manager/page/edit/{page.Id}";
                                }
                                else
                                {
                                    _logger.LogDebug($"Content {changeRequest.ContentId} is not a page, trying post");
                                    var post = await _api.Posts.GetByIdAsync(changeRequest.ContentId);
                                    if (post != null)
                                    {
                                        _logger.LogDebug($"Content {changeRequest.ContentId} is a post: {post.Title}");
                                        contentTitle = !string.IsNullOrWhiteSpace(post.Title) ? post.Title : "Untitled Post";
                                        contentType = "Post";
                                        editUrl = $"/manager/post/edit/{post.BlogId}/{post.Id}";
                                    }
                                    else
                                    {
                                        _logger.LogWarning($"Content {changeRequest.ContentId} not found as page or post");
                                        contentTitle = "Unknown Content";
                                        contentType = "Unknown";
                                        editUrl = "";
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogWarning(ex, $"Could not load content details for change request {changeRequest.Id}");
                                contentTitle = "Error loading content";
                                contentType = "Unknown";
                                editUrl = "";
                            }

                            var task = new
                            {
                                id = changeRequest.Id,
                                title = !string.IsNullOrWhiteSpace(changeRequest.Title) ? changeRequest.Title : "Untitled Change Request",
                                contentTitle = contentTitle,
                                contentType = contentType,
                                workflowName = !string.IsNullOrWhiteSpace(workflow.Title) ? workflow.Title : "Unknown Workflow",
                                currentStage = currentStage?.Title ?? "Unknown Stage",
                                status = changeRequest.Status.ToString(),
                                timestamp = changeRequest.CreatedAt,
                                user = changeRequest.CreatedById.ToString() == userId ? "You" : "Other User",
                                notes = !string.IsNullOrWhiteSpace(changeRequest.Notes) ? changeRequest.Notes : null,
                                availableActions = availableActions.Distinct().ToList(),
                                editUrl = editUrl
                            };

                            userTasks.Add(task);
                            _logger.LogDebug($"Added task for change request {changeRequest.Id} with {availableActions.Distinct().Count()} available actions");
                        }
                        else
                        {
                            _logger.LogDebug($"Change request {changeRequest.Id} is not a user task for user {userId}");
                        }
                    }
                }

                _logger.LogDebug($"Found {userTasks.Count} tasks for user {userId}");

                var orderedTasks = userTasks.OrderByDescending(t => ((DateTime)((dynamic)t).timestamp)).ToList();
                _logger.LogDebug("Tasks ordered by timestamp (descending)");

                return Ok(new { tasks = orderedTasks });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving my tasks");
                return StatusCode(500, new { error = "An error occurred while retrieving your tasks." });
            }
        }

        /// <summary>
        /// Approve a change request.
        /// </summary>
        [HttpPost]
        [Route("{id:guid}/approve")]
        public async Task<IActionResult> ApproveChangeRequest(Guid id, [FromBody] ApproveRequestModel model)
        {
            try
            {
                var changeRequest = await _api.ChangeRequests.GetByIdAsync(id);
                if (changeRequest == null)
                    return NotFound();

                // Get workflow and current stage
                var workflow = await _api.Workflows.GetByIdAsync(changeRequest.WorkflowId);
                var currentStage = workflow?.Stages.FirstOrDefault(s => s.Id == changeRequest.StageId);

                if (currentStage == null)
                    return BadRequest("Invalid workflow stage");

                // Check if user can approve in this stage
                var canApprove = false;
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                foreach (var stageRole in currentStage.Roles)
                {
                    var role = await _roleProvider.GetByIdAsync(stageRole.RoleId);
                    if (role != null && User.IsInRole(role.Name))
                    {
                        canApprove = true;
                        break;
                    }
                }

                if (!canApprove)
                    return Forbid();

                await _api.ChangeRequests.ApproveAsync(id, Guid.Parse(userId), model?.Comments);

                return Ok(new { success = true, message = "Change request approved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error approving change request {id}");
                return StatusCode(500, new { error = "An error occurred while approving the change request." });
            }
        }

        /// <summary>
        /// Reject a change request.
        /// </summary>
        [HttpPost]
        [Route("{id:guid}/reject")]
        public async Task<IActionResult> RejectChangeRequest(Guid id, [FromBody] RejectRequestModel model)
        {
            try
            {
                var changeRequest = await _api.ChangeRequests.GetByIdAsync(id);
                if (changeRequest == null)
                    return NotFound();

                // Get workflow and current stage
                var workflow = await _api.Workflows.GetByIdAsync(changeRequest.WorkflowId);
                var currentStage = workflow?.Stages.FirstOrDefault(s => s.Id == changeRequest.StageId);

                if (currentStage == null)
                    return BadRequest("Invalid workflow stage");

                // Check if user can reject in this stage
                var canReject = false;
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                foreach (var stageRole in currentStage.Roles)
                {
                    var role = await _roleProvider.GetByIdAsync(stageRole.RoleId);
                    if (role != null && User.IsInRole(role.Name))
                    {
                        canReject = true;
                        break;
                    }
                }

                if (!canReject)
                    return Forbid();

                await _api.ChangeRequests.RejectAsync(id, Guid.Parse(userId), model?.Reason ?? "No reason provided");

                return Ok(new { success = true, message = "Change request rejected successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error rejecting change request {id}");
                return StatusCode(500, new { error = "An error occurred while rejecting the change request." });
            }
        }

        public class ApproveRequestModel
        {
            public string Comments { get; set; }
        }

        public class RejectRequestModel
        {
            public string Reason { get; set; }
        }
    }
}
