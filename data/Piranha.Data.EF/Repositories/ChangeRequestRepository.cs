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
    public class ChangeRequestRepository : IChangeRequestRepository
    {
        private readonly IDb _db;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="db">The current db context</param>
        public ChangeRequestRepository(IDb db)
        {
            _db = db;
        }

        /// <summary>
        /// Gets all change requests.
        /// </summary>
        /// <returns>The change requests</returns>
        public async Task<IEnumerable<Models.ChangeRequest>> GetAllAsync()
        {
            return await _db.ChangeRequests
                .AsNoTracking()
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new Models.ChangeRequest
                {
                    Id = c.Id,
                    Title = c.Title,
                    ContentSnapshot = c.ContentSnapshot,
                    WorkflowId = c.WorkflowId,
                    StageId = c.StageId,
                    CreatedById = c.CreatedById,
                    CreatedAt = c.CreatedAt,
                    LastModified = c.LastModified,
                    ContentId = c.ContentId,
                    Status = (ChangeRequestStatus)c.Status,
                    Notes = c.Notes
                })
                .ToListAsync();
        }

        /// <summary>
        /// Gets the change request with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The change request</returns>
        public async Task<Models.ChangeRequest> GetByIdAsync(Guid id)
        {
            var changeRequest = await _db.ChangeRequests
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (changeRequest != null)
            {
                return new Models.ChangeRequest
                {
                    Id = changeRequest.Id,
                    Title = changeRequest.Title,
                    ContentSnapshot = changeRequest.ContentSnapshot,
                    WorkflowId = changeRequest.WorkflowId,
                    StageId = changeRequest.StageId,
                    CreatedById = changeRequest.CreatedById,
                    CreatedAt = changeRequest.CreatedAt,
                    LastModified = changeRequest.LastModified,
                    ContentId = changeRequest.ContentId,
                    Status = (ChangeRequestStatus)changeRequest.Status,
                    Notes = changeRequest.Notes
                };
            }
            return null;
        }

        /// <summary>
        /// Gets all change requests for the specified workflow.
        /// </summary>
        /// <param name="workflowId">The workflow id</param>
        /// <returns>The change requests</returns>
        public async Task<IEnumerable<Models.ChangeRequest>> GetByWorkflowIdAsync(Guid workflowId)
        {
            return await _db.ChangeRequests
                .AsNoTracking()
                .Where(c => c.WorkflowId == workflowId)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new Models.ChangeRequest
                {
                    Id = c.Id,
                    Title = c.Title,
                    ContentSnapshot = c.ContentSnapshot,
                    WorkflowId = c.WorkflowId,
                    StageId = c.StageId,
                    CreatedById = c.CreatedById,
                    CreatedAt = c.CreatedAt,
                    LastModified = c.LastModified,
                    ContentId = c.ContentId,
                    Status = (ChangeRequestStatus)c.Status,
                    Notes = c.Notes
                })
                .ToListAsync();
        }

        /// <summary>
        /// Gets all change requests created by the specified user.
        /// </summary>
        /// <param name="userId">The user id</param>
        /// <returns>The change requests</returns>
        public async Task<IEnumerable<Models.ChangeRequest>> GetByCreatedByIdAsync(Guid userId)
        {
            return await _db.ChangeRequests
                .AsNoTracking()
                .Where(c => c.CreatedById == userId)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new Models.ChangeRequest
                {
                    Id = c.Id,
                    Title = c.Title,
                    ContentSnapshot = c.ContentSnapshot,
                    WorkflowId = c.WorkflowId,
                    StageId = c.StageId,
                    CreatedById = c.CreatedById,
                    CreatedAt = c.CreatedAt,
                    LastModified = c.LastModified,
                    ContentId = c.ContentId,
                    Status = (ChangeRequestStatus)c.Status,
                    Notes = c.Notes
                })
                .ToListAsync();
        }

        /// <summary>
        /// Gets all change requests in the specified stage.
        /// </summary>
        /// <param name="stageId">The stage id</param>
        /// <returns>The change requests</returns>
        public async Task<IEnumerable<Models.ChangeRequest>> GetByStageIdAsync(Guid stageId)
        {
            return await _db.ChangeRequests
                .AsNoTracking()
                .Where(c => c.StageId == stageId)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new Models.ChangeRequest
                {
                    Id = c.Id,
                    Title = c.Title,
                    ContentSnapshot = c.ContentSnapshot,
                    WorkflowId = c.WorkflowId,
                    StageId = c.StageId,
                    CreatedById = c.CreatedById,
                    CreatedAt = c.CreatedAt,
                    LastModified = c.LastModified,
                    ContentId = c.ContentId,
                    Status = (ChangeRequestStatus)c.Status,
                    Notes = c.Notes
                })
                .ToListAsync();
        }

        /// <summary>
        /// Gets all change requests with the specified status.
        /// </summary>
        /// <param name="status">The status</param>
        /// <returns>The change requests</returns>
        public async Task<IEnumerable<Models.ChangeRequest>> GetByStatusAsync(ChangeRequestStatus status)
        {
            return await _db.ChangeRequests
                .AsNoTracking()
                .Where(c => c.Status == (int)status)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new Models.ChangeRequest
                {
                    Id = c.Id,
                    Title = c.Title,
                    ContentSnapshot = c.ContentSnapshot,
                    WorkflowId = c.WorkflowId,
                    StageId = c.StageId,
                    CreatedById = c.CreatedById,
                    CreatedAt = c.CreatedAt,
                    LastModified = c.LastModified,
                    ContentId = c.ContentId,
                    Status = (ChangeRequestStatus)c.Status,
                    Notes = c.Notes
                })
                .ToListAsync();
        }

        /// <summary>
        /// Gets change requests for the specified content.
        /// </summary>
        /// <param name="contentId">The content id</param>
        /// <returns>The change requests</returns>
        public async Task<IEnumerable<Models.ChangeRequest>> GetByContentIdAsync(Guid contentId)
        {
            return await _db.ChangeRequests
                .AsNoTracking()
                .Where(c => c.ContentId == contentId)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new Models.ChangeRequest
                {
                    Id = c.Id,
                    Title = c.Title,
                    ContentSnapshot = c.ContentSnapshot,
                    WorkflowId = c.WorkflowId,
                    StageId = c.StageId,
                    CreatedById = c.CreatedById,
                    CreatedAt = c.CreatedAt,
                    LastModified = c.LastModified,
                    ContentId = c.ContentId,
                    Status = (ChangeRequestStatus)c.Status,
                    Notes = c.Notes
                })
                .ToListAsync();
        }

        /// <summary>
        /// Adds or updates the given change request.
        /// </summary>
        /// <param name="changeRequest">The change request</param>
        public async Task SaveAsync(Models.ChangeRequest changeRequest)
        {
            var entity = await _db.ChangeRequests
                .FirstOrDefaultAsync(c => c.Id == changeRequest.Id);

            if (entity == null)
            {
                entity = new Data.ChangeRequest
                {
                    Id = changeRequest.Id != Guid.Empty ? changeRequest.Id : Guid.NewGuid(),
                    CreatedAt = DateTime.Now
                };
                await _db.ChangeRequests.AddAsync(entity);
            }

            entity.Title = changeRequest.Title;
            entity.ContentSnapshot = changeRequest.ContentSnapshot;
            entity.WorkflowId = changeRequest.WorkflowId;
            entity.StageId = changeRequest.StageId;
            entity.CreatedById = changeRequest.CreatedById;
            entity.LastModified = DateTime.Now;
            entity.ContentId = changeRequest.ContentId;
            entity.Status = (int)changeRequest.Status;
            entity.Notes = changeRequest.Notes;

            await _db.SaveChangesAsync();

            // Update the model with the saved entity values
            changeRequest.Id = entity.Id;
            changeRequest.CreatedAt = entity.CreatedAt;
            changeRequest.LastModified = entity.LastModified;
        }

        /// <summary>
        /// Moves the change request to the specified stage.
        /// </summary>
        /// <param name="id">The change request id</param>
        /// <param name="stageId">The target stage id</param>
        /// <returns>The updated change request</returns>
        public async Task<Models.ChangeRequest> MoveToStageAsync(Guid id, Guid stageId)
        {
            var entity = await _db.ChangeRequests
                .FirstOrDefaultAsync(c => c.Id == id);

            if (entity != null)
            {
                entity.StageId = stageId;
                entity.LastModified = DateTime.Now;
                await _db.SaveChangesAsync();

                return new Models.ChangeRequest
                {
                    Id = entity.Id,
                    Title = entity.Title,
                    ContentSnapshot = entity.ContentSnapshot,
                    WorkflowId = entity.WorkflowId,
                    StageId = entity.StageId,
                    CreatedById = entity.CreatedById,
                    CreatedAt = entity.CreatedAt,
                    LastModified = entity.LastModified,
                    ContentId = entity.ContentId,
                    Status = (ChangeRequestStatus)entity.Status,
                    Notes = entity.Notes
                };
            }
            return null;
        }

        /// <summary>
        /// Updates the status of the change request.
        /// </summary>
        /// <param name="changeRequestId">The change request id</param>
        /// <param name="status">The new status</param>
        public async Task UpdateStatusAsync(Guid changeRequestId, ChangeRequestStatus status)
        {
            var entity = await _db.ChangeRequests
                .FirstOrDefaultAsync(c => c.Id == changeRequestId);

            if (entity != null)
            {
                entity.Status = (int)status;
                entity.LastModified = DateTime.Now;
                await _db.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Deletes the change request with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        public async Task DeleteAsync(Guid id)
        {
            var entity = await _db.ChangeRequests
                .FirstOrDefaultAsync(c => c.Id == id);

            if (entity != null)
            {
                _db.ChangeRequests.Remove(entity);
                await _db.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Deletes the given change request.
        /// </summary>
        /// <param name="changeRequest">The change request</param>
        public async Task DeleteAsync(Models.ChangeRequest changeRequest)
        {
            await DeleteAsync(changeRequest.Id);
        }
    }
}