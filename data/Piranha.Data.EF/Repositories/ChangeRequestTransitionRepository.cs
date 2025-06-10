using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Piranha.Data;
using Piranha.Models;

namespace Piranha.Repositories
{
    using DataTransition = Piranha.Data.ChangeRequestTransition;
    using ModelTransition = Piranha.Models.ChangeRequestTransition;

    public class ChangeRequestTransitionRepository : IChangeRequestTransitionRepository
    {
        private readonly IDb _db;

        public ChangeRequestTransitionRepository(IDb db)
        {
            _db = db;
        }

        public async Task<IEnumerable<ModelTransition>> GetByChangeRequestIdAsync(Guid changeRequestId)
        {
            return await _db.ChangeRequestTransitions
                .AsNoTracking()
                .Where(t => t.ChangeRequestId == changeRequestId)
                .OrderBy(t => t.Timestamp)
                .Select(t => new ModelTransition
                {
                    Id = t.Id,
                    ChangeRequestId = t.ChangeRequestId,
                    FromStageId = t.FromStageId,
                    ToStageId = t.ToStageId,
                    UserId = t.UserId,
                    Timestamp = t.Timestamp,
                    ActionType = t.ActionType,
                    CommentId = t.CommentId,
                    ContentSnapshot = t.ContentSnapshot
                })
                .ToListAsync();
        }

        public async Task SaveAsync(ModelTransition transition)
        {
            var entity = await _db.ChangeRequestTransitions.FirstOrDefaultAsync(t => t.Id == transition.Id);
            if (entity == null)
            {
                entity = new DataTransition
                {
                    Id = transition.Id != Guid.Empty ? transition.Id : Guid.NewGuid(),
                    Timestamp = transition.Timestamp == default ? DateTime.UtcNow : transition.Timestamp
                };
                await _db.ChangeRequestTransitions.AddAsync(entity);
            }
            entity.ChangeRequestId = transition.ChangeRequestId;
            entity.FromStageId = transition.FromStageId;
            entity.ToStageId = transition.ToStageId;
            entity.UserId = transition.UserId;
            entity.ActionType = transition.ActionType;
            entity.CommentId = transition.CommentId;
            entity.ContentSnapshot = transition.ContentSnapshot;
            await _db.SaveChangesAsync();
            transition.Id = entity.Id;
            transition.Timestamp = entity.Timestamp;
        }
    }
}
