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
        var workflowToDelete = await _repo.GetByIdAsync(id).ConfigureAwait(false);
        if (workflowToDelete == null)
        {
            return; // Or throw not found
        }

        var allWorkflows = (await _repo.GetAllAsync().ConfigureAwait(false)).ToList();

        // Prevent deleting the last workflow
        if (allWorkflows.Count <= 1)
        {
            throw new ValidationException("Cannot delete the only workflow in the system.");
        }

        // Prevent deleting the last enabled workflow
        var enabledWorkflows = allWorkflows.Where(w => w.IsEnabled).ToList();
        if (workflowToDelete.IsEnabled && enabledWorkflows.Count == 1 && enabledWorkflows[0].Id == id)
        {
            throw new ValidationException("Cannot delete the last enabled workflow. Please disable it first or enable another workflow.");
        }

        if (workflowToDelete.IsDefault)
        {
            // Find another enabled workflow to set as default
            var nextDefault = allWorkflows.FirstOrDefault(w => w.Id != id && w.IsEnabled);
            if (nextDefault != null)
            {
                nextDefault.IsDefault = true;
                await _repo.SaveAsync(nextDefault).ConfigureAwait(false);
            }
            else
            {
                // This case should ideally be prevented by the "last enabled workflow" check if it was also default.
                // If somehow reached, it means we are deleting the default and no other enabled workflow exists to take over.
                // Depending on requirements, this could be an error or allowed.
                // For now, the above check for last enabled workflow should cover this.
            }
        }

        await _repo.DeleteAsync(id).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task ToggleEnabledAsync(Guid id)
    {
        var workflow = await _repo.GetByIdAsync(id).ConfigureAwait(false);
        if (workflow == null)
        {
            throw new KeyNotFoundException($"Workflow with id {id} not found.");
        }

        var wasEnabled = workflow.IsEnabled;
        workflow.IsEnabled = !workflow.IsEnabled;
        workflow.LastModified = DateTime.Now;

        var allWorkflows = (await _repo.GetAllAsync().ConfigureAwait(false)).ToList();

        if (workflow.IsEnabled) // Workflow is being enabled
        {
            // If no other workflow is default, make this one default
            if (!allWorkflows.Any(w => w.Id != workflow.Id && w.IsDefault))
            {
                workflow.IsDefault = true;
            }
        }
        else // Workflow is being disabled
        {
            if (workflow.IsDefault)
            {
                // This was the default workflow, try to set another enabled one as default
                var nextDefault = allWorkflows.FirstOrDefault(w => w.Id != workflow.Id && w.IsEnabled);
                if (nextDefault != null)
                {
                    nextDefault.IsDefault = true;
                    await _repo.SaveAsync(nextDefault).ConfigureAwait(false);
                }
                else
                {
                    // This is the last enabled workflow and it was default. Prevent disabling.
                    workflow.IsEnabled = true; // Revert
                    workflow.LastModified = DateTime.Now; // Revert modification time or handle appropriately
                    await _repo.SaveAsync(workflow).ConfigureAwait(false); // Save reverted state
                    throw new ValidationException("Cannot disable the last enabled default workflow. Enable another workflow first to make it default.");
                }
                workflow.IsDefault = false; // No longer default as it's disabled
            }
        }
        await _repo.SaveAsync(workflow).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<Workflow> CreateStandardWorkflowAsync(string title, string description = null)
    {
        var workflow = new Workflow
        {
            Id = Guid.NewGuid(),
            Title = title,
            Description = description,
            Created = DateTime.Now,
            LastModified = DateTime.Now
        };

        // Check if this is the first workflow in the system
        var workflows = await _repo.GetAllAsync();
        if (!workflows.Any())
        {
            workflow.IsDefault = true;
            workflow.IsEnabled = true; // Automatically enable the first workflow
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
                Title = "Legal Review",
                Description = "Content under legal review",
                SortOrder = 3,
                IsPublished = true
            },
            new WorkflowStage
            {
                Id = Guid.NewGuid(),
                WorkflowId = workflow.Id,
                Title = "Approved",
                Description = "Content ready for publishing",
                SortOrder = 4,
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
            },
            new WorkflowStageRelation
            {
                Id = Guid.NewGuid(),
                WorkflowId = workflow.Id,
                SourceStageId = workflow.Stages[2].Id,
                TargetStageId = workflow.Stages[3].Id
            },
            new WorkflowStageRelation
            {
                Id = Guid.NewGuid(),
                WorkflowId = workflow.Id,
                SourceStageId = workflow.Stages[1].Id,
                TargetStageId = workflow.Stages[3].Id
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
