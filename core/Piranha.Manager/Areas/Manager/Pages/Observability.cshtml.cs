using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Piranha.Security;

namespace Piranha.Manager.Areas.Manager.Pages
{
    /// <summary>
    /// Page model for the observability dashboard.
    /// </summary>
    [AllowAnonymous] // Allow anonymous access for observability monitoring
    public class ObservabilityModel : PageModel
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ObservabilityModel()
        {
        }

        /// <summary>
        /// Gets the observability dashboard page.
        /// </summary>
        /// <returns>The page result</returns>
        public IActionResult OnGet()
        {
            return Page();
        }
    }
}
