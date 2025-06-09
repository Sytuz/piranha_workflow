/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Microsoft.EntityFrameworkCore;
using Piranha.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Piranha.Repositories
{
    public class ChangeRequestCommentRepository : IChangeRequestCommentRepository
    {
        private readonly IDb _db;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="db">The current db context</param>
        public ChangeRequestCommentRepository(IDb db)
        {
            _db = db;
        }

        /// <summary>
        /// Gets all comments for the specified change request.
        /// </summary>
        /// <param name="changeRequestId">The change request id</param>
        /// <returns>The comments</returns>
        public async Task<IEnumerable<Models.ChangeRequestComment>> GetByChangeRequestIdAsync(Guid changeRequestId)
        {
            return await _db.ChangeRequestComments
                .AsNoTracking()
                .Where(c => c.ChangeRequestId == changeRequestId)
                .OrderBy(c => c.CreatedAt)
                .Select(c => new Models.ChangeRequestComment
                {
                    Id = c.Id,
                    ChangeRequestId = c.ChangeRequestId,
                    AuthorId = c.AuthorId,
                    AuthorName = c.AuthorName,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                    IsApprovalComment = c.IsApprovalComment,
                    ApprovalType = string.IsNullOrEmpty(c.ApprovalType) ? null : Enum.Parse<Models.ApprovalType>(c.ApprovalType),
                    StageId = c.StageId
                })
                .ToListAsync();
        }

        /// <summary>
        /// Gets all approval comments for the specified change request.
        /// </summary>
        /// <param name="changeRequestId">The change request id</param>
        /// <returns>The approval comments</returns>
        public async Task<IEnumerable<Models.ChangeRequestComment>> GetApprovalCommentsAsync(Guid changeRequestId)
        {
            return await _db.ChangeRequestComments
                .AsNoTracking()
                .Where(c => c.ChangeRequestId == changeRequestId && c.IsApprovalComment)
                .OrderBy(c => c.CreatedAt)
                .Select(c => new Models.ChangeRequestComment
                {
                    Id = c.Id,
                    ChangeRequestId = c.ChangeRequestId,
                    AuthorId = c.AuthorId,
                    AuthorName = c.AuthorName,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                    IsApprovalComment = c.IsApprovalComment,
                    ApprovalType = string.IsNullOrEmpty(c.ApprovalType) ? null : Enum.Parse<Models.ApprovalType>(c.ApprovalType),
                    StageId = c.StageId
                })
                .ToListAsync();
        }

        /// <summary>
        /// Gets all comments by the specified author.
        /// </summary>
        /// <param name="authorId">The author id</param>
        /// <returns>The comments</returns>
        public async Task<IEnumerable<Models.ChangeRequestComment>> GetByAuthorIdAsync(Guid authorId)
        {
            return await _db.ChangeRequestComments
                .AsNoTracking()
                .Where(c => c.AuthorId == authorId)
                .OrderBy(c => c.CreatedAt)
                .Select(c => new Models.ChangeRequestComment
                {
                    Id = c.Id,
                    ChangeRequestId = c.ChangeRequestId,
                    AuthorId = c.AuthorId,
                    AuthorName = c.AuthorName,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                    IsApprovalComment = c.IsApprovalComment,
                    ApprovalType = string.IsNullOrEmpty(c.ApprovalType) ? null : Enum.Parse<Models.ApprovalType>(c.ApprovalType),
                    StageId = c.StageId
                })
                .ToListAsync();
        }

        /// <summary>
        /// Gets the comment with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The comment</returns>
        public async Task<Models.ChangeRequestComment> GetByIdAsync(Guid id)
        {
            var comment = await _db.ChangeRequestComments
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (comment != null)
            {
                return new Models.ChangeRequestComment
                {
                    Id = comment.Id,
                    ChangeRequestId = comment.ChangeRequestId,
                    AuthorId = comment.AuthorId,
                    AuthorName = comment.AuthorName,
                    Content = comment.Content,
                    CreatedAt = comment.CreatedAt,
                    IsApprovalComment = comment.IsApprovalComment,
                    ApprovalType = string.IsNullOrEmpty(comment.ApprovalType) ? null : Enum.Parse<Models.ApprovalType>(comment.ApprovalType),
                    StageId = comment.StageId
                };
            }
            return null;
        }

        /// <summary>
        /// Adds or updates the given comment.
        /// </summary>
        /// <param name="comment">The comment</param>
        public async Task SaveAsync(Models.ChangeRequestComment comment)
        {
            var entity = await _db.ChangeRequestComments
                .FirstOrDefaultAsync(c => c.Id == comment.Id);

            if (entity == null)
            {
                entity = new Data.ChangeRequestComment
                {
                    Id = comment.Id != Guid.Empty ? comment.Id : Guid.NewGuid(),
                    CreatedAt = DateTime.Now
                };
                await _db.ChangeRequestComments.AddAsync(entity);
            }

            entity.ChangeRequestId = comment.ChangeRequestId;
            entity.AuthorId = comment.AuthorId;
            entity.AuthorName = comment.AuthorName;
            entity.Content = comment.Content;
            entity.IsApprovalComment = comment.IsApprovalComment;
            entity.ApprovalType = comment.ApprovalType?.ToString();
            entity.StageId = comment.StageId;

            await _db.SaveChangesAsync();

            // Update the model with the saved entity values
            comment.Id = entity.Id;
            comment.CreatedAt = entity.CreatedAt;
        }

        /// <summary>
        /// Deletes the comment with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        public async Task DeleteAsync(Guid id)
        {
            var entity = await _db.ChangeRequestComments
                .FirstOrDefaultAsync(c => c.Id == id);

            if (entity != null)
            {
                _db.ChangeRequestComments.Remove(entity);
                await _db.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Deletes the given comment.
        /// </summary>
        /// <param name="comment">The comment</param>
        public async Task DeleteAsync(Models.ChangeRequestComment comment)
        {
            await DeleteAsync(comment.Id);
        }
    }
}
