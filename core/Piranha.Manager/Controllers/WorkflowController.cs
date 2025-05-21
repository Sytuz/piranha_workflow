using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Piranha.Manager.Controllers
{
    [Area("Manager")]
    [Authorize(Policy = Permission.Admin)]
    public class WorkflowController : Controller
    {
        [Route("manager/workflow/list")]
        public IActionResult List()
        {
            return RedirectToPage("/WorkflowList", new { area = "Manager" });
        }

        [Route("manager/workflow/edit/{id:Guid?}")]
        public IActionResult Edit(Guid? id)
        {
            if (id.HasValue)
            {
                return RedirectToPage("/WorkflowEdit", new { area = "Manager", id = id.Value });
            }
            return RedirectToPage("/WorkflowEdit", new { area = "Manager" });
        }
    }
}
