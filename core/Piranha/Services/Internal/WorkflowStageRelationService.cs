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
using System.ComponentModel.DataAnnotations; // Added for ValidationException

namespace Piranha.Services;

public class WorkflowStageRelationService : IWorkflowStageRelationService
{
    private readonly IWorkflowStageRelationRepository _repo;
    private readonly IWorkflowStageRepository _stageRepo; // Added

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="repo">The workflow stage relation repository</param>
    /// <param name="stageRepo">The workflow stage repository</param> // Added
    public WorkflowStageRelationService(IWorkflowStageRelationRepository repo, IWorkflowStageRepository stageRepo) // Modified
    {
        _repo = repo;
        _stageRepo = stageRepo; // Added
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
        // Validate that the "Published" stage (IsPublished=true, IsImmutable=true) cannot be a source stage
        var sourceStage = await _stageRepo.GetById(relation.SourceStageId).ConfigureAwait(false);
        if (sourceStage != null && sourceStage.IsPublished && sourceStage.IsImmutable)
        {
            throw new ValidationException("The 'Published' stage cannot be the source of new relations.");
        }

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
