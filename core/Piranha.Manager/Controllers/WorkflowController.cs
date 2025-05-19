using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Piranha.Manager.Controllers
{
    [Area("Manager")]
    [Authorize(Policy = Permission.Admin)]
    public class WorkflowController : Controller
    {
        [Route("manager/workflow")]
        public IActionResult List()
        {
            return View("List");
        }

        [Route("manager/workflow/edit/{id?}")]
        public IActionResult Edit(Guid? id)
        {
            return View("Edit");
        }
    }
}
