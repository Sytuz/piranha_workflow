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
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Piranha.Manager.Services;

namespace Piranha.Manager.Models
{
    [Authorize(Policy = Permission.PostsEdit)]
    public class PostEditViewModel : PageModel
    {
        private readonly WorkflowService _workflowService;

        public PostEditViewModel(WorkflowService workflowService)
        {
            _workflowService = workflowService;
        }

        /// <summary>
        /// Gets the current user ID.
        /// </summary>
        public string UserId { get; private set; }

        /// <summary>
        /// Gets the default workflow ID.
        /// </summary>
        public Guid? DefaultWorkflowId { get; private set; }

        public async Task OnGetAsync()
        {
            // Get current user ID
            UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Get default workflow
            try
            {
                var workflows = await _workflowService.GetAllAsync();
                var defaultWorkflow = workflows.FirstOrDefault(w => w.IsDefault && w.IsEnabled);
                DefaultWorkflowId = defaultWorkflow?.Id;
            }
            catch (Exception)
            {
                // If there's an error getting workflows, set to null
                DefaultWorkflowId = null;
            }
        }
    }
}