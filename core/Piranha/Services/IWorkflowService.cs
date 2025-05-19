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
        Task<IEnumerable<Workflow>> GetAllAsync();

        /// <summary>
        /// Gets the workflow with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
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
        /// Creates a new workflow with standard stages.
        /// </summary>
        /// <param name="title">The workflow title</param>
        /// <param name="description">The optional description</param>
        Task<Workflow> CreateStandardWorkflowAsync(string title, string description = null);

        /// <summary>
        /// Checks if a workflow title is unique.
        /// </summary>
        /// <param name="title">The title to check</param>
        /// <param name="id">Optional workflow id to exclude</param>
        Task<bool> IsUniqueTitleAsync(string title, Guid? id = null);
    }
}
