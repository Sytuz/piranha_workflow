using Piranha.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Piranha.Repositories
{
    /// <summary>
    /// Repository interface for change request comments.
    /// </summary>
    public interface IChangeRequestCommentRepository
    {
        /// <summary>
        /// Gets all comments for the specified change request.
        /// </summary>
        /// <param name="changeRequestId">The change request id</param>
        /// <returns>The comments</returns>
        Task<IEnumerable<ChangeRequestComment>> GetByChangeRequestIdAsync(Guid changeRequestId);

        /// <summary>
        /// Gets the comment with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The comment</returns>
        Task<ChangeRequestComment> GetByIdAsync(Guid id);

        /// <summary>
        /// Gets all approval/rejection comments for a change request.
        /// </summary>
        /// <param name="changeRequestId">The change request id</param>
        /// <returns>The approval/rejection comments</returns>
        Task<IEnumerable<ChangeRequestComment>> GetApprovalCommentsAsync(Guid changeRequestId);

        /// <summary>
        /// Gets comments by author.
        /// </summary>
        /// <param name="authorId">The author id</param>
        /// <returns>The comments</returns>
        Task<IEnumerable<ChangeRequestComment>> GetByAuthorIdAsync(Guid authorId);

        /// <summary>
        /// Saves the given comment.
        /// </summary>
        /// <param name="model">The comment</param>
        Task SaveAsync(ChangeRequestComment model);

        /// <summary>
        /// Deletes the comment with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        Task DeleteAsync(Guid id);

        /// <summary>
        /// Deletes the given comment.
        /// </summary>
        /// <param name="model">The comment</param>
        Task DeleteAsync(ChangeRequestComment model);
    }
}
