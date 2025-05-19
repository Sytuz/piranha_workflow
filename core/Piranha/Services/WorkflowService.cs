using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Piranha.Models;
using Piranha.Repositories;

namespace Piranha.Services
{
    public class WorkflowService : IWorkflowService
    {
        private readonly IWorkflowRepository _repo;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="repo">The workflow repository</param>
        public WorkflowService(IWorkflowRepository repo)
        {
            _repo = repo;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Models.Workflow>> GetAllAsync()
        {
            return await _repo.GetAllAsync().ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<Models.Workflow> GetByIdAsync(Guid id)
        {
            return await _repo.GetByIdAsync(id).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task SaveAsync(Models.Workflow model)
        {
            // Validate name
            if (string.IsNullOrWhiteSpace(model.Title))
                throw new ArgumentException("Name is required");

            // Check for title uniqueness
            if (!await _repo.IsUniqueTitleAsync(model.Title, model.Id).ConfigureAwait(false))
                throw new ArgumentException("Name already in use");

            await _repo.SaveAsync(model).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task DeleteAsync(Guid id)
        {
            await _repo.DeleteAsync(id).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<Models.Workflow> CreateStandardWorkflowAsync(string title, string description = null)
        {
            var workflow = new Models.Workflow
            {
                Title = title,
                Description = description,
                Stages = new List<WorkflowStage>
                {
                    new WorkflowStage
                    {
                        Title = "Draft",
                        Description = "Initial content creation",
                        SortOrder = 1,
                        IsPublished = false
                    },
                    new WorkflowStage
                    {
                        Title = "Review",
                        Description = "Content under review",
                        SortOrder = 2,
                        IsPublished = false
                    },
                    new WorkflowStage
                    {
                        Title = "Approved",
                        Description = "Content ready for publishing",
                        SortOrder = 3,
                        IsPublished = true
                    }
                }
            };

            await SaveAsync(workflow).ConfigureAwait(false);
            return workflow;
        }

        /// <inheritdoc />
        public async Task<bool> IsUniqueTitleAsync(string title, Guid? id = null)
        {
            return await _repo.IsUniqueTitleAsync(title, id).ConfigureAwait(false);
        }
    }
}
