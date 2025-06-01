/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

namespace Piranha.Security;

/// <summary>
/// The role manager handles role providers across modules.
/// </summary>
public class RoleManager
{
    private IRoleProvider _provider;

    /// <summary>
    /// Sets the role provider for the system.
    /// </summary>
    /// <param name="provider">The role provider</param>
    public void SetProvider(IRoleProvider provider)
    {
        _provider = provider;
    }

    /// <summary>
    /// Gets all available roles.
    /// </summary>
    /// <returns>A list of role items</returns>
    public async Task<IEnumerable<RoleItem>> GetAllAsync()
    {
        if (_provider == null)
        {
            return Enumerable.Empty<RoleItem>();
        }

        return await _provider.GetAllAsync();
    }

    /// <summary>
    /// Gets a role by its ID.
    /// </summary>
    /// <param name="roleId">The role ID</param>
    /// <returns>The role item if found</returns>
    public async Task<RoleItem> GetByIdAsync(string roleId)
    {
        if (_provider == null)
        {
            return null;
        }

        return await _provider.GetByIdAsync(roleId);
    }
}
