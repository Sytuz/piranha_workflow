using System;
using System.ComponentModel.DataAnnotations;

namespace Piranha.Data
{
    /// <summary>
    /// Data entity for a change request transition (state change).
    /// </summary>
    [Serializable]
    public sealed class ChangeRequestTransition
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
        [StringLength(32)]
        public string ActionType { get; set; } // "Approve", "Reject"
        public Guid? CommentId { get; set; }
        public string ContentSnapshot { get; set; }
        // Navigation properties
        public ChangeRequest ChangeRequest { get; set; }
    }
}
