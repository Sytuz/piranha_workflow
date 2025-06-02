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
            catch (Exception)
            {
                // If there's an error getting workflows, set to null
                WorkflowId = null;
            }
        }
    }
}