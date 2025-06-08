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
        private readonly RoleManager _roleManager;
        private readonly ILogger<MyTasksApiController> _logger;

        public MyTasksApiController(IApi api, RoleManager roleManager, ILogger<MyTasksApiController> logger)
        {
            _api = api;
            _roleManager = roleManager;
            _logger = logger;
        }

        /// <summary>
        /// Gets the tasks assigned to the current user.
        /// </summary>
        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetMyTasks()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("User ID not found in claims");
                    return Ok(new { tasks = new List<object>() });
                }

                _logger.LogInformation($"Getting tasks for user: {userId}");

                // Get all workflows
                var workflows = await _api.Workflows.GetAllAsync();
                var userTasks = new List<object>();

                foreach (var workflow in workflows)
                {
                    // Get change requests for this workflow
                    var changeRequests = await _api.ChangeRequests.GetByWorkflowIdAsync(workflow.Id);
                    
                    foreach (var changeRequest in changeRequests)
                    {
                        // Skip if change request is completed
                        if (changeRequest.Status == ChangeRequestStatus.Approved)
                            continue;

                        var isUserTask = false;
                        var availableActions = new List<string>();

                        // Check if user created this change request
                        if (changeRequest.CreatedById.ToString() == userId)
                        {
                            isUserTask = true;
                            // Creator can view details
                            availableActions.Add("view");
                        }

                        // Check if user has access to current stage
                        var currentStage = workflow.Stages.FirstOrDefault(s => s.Id == changeRequest.StageId);
                        if (currentStage != null)
                        {
                            foreach (var stageRole in currentStage.Roles)
                            {
                                try
                                {
                                    var role = await _roleManager.GetByIdAsync(stageRole.RoleId);
                                    if (role != null && User.IsInRole(role.Name))
                                    {
                                        isUserTask = true;
                                        
                                        // Add available actions - all users with stage access can view and perform actions
                                        availableActions.Add("approve");
                                        availableActions.Add("reject");
                                        availableActions.Add("edit");
                                        availableActions.Add("view");
                                        break;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError(ex, $"Error resolving role {stageRole.RoleId} for stage {currentStage.Id}");
                                }
                            }
                        }

                        if (isUserTask)
                        {
                            // Get content details
                            var contentTitle = "Unknown";
                            var contentType = "Unknown";
                            var editUrl = "";

                            try
                            {
                                // Try to determine content type by loading the content
                                var page = await _api.Pages.GetByIdAsync(changeRequest.ContentId);
                                if (page != null)
                                {
                                    contentTitle = page.Title;
                                    contentType = "Page";
                                    editUrl = $"/manager/page/edit/{page.Id}";
                                }
                                else
                                {
                                    var post = await _api.Posts.GetByIdAsync(changeRequest.ContentId);
                                    if (post != null)
                                    {
                                        contentTitle = post.Title;
                                        contentType = "Post";
                                        editUrl = $"/manager/post/edit/{post.BlogId}/{post.Id}";
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogWarning(ex, $"Could not load content details for change request {changeRequest.Id}");
                            }

                            userTasks.Add(new
                            {
                                id = changeRequest.Id,
                                title = changeRequest.Title ?? "Untitled Change Request",
                                contentTitle = contentTitle,
                                contentType = contentType,
                                workflowName = workflow.Title,
                                currentStage = currentStage?.Title ?? "Unknown Stage",
                                status = changeRequest.Status.ToString(),
                                timestamp = changeRequest.CreatedAt,
                                user = changeRequest.CreatedById.ToString() == userId ? "You" : "Other User",
                                notes = changeRequest.Notes,
                                availableActions = availableActions.Distinct().ToList(),
                                editUrl = editUrl
                            });
                        }
                    }
                }

                _logger.LogInformation($"Found {userTasks.Count} tasks for user {userId}");

                return Ok(new { tasks = userTasks.OrderByDescending(t => ((DateTime)((dynamic)t).timestamp)) });
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
                    var role = await _roleManager.GetByIdAsync(stageRole.RoleId);
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
                    var role = await _roleManager.GetByIdAsync(stageRole.RoleId);
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
