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
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Piranha.Models;
using Piranha.Manager.Models;
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

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="service">The change request service</param>
        public ChangeRequestApiController(IChangeRequestService service)
        {
            _service = service;
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
}
