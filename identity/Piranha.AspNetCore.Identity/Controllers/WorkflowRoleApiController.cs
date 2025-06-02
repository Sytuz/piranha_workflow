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
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Piranha.AspNetCore.Identity.Models;

namespace Piranha.AspNetCore.Identity.Controllers;

/// <summary>
/// Api controller for workflow role management integration.
/// </summary>
[Area("Manager")]
[Route("manager/api/workflow/roles")]
[Authorize(Policy = Permissions.Roles)]
[ApiController]
public class WorkflowRoleApiController : Controller
{
    private readonly IDb _db;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="db">The identity database context</param>
    public WorkflowRoleApiController(IDb db)
    {
        _db = db;
    }    /// <summary>
    /// Gets all available roles for workflow assignment.
    /// </summary>
    /// <returns>The available roles formatted for workflow use</returns>
    [HttpGet]
    public IActionResult GetRoles()
    {
        try
        {
            var roles = _db.Roles
                .OrderBy(r => r.Name)
                .Select(r => new 
                { 
                    id = r.Id.ToString(),
                    name = r.Name
                })
                .ToList();
            
            return Ok(roles);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to load roles", details = ex.Message });
        }
    }
}
