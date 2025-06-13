/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Piranha.Manager.Extend;

namespace Piranha.Manager.Models
{
    /// <summary>
    /// View model for the my tasks page.
    /// </summary>
    public class MyTasksViewModel
    {
        /// <summary>
        /// Gets/sets the available menu actions.
        /// </summary>
        public IList<ToolbarAction> Items { get; set; } = new List<ToolbarAction>();
    }
}
