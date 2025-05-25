/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

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
        /// <param name="workflow">The workflow</param>
        Task SaveAsync(Workflow workflow);

        /// <summary>
        /// Deletes the workflow with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        Task DeleteAsync(Guid id);

        /// <summary>
        /// Checks if the given title is unique for the workflow.
        /// </summary>
        /// <param name="title">The title to check</param>
        /// <param name="id">The optional workflow id</param>
        /// <returns>If the title is unique</returns>
        Task<bool> IsUniqueTitleAsync(string title, Guid? id = null);
    }
}
