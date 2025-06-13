using System;
using System.ComponentModel.DataAnnotations;

namespace Piranha.Models
{
    /// <summary>
    /// Model for a change request in the editorial workflow.
    /// </summary>
    public class ChangeRequest
    {
        /// <summary>
        /// Gets or sets the unique id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the title of the change request.
        /// </summary>
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the serialized snapshot of the content at the time of the change request.
        /// </summary>
        [Required(ErrorMessage = "ContentSnapshot is required")]
        public string ContentSnapshot { get; set; }

        /// <summary>
        /// Gets or sets the workflow id this change request belongs to.
        /// </summary>
        [Required]
        public Guid WorkflowId { get; set; }

        /// <summary>
        /// Gets or sets the current stage id in the workflow.
        /// </summary>
        [Required]
        public Guid StageId { get; set; }

        /// <summary>
        /// Gets or sets the previous stage id in the workflow (can be null).
        /// </summary>
        public Guid? PreviousStageId { get; set; }

        /// <summary>
        /// Gets or sets the id of the user who created this change request.
        /// </summary>
        [Required]
        public Guid CreatedById { get; set; }

        /// <summary>
        /// Gets or sets the created date.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the last modified date.
        /// </summary>
        public DateTime LastModified { get; set; }

        /// <summary>
        /// Gets or sets the content id this change request is for (required).
        /// </summary>
        [Required(ErrorMessage = "ContentId is required")]
        public Guid ContentId { get; set; }

        /// <summary>
        /// Gets or sets the status of this change request.
        /// </summary>
        public ChangeRequestStatus Status { get; set; } = ChangeRequestStatus.Draft;

        /// <summary>
        /// Gets or sets any additional notes or comments.
        /// </summary>
        public string Notes { get; set; }
    }

    /// <summary>
    /// The status of a change request.
    /// </summary>
    public enum ChangeRequestStatus
    {
        Draft,
        InReview,
        Rejected,
        Published
    }
}
