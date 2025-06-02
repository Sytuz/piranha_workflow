/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Microsoft.EntityFrameworkCore;
using Piranha.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Piranha.Repositories
{
    public class WorkflowStageRepository : IWorkflowStageRepository
    {
        private readonly IDb _db;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="db">The current db context</param>
        public WorkflowStageRepository(IDb db)
        {
            _db = db;
        }

        /// <summary>
        /// Gets all workflow stages.
        /// </summary>
        /// <returns>The workflow stages</returns>
        public async Task<IEnumerable<Models.WorkflowStage>> GetAll()
        {
            return await _db.WorkflowStages
                .Include(s => s.Roles)
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
                    IsImmutable = s.IsImmutable,
                    Roles = s.Roles.Select(r => new Models.WorkflowStageRole
                    {
                        Id = r.Id,
                        WorkflowStageId = r.WorkflowStageId,
                        RoleId = r.RoleId
                    }).ToList()
                })
                .ToListAsync();
        }

        /// <summary>
        /// Gets the workflow stage with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The workflow stage</returns>
        public async Task<Models.WorkflowStage> GetById(Guid id)
        {
            var stage = await _db.WorkflowStages
                .Include(s => s.Roles)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (stage != null)
            {
                return new Models.WorkflowStage
                {
                    Id = stage.Id,
                    WorkflowId = stage.WorkflowId,
                    Title = stage.Title,
                    Description = stage.Description,
                    SortOrder = stage.SortOrder,
                    IsPublished = stage.IsPublished,
                    Color = stage.Color,
                    IsImmutable = stage.IsImmutable,
                    Roles = stage.Roles.Select(r => new Models.WorkflowStageRole
                    {
                        Id = r.Id,
                        WorkflowStageId = r.WorkflowStageId,
                        RoleId = r.RoleId
                    }).ToList()
                };
            }
            return null;
        }

        /// <summary>
        /// Gets all workflow stages for the specified workflow.
        /// </summary>
        /// <param name="workflowId">The workflow id</param>
        /// <returns>The workflow stages</returns>
        public async Task<IEnumerable<Models.WorkflowStage>> GetByWorkflowId(Guid workflowId)
        {
            return await _db.WorkflowStages
                .Include(s => s.Roles)
                .Where(s => s.WorkflowId == workflowId)
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
                    IsImmutable = s.IsImmutable,
                    Roles = s.Roles.Select(r => new Models.WorkflowStageRole
                    {
                        Id = r.Id,
                        WorkflowStageId = r.WorkflowStageId,
                        RoleId = r.RoleId
                    }).ToList()
                })
                .ToListAsync();
        }

        /// <summary>
        /// Adds or updates the given workflow stage.
        /// </summary>
        /// <param name="stage">The workflow stage</param>
        public async Task Save(Models.WorkflowStage stage)
        {
            var dbStage = await _db.WorkflowStages
                .Include(s => s.Roles)
                .FirstOrDefaultAsync(s => s.Id == stage.Id);

            if (dbStage == null)
            {
                dbStage = new Data.WorkflowStage
                {
                    Id = stage.Id != Guid.Empty ? stage.Id : Guid.NewGuid()
                };
                await _db.WorkflowStages.AddAsync(dbStage);
            }

            dbStage.WorkflowId = stage.WorkflowId;
            dbStage.Title = stage.Title;
            dbStage.Description = stage.Description;
            dbStage.SortOrder = stage.SortOrder;
            dbStage.IsPublished = stage.IsPublished;
            dbStage.Color = stage.Color;
            dbStage.IsImmutable = stage.IsImmutable;

            // Handle roles - remove existing and add new ones if provided
            if (stage.Roles != null)
            {
                // Remove existing roles
                var existingRoles = dbStage.Roles.ToList();
                foreach (var existingRole in existingRoles)
                {
                    _db.WorkflowStageRoles.Remove(existingRole);
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
                    dbStage.Roles.Add(dbRole);
                }
            }

            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes the workflow stage with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        public async Task Delete(Guid id)
        {
            var stage = await _db.WorkflowStages
                .FirstOrDefaultAsync(s => s.Id == id);

            if (stage != null)
            {
                _db.WorkflowStages.Remove(stage);
                await _db.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Deletes the given workflow stage.
        /// </summary>
        /// <param name="stage">The workflow stage</param>
        public async Task Delete(Models.WorkflowStage stage)
        {
            await Delete(stage.Id);
        }

        /// <summary>
        /// Deletes all stages for the specified workflow.
        /// </summary>
        /// <param name="workflowId">The workflow id</param>
        public async Task DeleteByWorkflow(Guid workflowId)
        {
            var stages = await _db.WorkflowStages
                .Where(s => s.WorkflowId == workflowId)
                .ToListAsync();

            if (stages.Count > 0)
            {
                _db.WorkflowStages.RemoveRange(stages);
                await _db.SaveChangesAsync();
            }
        }
    }
}
