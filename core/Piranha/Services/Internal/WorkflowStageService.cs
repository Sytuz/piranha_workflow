/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System.ComponentModel.DataAnnotations;
using Piranha.Models;
using Piranha.Repositories;

namespace Piranha.Services;

public class WorkflowStageService : IWorkflowStageService
{
    private readonly IWorkflowStageRepository _repo;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="repo">The workflow stage repository</param>
    public WorkflowStageService(IWorkflowStageRepository repo)
    {
        _repo = repo;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<WorkflowStage>> GetAllAsync(Guid workflowId)
    {
        return await _repo.GetByWorkflowId(workflowId).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<WorkflowStage> GetByIdAsync(Guid id)
    {
        return await _repo.GetById(id).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task SaveAsync(WorkflowStage stage)
    {
        // Validate title
        if (string.IsNullOrWhiteSpace(stage.Title))
        {
            throw new ValidationException("Title is required");
        }

        // Set a new id if not set
        if (stage.Id == Guid.Empty)
        {
            stage.Id = Guid.NewGuid();
        }

        await _repo.Save(stage).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id)
    {
        await _repo.Delete(id).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(WorkflowStage stage)
    {
        await _repo.Delete(stage).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task ReorderAsync(Guid workflowId, Guid[] stageIds)
    {
        var stages = await _repo.GetByWorkflowId(workflowId).ConfigureAwait(false);
        var ordered = new List<WorkflowStage>();

        foreach (var id in stageIds)
        {
            var stage = stages.FirstOrDefault(s => s.Id == id);
            if (stage != null)
            {
                ordered.Add(stage);
            }
        }

        for (var i = 0; i < ordered.Count; i++)
        {
            ordered[i].SortOrder = i + 1;
            await _repo.Save(ordered[i]).ConfigureAwait(false);
        }
    }
}
