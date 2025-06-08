/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Piranha.Manager.Models;

namespace Piranha.Manager.Controllers
{
    /// <summary>
    /// Controller for the my tasks page.
    /// </summary>
    [Area("Manager")]
    [Route("manager/mytasks")]
    [Authorize(Policy = Permission.ChangeRequests)]
    [AutoValidateAntiforgeryToken]
    public class MyTasksController : ManagerController
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public MyTasksController() { }

        /// <summary>
        /// Gets the list view for my tasks.
        /// </summary>
        [HttpGet]
        [Route("")]
        public IActionResult List()
        {
            var model = new MyTasksViewModel();
            return View(model);
        }
    }
}
