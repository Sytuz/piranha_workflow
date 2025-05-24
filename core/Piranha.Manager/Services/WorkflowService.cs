using Piranha.Models;

namespace Piranha.Manager.Services;

public class WorkflowService
{
    private readonly IApi _api;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="api">The current api</param>
    public WorkflowService(IApi api)
    {
        _api = api;
    }

    /// <summary>
    /// Gets all available workflows.
    /// </summary>
    /// <returns>The available workflows</returns>
    public async Task<IEnumerable<Workflow>> GetAllAsync()
    {
        return await _api.Workflows.GetAllAsync();
    }

    /// <summary>
    /// Gets the workflow with the specified id.
    /// </summary>
    /// <param name="id">The unique id</param>
    /// <returns>The workflow</returns>
    public async Task<Workflow> GetByIdAsync(Guid id)
    {
        return await _api.Workflows.GetByIdAsync(id);
    }

    /// <summary>
    /// Saves the given workflow.
    /// </summary>
    /// <param name="model">The workflow</param>
    public async Task SaveAsync(Workflow model)
    {
        await _api.Workflows.SaveAsync(model);
    }

    /// <summary>
    /// Creates a new standard workflow with the specified title.
    /// </summary>
    /// <param name="title">The workflow title</param>
    /// <param name="description">Optional workflow description</param>
    /// <returns>The created workflow</returns>
    public async Task<Workflow> CreateStandardWorkflowAsync(string title, string description = null)
    {
        return await _api.Workflows.CreateStandardWorkflowAsync(title, description);
    }

    /// <summary>
    /// Deletes the workflow with the specified id.
    /// </summary>
    /// <param name="id">The unique id</param>
    public async Task DeleteAsync(Guid id)
    {
        await _api.Workflows.DeleteAsync(id);
    }

    /// <summary>
    /// Toggles the workflow enabled state.
    /// </summary>
    /// <param name="id">The unique id</param>
    public async Task ToggleEnabledAsync(Guid id)
    {
        await _api.Workflows.ToggleEnabledAsync(id);
    }

    /// <summary>
    /// Checks if the given title is unique.
    /// </summary>
    /// <param name="title">The title</param>
    /// <param name="id">Optional id to exclude</param>
    /// <returns>If the title is unique</returns>
    public async Task<bool> IsUniqueTitleAsync(string title, Guid? id = null)
    {
        return await _api.Workflows.IsUniqueTitleAsync(title, id);
    }
}
