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
/// Interface for providing role information to the workflow system.
/// </summary>
public interface IRoleProvider
{
    /// <summary>
    /// Gets all available roles.
    /// </summary>
    /// <returns>A list of role items</returns>
    Task<IEnumerable<RoleItem>> GetAllAsync();

    /// <summary>
    /// Gets a role by its ID.
    /// </summary>
    /// <param name="roleId">The role ID</param>
    /// <returns>The role item if found</returns>
    Task<RoleItem> GetByIdAsync(string roleId);
}
