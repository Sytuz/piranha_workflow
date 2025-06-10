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
using Piranha.Models;

namespace Piranha.Manager.Models
{
    [Authorize(Policy = Permission.PagesEdit)]
    public class PageEditViewModel : PageModel
    {
        private readonly WorkflowService _workflowService;

        public PageEditViewModel(WorkflowService workflowService)
        {
            _workflowService = workflowService;
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
            UserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            // Get workflow
            try
            {
                var workflows = await _workflowService.GetAllAsync();
                var workflow = workflows.FirstOrDefault(w => w.IsEnabled);
                WorkflowId = workflow?.Id;
            }
            catch (Exception)
            {
                // If there's an error getting workflows, set to null
                WorkflowId = null;
            }

            // TODO: Load transition history for the current change request if available
            // For now, assign an empty list of view models
            ChangeRequestTransitions = new List<Piranha.Manager.Models.ChangeRequestTransitionViewModel>();
        }
    }
}