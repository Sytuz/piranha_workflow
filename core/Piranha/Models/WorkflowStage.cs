using System;
using System.ComponentModel.DataAnnotations;

namespace Piranha.Models
{
    /// <summary>
    /// Model for a workflow stage.
    /// </summary>
    public class WorkflowStage
    {
        /// <summary>
        /// Gets or sets the unique id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the workflow id.
        /// </summary>
        public Guid WorkflowId { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        [Required]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the optional description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the sort order in the workflow.
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// Gets or sets if content in this stage is published.
        /// </summary>
        public bool IsPublished { get; set; }
    }
}
