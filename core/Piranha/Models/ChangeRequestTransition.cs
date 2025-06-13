using System;
using System.ComponentModel.DataAnnotations;

namespace Piranha.Models
{
    /// <summary>
    /// Represents a transition (state change) in a change request's workflow.
    /// </summary>
    public class ChangeRequestTransition
    {
        public Guid Id { get; set; }
        [Required]
        public Guid ChangeRequestId { get; set; }
        [Required]
        public Guid FromStageId { get; set; }
        [Required]
        public Guid ToStageId { get; set; }
        [Required]
        public Guid UserId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        [Required]
        public string ActionType { get; set; } // e.g. "Approve", "Reject"
        public Guid? CommentId { get; set; } // Optional: link to approval/rejection comment
        public string ContentSnapshot { get; set; } // Optional: snapshot at this transition
    }
}
