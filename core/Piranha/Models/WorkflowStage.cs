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
}
