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
using System.Threading.Tasks;

namespace Piranha.Manager.Services
{
    /// <summary>
    /// Service interface for resolving user information.
    /// </summary>
    public interface IUserResolutionService
    {
        /// <summary>
        /// Gets a user name by user id.
        /// </summary>
        /// <param name="userId">The user id</param>
        /// <returns>The user name or "Unknown User" if not found</returns>
        Task<string> GetUserNameByIdAsync(Guid userId);

        /// <summary>
        /// Gets multiple user names by user ids.
        /// </summary>
        /// <param name="userIds">The user ids</param>
        /// <returns>A dictionary of user id to user name mappings</returns>
        Task<Dictionary<Guid, string>> GetUserNamesByIdsAsync(IEnumerable<Guid> userIds);
    }
}
