using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Piranha.Data
{
    [Serializable]
    public class Workflow
    {
        /// <summary>
        /// Gets or sets the unique id.
        /// </summary>
        public Guid Id { get; set; }

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
        /// Gets or sets if this is the default workflow.
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// Gets or sets the created date.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the last modified date.
        /// </summary>
        public DateTime LastModified { get; set; }

        /// <summary>
        /// Gets/sets the available workflow stages.
        /// </summary>
        public IList<WorkflowStage> Stages { get; set; } = new List<WorkflowStage>();
    }
}
