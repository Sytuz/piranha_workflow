using System;
using System.ComponentModel.DataAnnotations;

namespace Piranha.Data
{
    /// <summary>
    /// Data entity for a change request comment.
    /// </summary>
    [Serializable]
    public sealed class ChangeRequestComment
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
        /// Gets or sets the name of the author.
        /// </summary>
        [Required]
        [StringLength(128)]
        public string AuthorName { get; set; }

        /// <summary>
        /// Gets or sets the content of the comment.
        /// </summary>
        [Required]
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the created date.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets whether this is an approval/rejection comment.
        /// </summary>
        public bool IsApprovalComment { get; set; }

        /// <summary>
        /// Gets or sets the type of approval (Approved, Rejected, null for regular comments).
        /// </summary>
        [StringLength(50)]
        public string ApprovalType { get; set; }

        /// <summary>
        /// Gets or sets the stage id where this comment was made.
        /// </summary>
        public Guid? StageId { get; set; }

        /// <summary>
        /// Gets or sets the change request navigation property.
        /// </summary>
        public ChangeRequest ChangeRequest { get; set; }

        /// <summary>
        /// Gets or sets the stage navigation property.
        /// </summary>
        public WorkflowStage Stage { get; set; }
    }
}
