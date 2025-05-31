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
    /// Default implementation of content type resolution service.
    /// </summary>
    public class DefaultContentTypeResolutionService : IContentTypeResolutionService
    {
        private readonly IApi _api;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        public DefaultContentTypeResolutionService(IApi api)
        {
            _api = api;
        }

        /// <summary>
        /// Gets the content type name by content id.
        /// </summary>
        /// <param name="contentId">The content id</param>
        /// <returns>The content type name</returns>
        public async Task<string> GetContentTypeByIdAsync(Guid contentId)
        {
            try
            {
                // Try to find as a page first
                var page = await _api.Pages.GetByIdAsync<Piranha.Models.PageInfo>(contentId);
                if (page != null)
                {
                    return "Page";
                }

                // Try to find as a post
                var post = await _api.Posts.GetByIdAsync<Piranha.Models.PostInfo>(contentId);
                if (post != null)
                {
                    return "Post";
                }

                // Try to find as content
                var content = await _api.Content.GetByIdAsync<Piranha.Models.ContentInfo>(contentId);
                if (content != null)
                {
                    return "Content";
                }

                return "Unknown";
            }
            catch
            {
                return "Unknown";
            }
        }

        /// <summary>
        /// Gets the content title by content id.
        /// </summary>
        /// <param name="contentId">The content id</param>
        /// <returns>The content title</returns>
        public async Task<string> GetContentTitleByIdAsync(Guid contentId)
        {
            try
            {
                // Try to find as a page first
                var page = await _api.Pages.GetByIdAsync<Piranha.Models.PageInfo>(contentId);
                if (page != null)
                {
                    return page.Title;
                }

                // Try to find as a post
                var post = await _api.Posts.GetByIdAsync<Piranha.Models.PostInfo>(contentId);
                if (post != null)
                {
                    return post.Title;
                }

                // Try to find as content
                var content = await _api.Content.GetByIdAsync<Piranha.Models.ContentInfo>(contentId);
                if (content != null)
                {
                    return content.Title;
                }

                return "Unknown Content";
            }
            catch
            {
                return "Unknown Content";
            }
        }

        /// <summary>
        /// Gets the edit URL for the given content id.
        /// </summary>
        /// <param name="contentId">The content id</param>
        /// <param name="contentType">The content type (optional)</param>
        /// <returns>The edit URL</returns>
        public async Task<string> GetEditUrlByContentIdAsync(Guid contentId, string contentType = null)
        {
            try
            {
                if (string.IsNullOrEmpty(contentType))
                {
                    contentType = await GetContentTypeByIdAsync(contentId);
                }

                return contentType?.ToLower() switch
                {
                    "page" => $"/manager/page/edit/{contentId}",
                    "post" => $"/manager/post/edit/{contentId}",
                    "content" => $"/manager/content/edit/{contentId}",
                    _ => "#"
                };
            }
            catch
            {
                return "#";
            }
        }
    }
}
