using System;

namespace Piranha.Models;

[Serializable]
public class WorkflowStageRelation
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
    public Guid SourceStageId { get; set; }

    /// <summary>
    /// Gets or sets the target stage id.
    /// </summary>
    public Guid TargetStageId { get; set; }
}
