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

        model.LastModified = DateTime.Now;

        if (model.Id == Guid.Empty) // New workflow
        {
            model.Id = Guid.NewGuid();
            model.Created = DateTime.Now;

            // Get all available roles to initialize Draft stage
            var allRoles = await App.Roles.GetAllAsync();

            // Create Draft stage first
            var draftStageId = Guid.NewGuid();
            var draftStageRoles = allRoles.Select(role => new WorkflowStageRole
            {
                Id = Guid.NewGuid(),
                WorkflowStageId = draftStageId,
                RoleId = role.Id
            }).ToList();

            // If this is a new workflow, ensure it has the default two immutable stages
            // Only add these if no stages are already provided by the model (e.g. from CreateStandardWorkflowAsync)
            if (model.Stages == null || !model.Stages.Any())
            {
                var draftStage = new WorkflowStage
                {
                    Id = draftStageId,
                    WorkflowId = model.Id,
                    Title = "Draft",
                    Description = "Initial content creation",
                    SortOrder = 1,
                    IsPublished = false,
                    Color = "#c8c8c8",
                    IsImmutable = true,
                    Roles = draftStageRoles
                };
                var publishedStage = new WorkflowStage
                {
                    Id = Guid.NewGuid(),
                    WorkflowId = model.Id,
                    Title = "Published",
                    Description = "Final published stage",
                    SortOrder = 2,
                    IsPublished = true,
                    Color = "#4CAF50",
                    IsImmutable = true
                };
                model.Stages = new List<WorkflowStage> { draftStage, publishedStage };
                model.Relations = new List<WorkflowStageRelation>
                {
                    new WorkflowStageRelation
                    {
                        Id = Guid.NewGuid(),
                        WorkflowId = model.Id,
                        SourceStageId = draftStage.Id,
                        TargetStageId = publishedStage.Id
                    }
                };
            }

            var allCurrentWorkflows = await _repo.GetAllAsync().ConfigureAwait(false);
            if (!allCurrentWorkflows.Any(w => w.Id != model.Id)) // This new one will be the only one
            {
                model.IsEnabled = true;
                model.IsDefault = true;
            }
            else
            {
                model.IsEnabled = false;
                model.IsDefault = false;
            }
        }
        else // Existing workflow
        {
            // Preserve existing IsEnabled and IsDefault status.
            var existingWorkflow = await _repo.GetByIdAsync(model.Id).ConfigureAwait(false);
            if (existingWorkflow != null)
            {
                model.IsEnabled = existingWorkflow.IsEnabled;
                model.IsDefault = existingWorkflow.IsDefault;
            }
            // If existingWorkflow is null, it's an issue, but _repo.SaveAsync might handle it as an insert if ID doesn't match.
            // For this logic, we assume ID is valid and refers to an existing record.
        }

        // Set unique IDs for stages if not set (primarily for stages added manually to an existing workflow)
        foreach (var stage in model.Stages)
        {
            if (stage.Id == Guid.Empty)
            {
                stage.Id = Guid.NewGuid();
                stage.WorkflowId = model.Id; // Ensure WorkflowId is set for new stages
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
            // Or throw KeyNotFoundException if preferred
            return;
        }

        var allWorkflows = (await _repo.GetAllAsync().ConfigureAwait(false)).ToList();

        // Prevent deleting the only workflow in the system.
        if (allWorkflows.Count <= 1)
        {
            throw new ValidationException("Cannot delete the only workflow in the system.");
        }

        // Prevent deleting the currently enabled (active) workflow.
        if (workflowToDelete.IsEnabled)
        {
            throw new ValidationException("Cannot delete the active workflow. Please enable another workflow first.");
        }

        await _repo.DeleteAsync(id).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task ToggleEnabledAsync(Guid idToMakeEnabled)
    {
        var workflowToEnable = await _repo.GetByIdAsync(idToMakeEnabled).ConfigureAwait(false);
        if (workflowToEnable == null)
        {
            throw new KeyNotFoundException($"Workflow with id {idToMakeEnabled} not found.");
        }

        // If it's already enabled, there's nothing to do.
        if (workflowToEnable.IsEnabled)
        {
            return;
        }

        var allWorkflows = (await _repo.GetAllAsync().ConfigureAwait(false)).ToList();
        var now = DateTime.Now;

        foreach (var wf in allWorkflows)
        {
            bool changed = false;
            if (wf.Id == idToMakeEnabled)
            {
                if (!wf.IsEnabled) { wf.IsEnabled = true; changed = true; }
                if (!wf.IsDefault) { wf.IsDefault = true; changed = true; }
            }
            else
            {
                if (wf.IsEnabled) { wf.IsEnabled = false; changed = true; }
                if (wf.IsDefault) { wf.IsDefault = false; changed = true; }
            }

            if (changed)
            {
                wf.LastModified = now;
                await _repo.SaveAsync(wf).ConfigureAwait(false);
            }
        }
    }    /// <inheritdoc />
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
        }        // Get all available roles to initialize Draft stage
        var allRoles = await App.Roles.GetAllAsync();

        // Create Draft stage first to get its ID
        var draftStageId = Guid.NewGuid();
        var draftStageRoles = allRoles.Select(role => new WorkflowStageRole
        {
            Id = Guid.NewGuid(),
            WorkflowStageId = draftStageId,
            RoleId = role.Id
        }).ToList();

        // Add standard stages
        workflow.Stages = new List<WorkflowStage>
        {
            new WorkflowStage
            {
                Id = draftStageId,
                WorkflowId = workflow.Id,
                Title = "Draft",
                Description = "Initial content creation",
                SortOrder = 1,
                IsPublished = false,
                Color = "#c8c8c8",
                IsImmutable = true,
                Roles = draftStageRoles
            },
            new WorkflowStage
            {
                Id = Guid.NewGuid(),
                WorkflowId = workflow.Id,
                Title = "Review",
                Description = "Content under review",
                SortOrder = 2,
                IsPublished = false,
                IsImmutable = false // Explicitly set
            },
            new WorkflowStage
            {
                Id = Guid.NewGuid(),
                WorkflowId = workflow.Id,
                Title = "Legal Review",
                Description = "Content under legal review",
                SortOrder = 3,
                IsPublished = false,
                IsImmutable = false // Explicitly set
            },
            new WorkflowStage
            {
                Id = Guid.NewGuid(),
                WorkflowId = workflow.Id,
                Title = "Approved",
                Description = "Content ready for publishing",
                SortOrder = 4,
                IsPublished = false,
                IsImmutable = false // Explicitly set
            },
            new WorkflowStage
            {
                Id = Guid.NewGuid(),
                WorkflowId = workflow.Id,
                Title = "Published",
                Description = "Final published stage",
                SortOrder = 5,
                IsPublished = true,
                Color = "#4CAF50",
                IsImmutable = true
            }
        };

        // Add standard relations (no Draft→Published, but Approved→Published)
        workflow.Relations = new List<WorkflowStageRelation>
        {
            new WorkflowStageRelation
            {
                Id = Guid.NewGuid(),
                WorkflowId = workflow.Id,
                SourceStageId = workflow.Stages[0].Id, // Draft
                TargetStageId = workflow.Stages[1].Id  // Review
            },
            new WorkflowStageRelation
            {
                Id = Guid.NewGuid(),
                WorkflowId = workflow.Id,
                SourceStageId = workflow.Stages[1].Id, // Review
                TargetStageId = workflow.Stages[2].Id  // Legal Review
            },
            new WorkflowStageRelation
            {
                Id = Guid.NewGuid(),
                WorkflowId = workflow.Id,
                SourceStageId = workflow.Stages[2].Id, // Legal Review
                TargetStageId = workflow.Stages[3].Id  // Approved
            },
            new WorkflowStageRelation
            {
                Id = Guid.NewGuid(),
                WorkflowId = workflow.Id,
                SourceStageId = workflow.Stages[1].Id, // Review
                TargetStageId = workflow.Stages[3].Id  // Approved
            },
            new WorkflowStageRelation
            {
                Id = Guid.NewGuid(),
                WorkflowId = workflow.Id,
                SourceStageId = workflow.Stages[3].Id, // Approved
                TargetStageId = workflow.Stages[4].Id  // Published
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

    /// <summary>
    /// Initializes Draft stages for existing workflows with all available roles.
    /// This should be run once at project startup.
    /// </summary>
    public async Task InitializeDraftStageRolesAsync()
    {
        var allWorkflows = await _repo.GetAllAsync().ConfigureAwait(false);
        var allRoles = await App.Roles.GetAllAsync();
                
        foreach (var workflow in allWorkflows)
        {
            var draftStage = workflow.Stages?.FirstOrDefault(s => s.Title == "Draft");
            if (draftStage != null && (draftStage.Roles == null || !draftStage.Roles.Any()))
            {                
                draftStage.Roles = allRoles.Select(role => new WorkflowStageRole
                {
                    Id = Guid.NewGuid(),
                    WorkflowStageId = draftStage.Id,
                    RoleId = role.Id
                }).ToList();
                
                workflow.LastModified = DateTime.Now;
                await _repo.SaveAsync(workflow).ConfigureAwait(false);
            }
        }
    }
}
