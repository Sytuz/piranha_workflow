using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Piranha.Models;

namespace Piranha.Repositories
{
    public interface IWorkflowRepository
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
        /// Checks if a workflow title is unique.
        /// </summary>
        /// <param name="title">The title to check</param>
        /// <param name="id">Optional workflow id to exclude</param>
        Task<bool> IsUniqueTitleAsync(string title, Guid? id = null);
    }
}
