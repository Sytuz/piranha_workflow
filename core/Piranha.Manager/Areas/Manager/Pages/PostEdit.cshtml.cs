/*
 * Copyright (c) 2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Piranha.Manager.Services;
using Piranha.Services;

namespace Piranha.Manager.Models
{
    [Authorize(Policy = Permission.PostsEdit)]
    public class PostEditViewModel : PageModel
    {
        private readonly Piranha.Manager.Services.WorkflowService _workflowService;
        private readonly IChangeRequestService _changeRequestService;
        private readonly IWorkflowStageService _stageService;
        private readonly IUserResolutionService _userResolutionService;

        public PostEditViewModel(Piranha.Manager.Services.WorkflowService workflowService, IChangeRequestService changeRequestService, IWorkflowStageService stageService, IUserResolutionService userResolutionService)
        {
            _workflowService = workflowService;
            _changeRequestService = changeRequestService;
            _stageService = stageService;
            _userResolutionService = userResolutionService;
        }

        /// <summary>
        /// Gets the current user ID.
        /// </summary>
        public string UserId { get; private set; }

        /// <summary>
        /// Gets the workflow ID.
        /// </summary>
        public Guid? WorkflowId { get; private set; }

        /// <summary>
        /// The transition history for the current change request (if any).
        /// </summary>
        public IEnumerable<Piranha.Manager.Models.ChangeRequestTransitionViewModel> ChangeRequestTransitions { get; set; }

        public async Task OnGetAsync()
        {
            // Get current user ID
            UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Get workflow
            try
            {
                var workflows = await _workflowService.GetAllAsync();
                var workflow = workflows.FirstOrDefault(w => w.IsEnabled);
                WorkflowId = workflow?.Id;
            }
            catch (Exception ex)
            {
                // If there's an error getting workflows, set to null
                WorkflowId = null;
                Console.WriteLine("Error retrieving workflows. WorkflowId set to null.");
                // Log the error message from exception
                Console.WriteLine("Exception: " + ex.Message);
            }

            // Load transition history for change requests related to this post
            try
            {
                // Get the post ID from route data
                if (RouteData.Values.TryGetValue("id", out var postIdValue) && Guid.TryParse(postIdValue?.ToString(), out var postId))
                {
                    // Get change requests for this post content
                    var changeRequests = await _changeRequestService.GetByContentIdAsync(postId);
                    var transitionViewModels = new List<Piranha.Manager.Models.ChangeRequestTransitionViewModel>();

                    // For each change request, get its transition history
                    foreach (var changeRequest in changeRequests)
                    {
                        var transitions = (await _changeRequestService.GetTransitionsAsync(changeRequest.Id)).ToList();
                        
                        if (transitions.Any())
                        {
                            var stageIds = transitions.SelectMany(t => new[] { t.FromStageId, t.ToStageId }).Distinct().ToList();
                            var userIds = transitions.Select(t => t.UserId).Distinct().ToList();

                            // Resolve stage titles
                            var stageTitles = new Dictionary<Guid, string>();
                            foreach (var stageId in stageIds)
                            {
                                var stage = await _stageService.GetByIdAsync(stageId);
                                stageTitles[stageId] = stage?.Title ?? "Unknown";
                            }

                            // Resolve user names
                            var userNames = await _userResolutionService.GetUserNamesByIdsAsync(userIds);

                            // Map to view models and add to the list
                            var transitionVMs = transitions.Select(t => new Piranha.Manager.Models.ChangeRequestTransitionViewModel
                            {
                                TransitionedAt = t.Timestamp,
                                FromStageTitle = stageTitles.TryGetValue(t.FromStageId, out var fromTitle) ? fromTitle : "Unknown",
                                ToStageTitle = stageTitles.TryGetValue(t.ToStageId, out var toTitle) ? toTitle : "Unknown",
                                UserName = userNames.TryGetValue(t.UserId, out var userName) ? userName : "Unknown",
                                Notes = string.Empty, // You could resolve comment text here if needed
                                ActionType = t.ActionType
                            });
                            
                            transitionViewModels.AddRange(transitionVMs);
                        }
                    }

                    // Sort by timestamp (most recent first)
                    ChangeRequestTransitions = transitionViewModels.OrderByDescending(t => t.TransitionedAt);
                }
                else
                {
                    ChangeRequestTransitions = new List<Piranha.Manager.Models.ChangeRequestTransitionViewModel>();
                }
            }
            catch (Exception)
            {
                // If there's an error loading transition history, assign an empty list
                ChangeRequestTransitions = new List<Piranha.Manager.Models.ChangeRequestTransitionViewModel>();
            }
        }
    }
}