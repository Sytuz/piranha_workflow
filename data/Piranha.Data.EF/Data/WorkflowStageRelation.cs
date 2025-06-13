using System;
using System.ComponentModel.DataAnnotations;

namespace Piranha.Data
{
    [Serializable]
    public sealed class WorkflowStageRelation
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
        /// Gets or sets the source stage id.
        /// </summary>
        public Guid? SourceStageId { get; set; }

        /// <summary>
        /// Gets or sets the target stage id.
        /// </summary>
        public Guid? TargetStageId { get; set; }

        /// <summary>
        /// Gets or sets the workflow this relation belongs to.
        /// </summary>
        public Workflow Workflow { get; set; }

        /// <summary>
        /// Gets or sets the source stage.
        /// </summary>
        public WorkflowStage SourceStage { get; set; }

        /// <summary>
        /// Gets or sets the target stage.
        /// </summary>
        public WorkflowStage TargetStage { get; set; }
    }
}
