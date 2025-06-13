using System;

namespace Piranha.Models;

/// <summary>
/// Model representing the relationship between a workflow stage and roles that can perform transitions.
/// </summary>
[Serializable]
public class WorkflowStageRole
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
    public string RoleId { get; set; }

    /// <summary>
    /// Gets or sets the role name for display purposes.
    /// </summary>
    public string RoleName { get; set; }
}
