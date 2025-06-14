using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Piranha.Models;
using Piranha.Repositories;

namespace Piranha.Services
{
    public interface IWorkflowService
    {
        /// <summary>
        /// Gets all available workflows.
        /// </summary>
        /// <returns>The available workflows</returns>
        Task<IEnumerable<Workflow>> GetAllAsync();

        /// <summary>
        /// Gets the workflow with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The workflow</returns>
        Task<Workflow> GetByIdAsync(Guid id);

        /// <summary>
        /// Saves the given workflow.
        /// </summary>
        /// <param name="model">The workflow</param>
        Task SaveAsync(Workflow model);

        /// <summary>
        /// Deletes the workflow with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        Task DeleteAsync(Guid id);

        /// <summary>
        /// Toggles the workflow enabled state.
        /// </summary>
        /// <param name="id">The unique id</param>
        Task ToggleEnabledAsync(Guid id);

        /// <summary>
        /// Creates a new standard workflow with the specified title.
        /// </summary>
        /// <param name="title">The workflow title</param>
        /// <param name="description">Optional workflow description</param>
        /// <returns>The created workflow</returns>
        Task<Workflow> CreateStandardWorkflowAsync(string title, string description = null);

        /// <summary>
        /// Checks if the given title is unique.
        /// </summary>
        /// <param name="title">The title</param>
        /// <param name="id">Optional id to exclude</param>
        /// <returns>If the title is unique</returns>
        Task<bool> IsUniqueTitleAsync(string title, Guid? id = null);

        /// <summary>
        /// Gets the enabled workflow with stages and relations.
        /// </summary>
        /// <returns>The enabled workflow</returns>
        Task<Workflow> GetEnabledWorkflowAsync();

        /// <summary>
        /// Initializes Draft stages for existing workflows with all available roles.
        /// This should be run once at project startup.
        /// </summary>
        Task InitializeDefaultWorkflowRolesAsync();
    }
}
