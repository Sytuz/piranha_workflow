using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Piranha.Manager.Areas.Manager.Pages
{
    public class WorkflowEditModel : PageModel
    {
        /// <summary>
        /// Gets/sets the workflow id.
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public Guid? Id { get; set; }
        
        public void OnGet()
        {
            // Logic for loading workflow data if needed
        }
    }
}