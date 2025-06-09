using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using Piranha;
using Piranha.Manager;

namespace Piranha.Manager.Areas.Manager.Pages
{
    public class MyTasksModel : PageModel
    {
        private readonly IApi _api;
        private readonly ManagerLocalizer _localizer;

        public MyTasksModel(IApi api, ManagerLocalizer localizer)
        {
            _api = api;
            _localizer = localizer;
        }

        public void OnGet()
        {
            // Add any server-side logic here if needed.
        }
    }
}
