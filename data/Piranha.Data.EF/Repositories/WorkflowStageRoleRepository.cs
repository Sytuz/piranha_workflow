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
using Piranha.Data.EF;

namespace Piranha.Repositories
{
    public class WorkflowStageRoleRepository : IWorkflowStageRoleRepository
    {
        private readonly IDb _db;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="db">The current db context</param>
        public WorkflowStageRoleRepository(IDb db)
        {
            _db = db;
        }

        /// <summary>
        /// Gets all roles for a specific workflow stage.
        /// </summary>
        /// <param name="workflowStageId">The workflow stage id</param>
        /// <returns>The workflow stage roles</returns>
        public async Task<IEnumerable<Models.WorkflowStageRole>> GetByWorkflowStageIdAsync(Guid workflowStageId)
        {
            var roles = await _db.WorkflowStageRoles
                .AsNoTracking()
                .Where(r => r.WorkflowStageId == workflowStageId)
                .OrderBy(r => r.RoleId)
                .Select(r => new Models.WorkflowStageRole
                {
                    Id = r.Id,
                    WorkflowStageId = r.WorkflowStageId,
                    RoleId = r.RoleId
                })
                .ToListAsync()
                .ConfigureAwait(false);

            return roles;
        }

        /// <summary>
        /// Saves the given workflow stage role.
        /// </summary>
        /// <param name="model">The workflow stage role</param>
        public async Task SaveAsync(Models.WorkflowStageRole model)
        {
            var role = await _db.WorkflowStageRoles
                .FirstOrDefaultAsync(r => r.Id == model.Id)
                .ConfigureAwait(false);

            if (role == null)
            {
                role = new Data.WorkflowStageRole
                {
                    Id = model.Id != Guid.Empty ? model.Id : Guid.NewGuid(),
                    Created = DateTime.Now
                };
                await _db.WorkflowStageRoles.AddAsync(role).ConfigureAwait(false);
            }

            role.WorkflowStageId = model.WorkflowStageId;
            role.RoleId = model.RoleId;
            role.LastModified = DateTime.Now;

            await _db.SaveChangesAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes the workflow stage role with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        public async Task DeleteAsync(Guid id)
        {
            var role = await _db.WorkflowStageRoles
                .FirstOrDefaultAsync(r => r.Id == id)
                .ConfigureAwait(false);

            if (role != null)
            {
                _db.WorkflowStageRoles.Remove(role);
                await _db.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Deletes all workflow stage roles for the given workflow stage.
        /// </summary>
        /// <param name="workflowStageId">The workflow stage id</param>
        public async Task DeleteByWorkflowStageIdAsync(Guid workflowStageId)
        {
            var roles = await _db.WorkflowStageRoles
                .Where(r => r.WorkflowStageId == workflowStageId)
                .ToListAsync()
                .ConfigureAwait(false);

            if (roles.Any())
            {
                _db.WorkflowStageRoles.RemoveRange(roles);
                await _db.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Checks if the given role is assigned to the workflow stage.
        /// </summary>
        /// <param name="workflowStageId">The workflow stage id</param>
        /// <param name="roleId">The role id</param>
        /// <returns>True if the role is assigned</returns>
        public async Task<bool> IsRoleAssignedAsync(Guid workflowStageId, string roleId)
        {
            return await _db.WorkflowStageRoles
                .AsNoTracking()
                .AnyAsync(r => r.WorkflowStageId == workflowStageId && r.RoleId == roleId)
                .ConfigureAwait(false);
        }
    }
}
