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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Piranha.AspNetCore.Identity.Models;

namespace Piranha.AspNetCore.Identity.Controllers;

/// <summary>
/// Api controller for role management.
/// </summary>
[Area("Manager")]
[Route("manager/api/roles")]
[Authorize(Policy = Permissions.Roles)]
[ApiController]
public class RoleApiController : Controller
{
    private readonly IDb _db;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="db">The identity database context</param>
    public RoleApiController(IDb db)
    {
        _db = db;
    }

    /// <summary>
    /// Gets all available roles.
    /// </summary>
    /// <returns>The available roles</returns>
    [HttpGet]
    public IActionResult GetRoles()
    {
        try
        {
            var roleModel = RoleListModel.Get(_db);
            return Ok(roleModel.Roles);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}
