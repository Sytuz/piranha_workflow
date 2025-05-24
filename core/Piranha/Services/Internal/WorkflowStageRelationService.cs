/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Piranha.Models;
using Piranha.Repositories;

namespace Piranha.Services;

public class WorkflowStageRelationService : IWorkflowStageRelationService
{
    private readonly IWorkflowStageRelationRepository _repo;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="repo">The workflow stage relation repository</param>
    public WorkflowStageRelationService(IWorkflowStageRelationRepository repo)
    {
        _repo = repo;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<WorkflowStageRelation>> GetAllAsync(Guid workflowId)
    {
        return await _repo.GetByWorkflowId(workflowId).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<WorkflowStageRelation> GetByIdAsync(Guid id)
    {
        return await _repo.GetById(id).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<WorkflowStageRelation>> GetBySourceStageIdAsync(Guid sourceStageId)
    {
        return await _repo.GetBySourceStageId(sourceStageId).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<WorkflowStageRelation>> GetByTargetStageIdAsync(Guid targetStageId)
    {
        return await _repo.GetByTargetStageId(targetStageId).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task SaveAsync(WorkflowStageRelation relation)
    {
        // Set a new id if not set
        if (relation.Id == Guid.Empty)
        {
            relation.Id = Guid.NewGuid();
        }

        await _repo.Save(relation).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id)
    {
        await _repo.Delete(id).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(WorkflowStageRelation relation)
    {
        await _repo.Delete(relation).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task DeleteByWorkflowIdAsync(Guid workflowId)
    {
        await _repo.DeleteByWorkflow(workflowId).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task DeleteByStageIdAsync(Guid stageId)
    {
        await _repo.DeleteByStage(stageId).ConfigureAwait(false);
    }
}
