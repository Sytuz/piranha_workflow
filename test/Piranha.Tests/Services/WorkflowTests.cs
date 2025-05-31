
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Piranha.Models;

namespace Piranha.Tests.Services
{
    [Collection("Integration tests")]
    public class WorkflowTests : BaseTestsAsync
    {
        public readonly Guid WORKFLOW_ID_1 = Guid.NewGuid();
        public readonly Guid WORKFLOW_ID_2 = Guid.NewGuid();
        public readonly Guid STAGE_ID_1 = Guid.NewGuid();
        public readonly Guid STAGE_ID_2 = Guid.NewGuid();
        
        public override async Task InitializeAsync()
        {
            using (var api = CreateApi())
            {
                Piranha.App.Init(api);

                // Add some test workflows
                var workflow1 = new Workflow
                {
                    Id = WORKFLOW_ID_1,
                    Title = "Test Workflow 1",
                    Description = "First workflow for testing",
                    IsDefault = true,
                    IsEnabled = true,
                    Created = DateTime.Now,
                    LastModified = DateTime.Now
                };
                
                workflow1.Stages = new List<WorkflowStage>
                {
                    new WorkflowStage
                    {
                        Id = STAGE_ID_1,
                        WorkflowId = workflow1.Id,
                        Title = "Draft",
                        Description = "Initial draft",
                        SortOrder = 1,
                        IsPublished = false
                    },
                    new WorkflowStage
                    {
                        Id = STAGE_ID_2,
                        WorkflowId = workflow1.Id,
                        Title = "Review",
                        Description = "Under review",
                        SortOrder = 2,
                        IsPublished = false
                    }
                };
                
                await api.Workflows.SaveAsync(workflow1);

                var workflow2 = new Workflow
                {
                    Id = WORKFLOW_ID_2,
                    Title = "Test Workflow 2",
                    Description = "Second workflow for testing",
                    IsDefault = false,
                    IsEnabled = true,
                    Created = DateTime.Now,
                    LastModified = DateTime.Now
                };
                await api.Workflows.SaveAsync(workflow2);
            }
        }

        public override async Task DisposeAsync()
        {
            using (var api = CreateApi())
            {
                var workflows = await api.Workflows.GetAllAsync();
                foreach (var workflow in workflows.Where(w => w.Id == WORKFLOW_ID_1 || w.Id == WORKFLOW_ID_2))
                {
                    await api.Workflows.DeleteAsync(workflow.Id);
                }
            }
        }

        [Fact]
        public async Task GetAllWorkflows()
        {
            using (var api = CreateApi())
            {
                var workflows = await api.Workflows.GetAllAsync();

                Assert.NotNull(workflows);
                Assert.True(workflows.Count() >= 2);
            }
        }

        [Fact]
        public async Task GetWorkflowById()
        {
            using (var api = CreateApi())
            {
                var workflow = await api.Workflows.GetByIdAsync(WORKFLOW_ID_1);

                Assert.NotNull(workflow);
                Assert.Equal(WORKFLOW_ID_1, workflow.Id);
                Assert.Equal("Test Workflow 1", workflow.Title);
                Assert.True(workflow.IsDefault);
                Assert.True(workflow.IsEnabled);
            }
        }

        [Fact]
        public async Task CreateNewWorkflow()
        {
            using (var api = CreateApi())
            {
                var newWorkflow = new Workflow
                {
                    Id = Guid.NewGuid(),
                    Title = "New Test Workflow",
                    Description = "A new test workflow",
                    IsDefault = false,
                    IsEnabled = true,
                    Created = DateTime.Now,
                    LastModified = DateTime.Now
                };

                await api.Workflows.SaveAsync(newWorkflow);

                var savedWorkflow = await api.Workflows.GetByIdAsync(newWorkflow.Id);
                Assert.NotNull(savedWorkflow);
                Assert.Equal("New Test Workflow", savedWorkflow.Title);
                Assert.Equal("A new test workflow", savedWorkflow.Description);
                Assert.False(savedWorkflow.IsDefault);
                Assert.True(savedWorkflow.IsEnabled);

                // Cleanup
                await api.Workflows.DeleteAsync(newWorkflow.Id);
            }
        }

        [Fact]
        public async Task UpdateWorkflow()
        {
            using (var api = CreateApi())
            {
                var workflow = await api.Workflows.GetByIdAsync(WORKFLOW_ID_2);
                Assert.NotNull(workflow);

                var originalTitle = workflow.Title;
                workflow.Title = "Updated Workflow Title";
                workflow.Description = "Updated description";
                workflow.LastModified = DateTime.Now;

                await api.Workflows.SaveAsync(workflow);

                var updatedWorkflow = await api.Workflows.GetByIdAsync(WORKFLOW_ID_2);
                Assert.Equal("Updated Workflow Title", updatedWorkflow.Title);
                Assert.Equal("Updated description", updatedWorkflow.Description);
                Assert.NotEqual(originalTitle, updatedWorkflow.Title);
            }
        }

        [Fact]
        public async Task ToggleWorkflowEnabled()
        {
            using (var api = CreateApi())
            {
                var workflow = await api.Workflows.GetByIdAsync(WORKFLOW_ID_2);
                Assert.NotNull(workflow);
                
                var originalState = workflow.IsEnabled;

                await api.Workflows.ToggleEnabledAsync(WORKFLOW_ID_2);

                var toggledWorkflow = await api.Workflows.GetByIdAsync(WORKFLOW_ID_2);
                Assert.NotEqual(originalState, toggledWorkflow.IsEnabled);

                // Toggle back
                await api.Workflows.ToggleEnabledAsync(WORKFLOW_ID_2);
                
                var restoredWorkflow = await api.Workflows.GetByIdAsync(WORKFLOW_ID_2);
                Assert.Equal(originalState, restoredWorkflow.IsEnabled);
            }
        }

        [Fact]
        public async Task CreateStandardWorkflow()
        {
            using (var api = CreateApi())
            {
                var standardWorkflow = await api.Workflows.CreateStandardWorkflowAsync(
                    "Standard Test Workflow", 
                    "A standard workflow created for testing"
                );

                Assert.NotNull(standardWorkflow);
                Assert.Equal("Standard Test Workflow", standardWorkflow.Title);
                Assert.Equal("A standard workflow created for testing", standardWorkflow.Description);
                Assert.NotNull(standardWorkflow.Stages);
                Assert.True(standardWorkflow.Stages.Any());

                // Cleanup
                await api.Workflows.DeleteAsync(standardWorkflow.Id);
            }
        }

        [Fact]
        public async Task CheckUniqueTitleValidation()
        {
            using (var api = CreateApi())
            {
                // Test existing title
                var isUnique1 = await api.Workflows.IsUniqueTitleAsync("Test Workflow 1");
                Assert.False(isUnique1);

                // Test new title
                var isUnique2 = await api.Workflows.IsUniqueTitleAsync("Completely New Workflow Title");
                Assert.True(isUnique2);

                // Test existing title but excluding the workflow itself
                var isUnique3 = await api.Workflows.IsUniqueTitleAsync("Test Workflow 1", WORKFLOW_ID_1);
                Assert.True(isUnique3);
            }
        }

        [Fact]
        public async Task GetWorkflowStages()
        {
            using (var api = CreateApi())
            {
                var stages = await api.WorkflowStages.GetAllAsync(WORKFLOW_ID_1);

                Assert.NotNull(stages);
                Assert.True(stages.Count() >= 2);
                
                var draftStage = stages.FirstOrDefault(s => s.Title == "Draft");
                var reviewStage = stages.FirstOrDefault(s => s.Title == "Review");
                
                Assert.NotNull(draftStage);
                Assert.NotNull(reviewStage);
                Assert.Equal(STAGE_ID_1, draftStage.Id);
                Assert.Equal(STAGE_ID_2, reviewStage.Id);
            }
        }

        [Fact]
        public async Task CreateAndManageWorkflowStage()
        {
            using (var api = CreateApi())
            {
                var newStage = new WorkflowStage
                {
                    Id = Guid.NewGuid(),
                    WorkflowId = WORKFLOW_ID_2,
                    Title = "Approval",
                    Description = "Final approval stage",
                    SortOrder = 3,
                    IsPublished = true
                };

                await api.WorkflowStages.SaveAsync(newStage);

                var savedStage = await api.WorkflowStages.GetByIdAsync(newStage.Id);
                Assert.NotNull(savedStage);
                Assert.Equal("Approval", savedStage.Title);
                Assert.Equal("Final approval stage", savedStage.Description);
                Assert.Equal(3, savedStage.SortOrder);
                Assert.True(savedStage.IsPublished);

                // Update the stage
                savedStage.Title = "Updated Approval";
                await api.WorkflowStages.SaveAsync(savedStage);

                var updatedStage = await api.WorkflowStages.GetByIdAsync(newStage.Id);
                Assert.Equal("Updated Approval", updatedStage.Title);

                // Cleanup
                await api.WorkflowStages.DeleteAsync(newStage.Id);
            }
        }

        [Fact]
        public async Task DeleteWorkflow()
        {
            using (var api = CreateApi())
            {
                var newWorkflow = new Workflow
                {
                    Id = Guid.NewGuid(),
                    Title = "Workflow To Delete",
                    Description = "This workflow will be deleted",
                    IsDefault = false,
                    IsEnabled = true,
                    Created = DateTime.Now,
                    LastModified = DateTime.Now
                };

                await api.Workflows.SaveAsync(newWorkflow);

                var savedWorkflow = await api.Workflows.GetByIdAsync(newWorkflow.Id);
                Assert.NotNull(savedWorkflow);

                await api.Workflows.DeleteAsync(newWorkflow.Id);

                var deletedWorkflow = await api.Workflows.GetByIdAsync(newWorkflow.Id);
                Assert.Null(deletedWorkflow);
            }
        }
    }
}
