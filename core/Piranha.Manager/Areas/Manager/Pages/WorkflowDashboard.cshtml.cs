using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Piranha.Manager.Services;
using Piranha.Security;

namespace Piranha.Manager.Areas.Manager.Pages
{
    /// <summary>
    /// Page model for the workflow dashboard.
    /// </summary>
    [Authorize(Policy = Permission.Admin)]
    public class WorkflowDashboardModel : PageModel
    {
        private readonly IApi _api;
        private readonly ManagerLocalizer _localizer;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <param name="localizer">The manager localizer</param>
        public WorkflowDashboardModel(IApi api, ManagerLocalizer localizer)
        {
            _api = api;
            _localizer = localizer;
        }

        /// <summary>
        /// Gets the dashboard page.
        /// </summary>
        public IActionResult OnGet()
        {
            return Page();
        }
    }
}
