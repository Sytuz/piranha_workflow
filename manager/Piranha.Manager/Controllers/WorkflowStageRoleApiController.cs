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
using Piranha.Models;

namespace Piranha.Manager.Controllers;

/// <summary>
/// Api controller for workflow stage role management.
/// </summary>
[Area("Manager")]
[Route("manager/api/workflow-stage-roles")]
[Authorize(Policy = Permissions.Admin)]
[ApiController]
public class WorkflowStageRoleApiController : Controller
{
    private readonly IApi _api;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="api">The current api</param>
    public WorkflowStageRoleApiController(IApi api)
    {
        _api = api;
    }

    /// <summary>
    /// Gets all roles assigned to a specific workflow stage.
    /// </summary>
    /// <param name="workflowStageId">The workflow stage id</param>
    /// <returns>The workflow stage roles</returns>
    [HttpGet("by-stage/{workflowStageId:guid}")]
    public async Task<IActionResult> GetByWorkflowStageAsync(Guid workflowStageId)
    {
        try
        {
            var roles = await _api.WorkflowStageRoles.GetByWorkflowStageIdAsync(workflowStageId);
            return Ok(roles);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    /// <summary>
    /// Assigns a role to a workflow stage.
    /// </summary>
    /// <param name="model">The workflow stage role model</param>
    /// <returns>The created workflow stage role</returns>
    [HttpPost]
    public async Task<IActionResult> AssignRoleAsync([FromBody] WorkflowStageRole model)
    {
        try
        {
            if (model == null)
            {
                return BadRequest("Invalid workflow stage role data");
            }

            // Check if role is already assigned
            var isAssigned = await _api.WorkflowStageRoles.IsRoleAssignedAsync(model.WorkflowStageId, model.RoleId);
            if (isAssigned)
            {
                return Conflict("Role is already assigned to this workflow stage");
            }

            await _api.WorkflowStageRoles.SaveAsync(model);
            return Ok(model);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    /// <summary>
    /// Removes a role assignment from a workflow stage.
    /// </summary>
    /// <param name="id">The workflow stage role id</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> UnassignRoleAsync(Guid id)
    {
        try
        {
            await _api.WorkflowStageRoles.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    /// <summary>
    /// Removes all role assignments from a workflow stage.
    /// </summary>
    /// <param name="workflowStageId">The workflow stage id</param>
    /// <returns>No content on success</returns>
    [HttpDelete("by-stage/{workflowStageId:guid}")]
    public async Task<IActionResult> UnassignAllRolesAsync(Guid workflowStageId)
    {
        try
        {
            await _api.WorkflowStageRoles.DeleteByWorkflowStageIdAsync(workflowStageId);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    /// <summary>
    /// Checks if a role is assigned to a workflow stage.
    /// </summary>
    /// <param name="workflowStageId">The workflow stage id</param>
    /// <param name="roleId">The role id</param>
    /// <returns>True if the role is assigned</returns>
    [HttpGet("check/{workflowStageId:guid}/{roleId}")]
    public async Task<IActionResult> IsRoleAssignedAsync(Guid workflowStageId, string roleId)
    {
        try
        {
            var isAssigned = await _api.WorkflowStageRoles.IsRoleAssignedAsync(workflowStageId, roleId);
            return Ok(new { IsAssigned = isAssigned });
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}
