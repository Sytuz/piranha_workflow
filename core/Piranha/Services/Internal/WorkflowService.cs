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

public class WorkflowService : IWorkflowService
{
    private readonly IWorkflowRepository _repo;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="repo">The workflow repository</param>
    public WorkflowService(IWorkflowRepository repo)
    {
        _repo = repo;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Workflow>> GetAllAsync()
    {
        return await _repo.GetAllAsync().ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<Workflow> GetByIdAsync(Guid id)
    {
        return await _repo.GetByIdAsync(id).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task SaveAsync(Workflow model)
    {
        // Validate name
        if (string.IsNullOrWhiteSpace(model.Title))
        {
            throw new ValidationException("Name is required");
        }

        // Check for title uniqueness
        if (!await _repo.IsUniqueTitleAsync(model.Title, model.Id).ConfigureAwait(false))
        {
            throw new ValidationException("Name already in use");
        }

        // Set timestamps
        if (model.Id == Guid.Empty)
        {
            model.Id = Guid.NewGuid();
            model.Created = DateTime.Now;
        }
        model.LastModified = DateTime.Now;

        // If this workflow is set as default, unset any other default workflow
        if (model.IsDefault)
        {
            var workflows = await _repo.GetAllAsync();
            foreach (var workflow in workflows.Where(w => w.Id != model.Id && w.IsDefault))
            {
                workflow.IsDefault = false;
                await _repo.SaveAsync(workflow).ConfigureAwait(false);
            }
        }

        // Set unique IDs for stages if not set
        foreach (var stage in model.Stages)
        {
            if (stage.Id == Guid.Empty)
            {
                stage.Id = Guid.NewGuid();
            }
        }

        await _repo.SaveAsync(model).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id)
    {
        // Check if it's the only workflow before deleting
        var workflows = await _repo.GetAllAsync();
        if (workflows.Count() <= 1)
        {
            throw new ValidationException("Cannot delete the only workflow in the system");
        }
        
        // Check if it's the default workflow
        var workflow = await _repo.GetByIdAsync(id);
        if (workflow != null && workflow.IsDefault)
        {
            var otherWorkflow = workflows.FirstOrDefault(w => w.Id != id);
            if (otherWorkflow != null)
            {
                otherWorkflow.IsDefault = true;
                await _repo.SaveAsync(otherWorkflow).ConfigureAwait(false);
            }
        }

        await _repo.DeleteAsync(id).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<Workflow> CreateStandardWorkflowAsync(string title, string description = null)
    {
        var workflow = new Workflow
        {
            Id = Guid.NewGuid(),
            Title = title,
            Description = description,
            IsEnabled = true,
            Created = DateTime.Now,
            LastModified = DateTime.Now
        };

        // Check if this is the first workflow in the system
        var workflows = await _repo.GetAllAsync();
        if (!workflows.Any())
        {
            workflow.IsDefault = true;
        }

        // Add standard stages
        workflow.Stages = new List<WorkflowStage>
        {
            new WorkflowStage
            {
                Id = Guid.NewGuid(),
                WorkflowId = workflow.Id,
                Title = "Draft",
                Description = "Initial content creation",
                SortOrder = 1,
                IsPublished = false
            },
            new WorkflowStage
            {
                Id = Guid.NewGuid(),
                WorkflowId = workflow.Id,
                Title = "Review",
                Description = "Content under review",
                SortOrder = 2,
                IsPublished = false
            },
            new WorkflowStage
            {
                Id = Guid.NewGuid(),
                WorkflowId = workflow.Id,
                Title = "Approved",
                Description = "Content ready for publishing",
                SortOrder = 3,
                IsPublished = true
            }
        };

        // Add standard relations
        workflow.Relations = new List<WorkflowStageRelation>
        {
            new WorkflowStageRelation
            {
                Id = Guid.NewGuid(),
                WorkflowId = workflow.Id,
                SourceStageId = workflow.Stages[0].Id,
                TargetStageId = workflow.Stages[1].Id
            },
            new WorkflowStageRelation
            {
                Id = Guid.NewGuid(),
                WorkflowId = workflow.Id,
                SourceStageId = workflow.Stages[1].Id,
                TargetStageId = workflow.Stages[2].Id
            }
        };

        await SaveAsync(workflow).ConfigureAwait(false);
        return workflow;
    }

    /// <inheritdoc />
    public async Task<bool> IsUniqueTitleAsync(string title, Guid? id = null)
    {
        return await _repo.IsUniqueTitleAsync(title, id).ConfigureAwait(false);
    }
}
