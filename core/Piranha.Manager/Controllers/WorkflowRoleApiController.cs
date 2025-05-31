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

namespace Piranha.Manager.Controllers;

/// <summary>
/// Api controller for workflow role management.
/// </summary>
[Area("Manager")]
[Route("manager/api/workflow/roles")]
[Authorize(Policy = Permission.Admin)]
[ApiController]
public class WorkflowRoleApiController : Controller
{
    /// <summary>
    /// Gets all available roles for workflow assignment.
    /// </summary>
    /// <returns>The available roles</returns>
    [HttpGet]
    public IActionResult GetRoles()
    {
        try
        {
            // For now, return basic role structure
            // In a real implementation, this would connect to the identity system
            var roles = new[]
            {
                new { id = "admin", name = "Administrator" },
                new { id = "editor", name = "Editor" },
                new { id = "author", name = "Author" },
                new { id = "reviewer", name = "Reviewer" }
            };
            
            return Ok(roles);
        }
        catch (System.Exception ex)
        {
            return StatusCode(500, new { error = "Failed to load roles", details = ex.Message });
        }
    }
}
