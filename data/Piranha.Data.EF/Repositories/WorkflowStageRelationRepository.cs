using Microsoft.EntityFrameworkCore;
using Piranha.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Piranha.Repositories
{
    public class WorkflowStageRelationRepository : IWorkflowStageRelationRepository
    {
        private readonly IDb _db;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="db">The current db context</param>
        public WorkflowStageRelationRepository(IDb db)
        {
            _db = db;
        }

        /// <summary>
        /// Gets all workflow stage relations.
        /// </summary>
        /// <returns>The workflow stage relations</returns>
        public async Task<IEnumerable<Models.WorkflowStageRelation>> GetAll()
        {
            return await _db.WorkflowStageRelations
                .Select(r => new Models.WorkflowStageRelation
                {
                    Id = r.Id,
                    WorkflowId = r.WorkflowId,
                    SourceStageId = r.SourceStageId.Value,
                    TargetStageId = r.TargetStageId.Value
                })
                .ToListAsync();
        }

        /// <summary>
        /// Gets the workflow stage relation with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The workflow stage relation</returns>
        public async Task<Models.WorkflowStageRelation> GetById(Guid id)
        {
            var relation = await _db.WorkflowStageRelations
                .FirstOrDefaultAsync(r => r.Id == id);

            if (relation != null)
            {
                return new Models.WorkflowStageRelation
                {
                    Id = relation.Id,
                    WorkflowId = relation.WorkflowId,
                    SourceStageId = relation.SourceStageId.Value,
                    TargetStageId = relation.TargetStageId.Value
                };
            }
            return null;
        }

        /// <summary>
        /// Gets all workflow stage relations for the specified workflow.
        /// </summary>
        /// <param name="workflowId">The workflow id</param>
        /// <returns>The workflow stage relations</returns>
        public async Task<IEnumerable<Models.WorkflowStageRelation>> GetByWorkflowId(Guid workflowId)
        {
            return await _db.WorkflowStageRelations
                .Where(r => r.WorkflowId == workflowId)
                .Select(r => new Models.WorkflowStageRelation
                {
                    Id = r.Id,
                    WorkflowId = r.WorkflowId,
                    SourceStageId = r.SourceStageId.Value,
                    TargetStageId = r.TargetStageId.Value
                })
                .ToListAsync();
        }

        /// <summary>
        /// Gets all workflow stage relations for the specified source stage.
        /// </summary>
        /// <param name="stageId">The source stage id</param>
        /// <returns>The workflow stage relations</returns>
        public async Task<IEnumerable<Models.WorkflowStageRelation>> GetBySourceStageId(Guid stageId)
        {
            return await _db.WorkflowStageRelations
                .Where(r => r.SourceStageId == stageId)
                .Select(r => new Models.WorkflowStageRelation
                {
                    Id = r.Id,
                    WorkflowId = r.WorkflowId,
                    SourceStageId = r.SourceStageId.Value,
                    TargetStageId = r.TargetStageId.Value
                })
                .ToListAsync();
        }

        /// <summary>
        /// Gets all workflow stage relations for the specified target stage.
        /// </summary>
        /// <param name="stageId">The target stage id</param>
        /// <returns>The workflow stage relations</returns>
        public async Task<IEnumerable<Models.WorkflowStageRelation>> GetByTargetStageId(Guid stageId)
        {
            return await _db.WorkflowStageRelations
                .Where(r => r.TargetStageId == stageId)
                .Select(r => new Models.WorkflowStageRelation
                {
                    Id = r.Id,
                    WorkflowId = r.WorkflowId,
                    SourceStageId = r.SourceStageId.Value,
                    TargetStageId = r.TargetStageId.Value
                })
                .ToListAsync();
        }

        /// <summary>
        /// Adds or updates the given workflow stage relation.
        /// </summary>
        /// <param name="relation">The workflow stage relation</param>
        public async Task Save(Models.WorkflowStageRelation relation)
        {
            var dbRelation = await _db.WorkflowStageRelations
                .FirstOrDefaultAsync(r => r.Id == relation.Id);

            if (dbRelation == null)
            {
                dbRelation = new Data.WorkflowStageRelation
                {
                    Id = relation.Id != Guid.Empty ? relation.Id : Guid.NewGuid()
                };
                await _db.WorkflowStageRelations.AddAsync(dbRelation);
            }

            dbRelation.WorkflowId = relation.WorkflowId;
            dbRelation.SourceStageId = relation.SourceStageId;
            dbRelation.TargetStageId = relation.TargetStageId;

            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes the workflow stage relation with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        public async Task Delete(Guid id)
        {
            var relation = await _db.WorkflowStageRelations
                .FirstOrDefaultAsync(r => r.Id == id);

            if (relation != null)
            {
                _db.WorkflowStageRelations.Remove(relation);
                await _db.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Deletes the given workflow stage relation.
        /// </summary>
        /// <param name="relation">The workflow stage relation</param>
        public async Task Delete(Models.WorkflowStageRelation relation)
        {
            await Delete(relation.Id);
        }

        /// <summary>
        /// Deletes all relations for the specified workflow.
        /// </summary>
        /// <param name="workflowId">The workflow id</param>
        public async Task DeleteByWorkflow(Guid workflowId)
        {
            var relations = await _db.WorkflowStageRelations
                .Where(r => r.WorkflowId == workflowId)
                .ToListAsync();

            if (relations.Count > 0)
            {
                _db.WorkflowStageRelations.RemoveRange(relations);
                await _db.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Deletes all relations involving the specified stage (as source or target).
        /// </summary>
        /// <param name="stageId">The stage id</param>
        public async Task DeleteByStage(Guid stageId)
        {
            var relations = await _db.WorkflowStageRelations
                .Where(r => r.SourceStageId == stageId || r.TargetStageId == stageId)
                .ToListAsync();

            if (relations.Count > 0)
            {
                _db.WorkflowStageRelations.RemoveRange(relations);
                await _db.SaveChangesAsync();
            }
        }
    }
}
