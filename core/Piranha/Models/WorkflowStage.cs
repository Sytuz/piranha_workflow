using System.ComponentModel.DataAnnotations;

namespace Piranha.Models;

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
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets the optional description.
    /// </summary>
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
    /// Gets or sets the color used to represent this stage in the UI.
    /// </summary>
    public string Color { get; set; }

    /// <summary>
    /// Gets or sets the roles that can perform transitions from this stage.
    /// </summary>
    public IList<WorkflowStageRole> Roles { get; set; } = new List<WorkflowStageRole>();

    /// <summary>
    /// Gets or sets if this stage is immutable (cannot be edited or deleted).
    /// </summary>
    public bool IsImmutable { get; set; }

    /// <summary>
    /// Prevent editing of immutable stages.
    /// </summary>
    public void UpdateFrom(WorkflowStage source)
    {
        if (IsImmutable)
        {
            // Only allow updating non-editable fields (e.g. SortOrder, but not Title, Description, Color, Roles)
            this.SortOrder = source.SortOrder;
            // Optionally: allow IsPublished to be updated if needed
            this.IsPublished = source.IsPublished;
        }
        else
        {
            this.Title = source.Title;
            this.Description = source.Description;
            this.SortOrder = source.SortOrder;
            this.Color = source.Color;
            this.IsPublished = source.IsPublished;
            this.Roles = source.Roles;
        }
    }
}
