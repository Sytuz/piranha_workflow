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
using System.Threading.Tasks;

namespace Piranha.Manager.Services
{
    /// <summary>
    /// Service interface for resolving content type information.
    /// </summary>
    public interface IContentTypeResolutionService
    {
        /// <summary>
        /// Gets the content type name by content id.
        /// </summary>
        /// <param name="contentId">The content id</param>
        /// <returns>The content type name</returns>
        Task<string> GetContentTypeByIdAsync(Guid contentId);

        /// <summary>
        /// Gets the content title by content id.
        /// </summary>
        /// <param name="contentId">The content id</param>
        /// <returns>The content title</returns>
        Task<string> GetContentTitleByIdAsync(Guid contentId);

        /// <summary>
        /// Gets the edit URL for the given content id.
        /// </summary>
        /// <param name="contentId">The content id</param>
        /// <param name="contentType">The content type (optional)</param>
        /// <returns>The edit URL</returns>
        Task<string> GetEditUrlByContentIdAsync(Guid contentId, string contentType = null);
    }
}
