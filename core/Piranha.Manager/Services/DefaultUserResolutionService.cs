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
using System.Linq;
using System.Threading.Tasks;

namespace Piranha.Manager.Services
{
    /// <summary>
    /// Default implementation of user resolution service.
    /// This is a simple implementation that can be overridden by applications
    /// that use the Identity module to provide actual user name resolution.
    /// </summary>
    public class DefaultUserResolutionService : IUserResolutionService
    {
        /// <summary>
        /// Gets a user name by user id.
        /// This default implementation returns "System" for empty GUID and "User" for others.
        /// </summary>
        /// <param name="userId">The user id</param>
        /// <returns>The user name or "Unknown User" if not found</returns>
        public Task<string> GetUserNameByIdAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                return Task.FromResult("System");
            
            // This is a placeholder implementation
            // Applications using Identity should override this service
            return Task.FromResult($"User-{userId.ToString()[..8]}");
        }

        /// <summary>
        /// Gets multiple user names by user ids.
        /// </summary>
        /// <param name="userIds">The user ids</param>
        /// <returns>A dictionary of user id to user name mappings</returns>
        public async Task<Dictionary<Guid, string>> GetUserNamesByIdsAsync(IEnumerable<Guid> userIds)
        {
            var result = new Dictionary<Guid, string>();
            
            foreach (var userId in userIds.Distinct())
            {
                result[userId] = await GetUserNameByIdAsync(userId);
            }
            
            return result;
        }
    }
}
