using System;
using System.ComponentModel.DataAnnotations;

namespace Piranha.Data
{
    [Serializable]
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
        [StringLength(128)]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the optional description.
        /// </summary>
        [StringLength(512)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the sort order of this stage.
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// Gets or sets if content is published in this stage.
        /// </summary>
        public bool IsPublished { get; set; }

        /// <summary>
        /// Gets or sets the workflow.
        /// </summary>
        public Workflow Workflow { get; set; }
    }
}
