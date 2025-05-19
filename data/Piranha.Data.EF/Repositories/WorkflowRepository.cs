using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Piranha.Models;
using Piranha.Repositories;

namespace Piranha.Repositories
{
    public class WorkflowRepository : IWorkflowRepository
    {
        private readonly IDb _db;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="db">The current db context</param>
        public WorkflowRepository(IDb db)
        {
            _db = db;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Models.Workflow>> GetAllAsync()
        {
            var result = new List<Models.Workflow>();
            var workflows = await _db.Workflows
                .Include(w => w.Stages)
                .OrderBy(w => w.Title)
                .ToListAsync()
                .ConfigureAwait(false);

            foreach (var workflow in workflows)
            {
                result.Add(Map(workflow));
            }
            return result;
        }

        /// <inheritdoc />
        public async Task<Models.Workflow> GetByIdAsync(Guid id)
        {
            var workflow = await _db.Workflows
                .Include(w => w.Stages)
                .FirstOrDefaultAsync(w => w.Id == id)
                .ConfigureAwait(false);

            if (workflow != null)
                return Map(workflow);
            return null;
        }

        /// <inheritdoc />
        public async Task SaveAsync(Models.Workflow model)
        {
            var isNew = model.Id == Guid.Empty;

            // Create or update the workflow
            Data.Workflow workflow;
            if (isNew)
            {
                workflow = new Data.Workflow
                {
                    Id = Guid.NewGuid(),
                    Created = DateTime.Now
                };
                model.Id = workflow.Id;
                await _db.Workflows.AddAsync(workflow).ConfigureAwait(false);
            }
            else
            {
                workflow = await _db.Workflows
                    .Include(w => w.Stages)
                    .FirstOrDefaultAsync(w => w.Id == model.Id)
                    .ConfigureAwait(false);

                if (workflow == null)
                    throw new ArgumentException($"Workflow with ID {model.Id} not found");

                // Remove deleted stages
                var removedStages = workflow.Stages
                    .Where(s => !model.Stages.Any(ms => ms.Id == s.Id))
                    .ToList();

                foreach (var stage in removedStages)
                {
                    _db.WorkflowStages.Remove(stage);
                }
            }

            workflow.Title = model.Title;
            workflow.Description = model.Description;
            workflow.IsDefault = model.IsDefault;
            workflow.LastModified = DateTime.Now;

            // Handle stages
            foreach (var stage in model.Stages)
            {
                var stageId = stage.Id == Guid.Empty ? Guid.NewGuid() : stage.Id;

                var dbStage = workflow.Stages.FirstOrDefault(s => s.Id == stage.Id);

                if (dbStage == null)
                {
                    dbStage = new Data.WorkflowStage
                    {
                        Id = stageId,
                        WorkflowId = workflow.Id
                    };
                    workflow.Stages.Add(dbStage);
                }

                dbStage.Title = stage.Title;
                dbStage.Description = stage.Description;
                dbStage.SortOrder = stage.SortOrder;
                dbStage.IsPublished = stage.IsPublished;
            }

            await _db.SaveChangesAsync().ConfigureAwait(false);

            // If this is the default workflow, unset any other defaults
            if (workflow.IsDefault)
            {
                var otherDefaults = await _db.Workflows
                    .Where(w => w.IsDefault && w.Id != workflow.Id)
                    .ToListAsync()
                    .ConfigureAwait(false);

                if (otherDefaults.Any())
                {
                    foreach (var other in otherDefaults)
                    {
                        other.IsDefault = false;
                    }
                    await _db.SaveChangesAsync().ConfigureAwait(false);
                }
            }
        }

        /// <inheritdoc />
        public async Task DeleteAsync(Guid id)
        {
            var workflow = await _db.Workflows
                .FirstOrDefaultAsync(w => w.Id == id)
                .ConfigureAwait(false);

            if (workflow != null)
            {
                _db.Workflows.Remove(workflow);
                await _db.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        /// <inheritdoc />
        public async Task<bool> IsUniqueTitleAsync(string title, Guid? id = null)
        {
            return !await _db.Workflows.AnyAsync(w =>
                w.Title.ToLower() == title.ToLower() &&
                (!id.HasValue || w.Id != id.Value))
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Maps a data entity to a model.
        /// </summary>
        /// <param name="workflow">The data entity</param>
        /// <returns>The model</returns>
        private Models.Workflow Map(Data.Workflow workflow)
        {
            var model = new Models.Workflow
            {
                Id = workflow.Id,
                Title = workflow.Title,
                Description = workflow.Description,
                IsDefault = workflow.IsDefault,
                Created = workflow.Created,
                LastModified = workflow.LastModified
            };

            foreach (var stage in workflow.Stages.OrderBy(s => s.SortOrder))
            {
                model.Stages.Add(new Models.WorkflowStage
                {
                    Id = stage.Id,
                    WorkflowId = stage.WorkflowId,
                    Title = stage.Title,
                    Description = stage.Description,
                    SortOrder = stage.SortOrder,
                    IsPublished = stage.IsPublished
                });
            }

            return model;
        }
    }
}
