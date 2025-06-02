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
/// Represents a role item in the system.
/// </summary>
public class RoleItem
{
    /// <summary>
    /// Gets/sets the role ID.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Gets/sets the role name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets/sets the role description.
    /// </summary>
    public string Description { get; set; }
}
