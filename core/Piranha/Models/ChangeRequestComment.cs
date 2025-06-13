using System;
using System.ComponentModel.DataAnnotations;

namespace Piranha.Models
{
    /// <summary>
    /// Model for a comment on a change request.
    /// </summary>
    public class ChangeRequestComment
    {
        /// <summary>
        /// Gets or sets the unique id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the change request id this comment belongs to.
        /// </summary>
        [Required]
        public Guid ChangeRequestId { get; set; }

        /// <summary>
        /// Gets or sets the id of the user who created this comment.
        /// </summary>
        [Required]
        public Guid AuthorId { get; set; }

        /// <summary>
        /// Gets or sets the name of the author who created this comment.
        /// </summary>
        [Required]
        public string AuthorName { get; set; }

        /// <summary>
        /// Gets or sets the comment content.
        /// </summary>
        [Required(ErrorMessage = "Comment content is required")]
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the created date.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets whether this comment is an approval/rejection comment.
        /// </summary>
        public bool IsApprovalComment { get; set; } = false;

        /// <summary>
        /// Gets or sets the type of approval (approval or rejection). Only relevant when IsApprovalComment is true.
        /// </summary>
        public ApprovalType? ApprovalType { get; set; }

        /// <summary>
        /// Gets or sets the stage id at which this approval/rejection was made. Only relevant when IsApprovalComment is true.
        /// </summary>
        public Guid? StageId { get; set; }
    }

    /// <summary>
    /// The type of approval comment.
    /// </summary>
    public enum ApprovalType
    {
        Approval,
        Rejection
    }
}
