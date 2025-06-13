using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Piranha.Data;

/// <summary>
/// Data entity representing the relationship between a workflow stage and roles.
/// </summary>
[Serializable]
public sealed class WorkflowStageRole
{
    /// <summary>
    /// Gets or sets the unique id.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the workflow stage id.
    /// </summary>
    public Guid WorkflowStageId { get; set; }

    /// <summary>
    /// Gets or sets the role id.
    /// </summary>
    [StringLength(450)]
    [Required]
    public string RoleId { get; set; }

    /// <summary>
    /// Gets or sets the created date.
    /// </summary>
    public DateTime Created { get; set; }

    /// <summary>
    /// Gets or sets the last modified date.
    /// </summary>
    public DateTime LastModified { get; set; }

    /// <summary>
    /// Gets or sets the workflow stage this role assignment belongs to.
    /// </summary>
    public WorkflowStage WorkflowStage { get; set; }
}
