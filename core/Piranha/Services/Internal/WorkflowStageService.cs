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
    private readonly IWorkflowStageRoleRepository _roleRepo;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="repo">The workflow stage repository</param>
    /// <param name="roleRepo">The workflow stage role repository</param>
    public WorkflowStageService(IWorkflowStageRepository repo, IWorkflowStageRoleRepository roleRepo)
    {
        _repo = repo;
        _roleRepo = roleRepo;
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

        // Validate unique stage name within workflow
        var existingStages = await _repo.GetByWorkflowId(stage.WorkflowId).ConfigureAwait(false);
        var duplicateStage = existingStages.FirstOrDefault(s => s.Id != stage.Id && s.Title.Equals(stage.Title, StringComparison.OrdinalIgnoreCase));
        if (duplicateStage != null)
        {
            throw new ValidationException($"A stage with the title '{stage.Title}' already exists in this workflow");
        }

        // Validate at least one role is assigned
        if (stage.Roles == null || !stage.Roles.Any())
        {
            throw new ValidationException("At least one role must be assigned to the stage");
        }

        // Prevent editing of immutable stages
        if (stage.IsImmutable)
            throw new ValidationException("Immutable stages cannot be edited.");

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
        var stage = await _repo.GetById(id).ConfigureAwait(false);
        if (stage != null && stage.IsImmutable)
            throw new ValidationException("Immutable stages cannot be deleted.");

        // Delete associated roles first
        await _roleRepo.DeleteByWorkflowStageIdAsync(id).ConfigureAwait(false);
        await _repo.Delete(id).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(WorkflowStage stage)
    {
        if (stage.IsImmutable)
            throw new ValidationException("Immutable stages cannot be deleted.");

        // Delete associated roles first
        await _roleRepo.DeleteByWorkflowStageIdAsync(stage.Id).ConfigureAwait(false);
        await _repo.Delete(stage).ConfigureAwait(false);
    }

    /// <summary>
    /// Assigns roles to a workflow stage
    /// </summary>
    /// <param name="stageId">The stage id</param>
    /// <param name="roleIds">The role ids to assign</param>
    public async Task AssignRolesAsync(Guid stageId, IEnumerable<string> roleIds)
    {
        if (roleIds == null || !roleIds.Any())
        {
            throw new ValidationException("At least one role must be assigned to the stage");
        }

        // Remove existing role assignments
        await _roleRepo.DeleteByWorkflowStageIdAsync(stageId).ConfigureAwait(false);

        // Add new role assignments
        foreach (var roleId in roleIds)
        {
            var stageRole = new WorkflowStageRole
            {
                Id = Guid.NewGuid(),
                WorkflowStageId = stageId,
                RoleId = roleId
            };
            await _roleRepo.SaveAsync(stageRole).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Gets the roles assigned to a workflow stage
    /// </summary>
    /// <param name="stageId">The stage id</param>
    /// <returns>The assigned role ids</returns>
    public async Task<IEnumerable<string>> GetStageRolesAsync(Guid stageId)
    {
        var stageRoles = await _roleRepo.GetByWorkflowStageIdAsync(stageId).ConfigureAwait(false);
        return stageRoles.Select(sr => sr.RoleId);
    }

    /// <inheritdoc />
    public async Task ReorderAsync(Guid workflowId, Guid[] stageIds)
    {
        var stages = await _repo.GetByWorkflowId(workflowId).ConfigureAwait(false);
        if (stages.Any(s => s.IsImmutable && Array.IndexOf(stageIds, s.Id) != s.SortOrder - 1))
            throw new ValidationException("Immutable stages cannot be reordered.");

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
