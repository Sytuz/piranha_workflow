using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Piranha.Manager.Areas.Manager.Pages
{
    [Authorize(Policy = Permission.Admin)]
    public class WorkflowListModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
