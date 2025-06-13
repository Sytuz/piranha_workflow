/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Microsoft.AspNetCore.Identity;
using Piranha.AspNetCore.Identity.Data;
using Piranha.Security;

namespace Piranha.AspNetCore.Identity.Security;

/// <summary>
/// Identity role provider implementation.
/// </summary>
public class IdentityRoleProvider : IRoleProvider
{
    private readonly RoleManager<Role> _roleManager;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="roleManager">The role manager</param>
    public IdentityRoleProvider(RoleManager<Role> roleManager)
    {
        _roleManager = roleManager;
    }    /// <summary>
    /// Gets all available roles.
    /// </summary>
    /// <returns>The available roles</returns>
    public async Task<IEnumerable<RoleItem>> GetAllAsync()
    {
        var roles = await Task.FromResult(_roleManager.Roles.ToList());
        return roles.Select(r => new RoleItem
        {
            Id = r.Id.ToString(),
            Name = r.Name,
            Description = r.NormalizedName
        });
    }    /// <summary>
    /// Gets the role with the specified id.
    /// </summary>
    /// <param name="roleId">The role id</param>
    /// <returns>The role if found</returns>
    public async Task<RoleItem> GetByIdAsync(string roleId)
    {
        var role = await _roleManager.FindByIdAsync(roleId);
        if (role != null)
        {
            return new RoleItem
            {
                Id = role.Id.ToString(),
                Name = role.Name,
                Description = role.NormalizedName
            };
        }
        return null;
    }
}
