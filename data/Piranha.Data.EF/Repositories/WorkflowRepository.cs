using Microsoft.EntityFrameworkCore;
using Piranha.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        /// <summary>
        /// Gets all available workflows.
        /// </summary>
        /// <returns>The available workflows</returns>
        public async Task<IEnumerable<Models.Workflow>> GetAllAsync()
        {
            var workflows = await _db.Workflows
                .AsNoTracking()
                .OrderBy(w => w.Title)
                .ToListAsync();

            var models = new List<Models.Workflow>();

            foreach (var workflow in workflows)
            {
                var model = new Models.Workflow
                {
                    Id = workflow.Id,
                    Title = workflow.Title,
                    Description = workflow.Description,
                    IsDefault = workflow.IsDefault,
                    IsEnabled = workflow.IsEnabled,
                    Created = workflow.Created,
                    LastModified = workflow.LastModified
                };

                // Get stages
                model.Stages = await _db.WorkflowStages
                    .Include(s => s.Roles)
                    .Where(s => s.WorkflowId == workflow.Id)
                    .OrderBy(s => s.SortOrder)
                    .Select(s => new Models.WorkflowStage
                    {
                        Id = s.Id,
                        WorkflowId = s.WorkflowId,
                        Title = s.Title,
                        Description = s.Description,
                        SortOrder = s.SortOrder,
                        IsPublished = s.IsPublished,
                        Color = s.Color,
                        IsImmutable = s.IsImmutable, // Add this line
                        Roles = s.Roles.Select(r => new Models.WorkflowStageRole
                        {
                            Id = r.Id,
                            WorkflowStageId = r.WorkflowStageId,
                            RoleId = r.RoleId
                        }).ToList()
                    })
                    .ToListAsync();

                // Get relations
                model.Relations = await _db.WorkflowStageRelations
                    .Where(r => r.WorkflowId == workflow.Id)
                    .Select(r => new Models.WorkflowStageRelation
                    {
                        Id = r.Id,
                        WorkflowId = r.WorkflowId,
                        SourceStageId = r.SourceStageId.Value,
                        TargetStageId = r.TargetStageId.Value
                    })
                    .ToListAsync();

                models.Add(model);
            }
            return models;
        }

        /// <summary>
        /// Gets the workflow with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The workflow</returns>
        public async Task<Models.Workflow> GetByIdAsync(Guid id)
        {
            var workflow = await _db.Workflows
                .AsNoTracking()
                .FirstOrDefaultAsync(w => w.Id == id);

            if (workflow != null)
            {
                var model = new Models.Workflow
                {
                    Id = workflow.Id,
                    Title = workflow.Title,
                    Description = workflow.Description,
                    IsDefault = workflow.IsDefault,
                    IsEnabled = workflow.IsEnabled,
                    Created = workflow.Created,
                    LastModified = workflow.LastModified
                };

                // Get stages
                model.Stages = await _db.WorkflowStages
                    .Include(s => s.Roles)
                    .Where(s => s.WorkflowId == id)
                    .OrderBy(s => s.SortOrder)
                    .Select(s => new Models.WorkflowStage
                    {
                        Id = s.Id,
                        WorkflowId = s.WorkflowId,
                        Title = s.Title,
                        Description = s.Description,
                        SortOrder = s.SortOrder,
                        IsPublished = s.IsPublished,
                        Color = s.Color,
                        IsImmutable = s.IsImmutable, // Add this line
                        Roles = s.Roles.Select(r => new Models.WorkflowStageRole
                        {
                            Id = r.Id,
                            WorkflowStageId = r.WorkflowStageId,
                            RoleId = r.RoleId
                        }).ToList()
                    })
                    .ToListAsync();

                // Get relations
                model.Relations = await _db.WorkflowStageRelations
                    .Where(r => r.WorkflowId == id)
                    .Select(r => new Models.WorkflowStageRelation
                    {
                        Id = r.Id,
                        WorkflowId = r.WorkflowId,
                        SourceStageId = r.SourceStageId.Value,
                        TargetStageId = r.TargetStageId.Value
                    })
                    .ToListAsync();

                return model;
            }
            return null;
        }

        /// <summary>
        /// Saves the given workflow.
        /// </summary>
        /// <param name="model">The workflow</param>
        public async Task SaveAsync(Models.Workflow model)
        {
            var workflow = await _db.Workflows
                .FirstOrDefaultAsync(w => w.Id == model.Id);

            if (workflow == null)
            {
                workflow = new Data.Workflow
                {
                    Id = model.Id != Guid.Empty ? model.Id : Guid.NewGuid(),
                    Created = DateTime.Now
                };
                await _db.Workflows.AddAsync(workflow);
            }

            workflow.Title = model.Title;
            workflow.Description = model.Description;
            workflow.IsDefault = model.IsDefault;
            workflow.IsEnabled = model.IsEnabled;
            workflow.LastModified = DateTime.Now;

            // Save stages
            foreach (var stage in model.Stages)
            {
                stage.WorkflowId = workflow.Id;
                var dbStage = await _db.WorkflowStages
                    .FirstOrDefaultAsync(s => s.Id == stage.Id);

                if (dbStage == null)
                {
                    dbStage = new Data.WorkflowStage
                    {
                        Id = stage.Id != Guid.Empty ? stage.Id : Guid.NewGuid(),
                        WorkflowId = workflow.Id
                        // IsImmutable will be set below
                    };
                    await _db.WorkflowStages.AddAsync(dbStage);
                }

                dbStage.Title = stage.Title;
                dbStage.Description = stage.Description;
                dbStage.SortOrder = stage.SortOrder;
                dbStage.IsPublished = stage.IsPublished;
                dbStage.Color = stage.Color;
                dbStage.IsImmutable = stage.IsImmutable; // Add this line

                // Handle roles - remove existing and add new ones if provided
                if (stage.Roles != null)
                {
                    // Remove existing roles using direct query to avoid tracking issues
                    var existingRoles = await _db.WorkflowStageRoles
                        .Where(r => r.WorkflowStageId == dbStage.Id)
                        .ToListAsync();
                    
                    if (existingRoles.Any())
                    {
                        _db.WorkflowStageRoles.RemoveRange(existingRoles);
                    }

                    // Add new roles
                    foreach (var role in stage.Roles)
                    {
                        var dbRole = new Data.WorkflowStageRole
                        {
                            Id = role.Id != Guid.Empty ? role.Id : Guid.NewGuid(),
                            WorkflowStageId = dbStage.Id,
                            RoleId = role.RoleId,
                            Created = DateTime.Now,
                            LastModified = DateTime.Now
                        };
                        await _db.WorkflowStageRoles.AddAsync(dbRole);
                    }
                }
            }

            // Delete removed stages
            var oldStages = await _db.WorkflowStages
                .Where(s => s.WorkflowId == workflow.Id)
                .ToListAsync();

            foreach (var oldStage in oldStages)
            {
                if (!model.Stages.Any(s => s.Id == oldStage.Id))
                {
                    _db.WorkflowStages.Remove(oldStage);
                }
            }

            // Save relations
            foreach (var relation in model.Relations)
            {
                relation.WorkflowId = workflow.Id;
                var dbRelation = await _db.WorkflowStageRelations
                    .FirstOrDefaultAsync(r => r.Id == relation.Id);

                if (dbRelation == null)
                {
                    dbRelation = new Data.WorkflowStageRelation
                    {
                        Id = relation.Id != Guid.Empty ? relation.Id : Guid.NewGuid(),
                        WorkflowId = workflow.Id
                    };
                    await _db.WorkflowStageRelations.AddAsync(dbRelation);
                }

                dbRelation.SourceStageId = relation.SourceStageId;
                dbRelation.TargetStageId = relation.TargetStageId;
            }

            // Delete removed relations
            var oldRelations = await _db.WorkflowStageRelations
                .Where(r => r.WorkflowId == workflow.Id)
                .ToListAsync();

            foreach (var oldRelation in oldRelations)
            {
                if (!model.Relations.Any(r => r.Id == oldRelation.Id))
                {
                    _db.WorkflowStageRelations.Remove(oldRelation);
                }
            }

            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes the workflow with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        public async Task DeleteAsync(Guid id)
        {
            // Delete workflow stages
            var stages = await _db.WorkflowStages
                .Where(s => s.WorkflowId == id)
                .ToListAsync();
            
            if (stages.Count > 0)
            {
                _db.WorkflowStages.RemoveRange(stages);
            }

            // Delete workflow stage relations
            var relations = await _db.WorkflowStageRelations
                .Where(r => r.WorkflowId == id)
                .ToListAsync();
            
            if (relations.Count > 0)
            {
                _db.WorkflowStageRelations.RemoveRange(relations);
            }

            // Delete workflow
            var workflow = await _db.Workflows
                .FirstOrDefaultAsync(w => w.Id == id);
            
            if (workflow != null)
            {
                _db.Workflows.Remove(workflow);
            }

            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Checks if the given title is unique for the workflow.
        /// </summary>
        /// <param name="title">The title to check</param>
        /// <param name="id">The optional workflow id</param>
        /// <returns>If the title is unique</returns>
        public async Task<bool> IsUniqueTitleAsync(string title, Guid? id = null)
        {
            return await _db.Workflows
                .Where(w => w.Title == title)
                .Where(w => id == null || w.Id != id.Value)
                .CountAsync() == 0;
        }
    }
}
