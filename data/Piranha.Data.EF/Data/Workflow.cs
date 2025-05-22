using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Piranha.Data
{
    /*
     * Copyright (c) .NET Foundation and Contributors
     *
     * This software may be modified and distributed under the terms
     * of the MIT license. See the LICENSE file for details.
     *
     * https://github.com/piranhacms/piranha.core
     *
     */

    [Serializable]
    public sealed class Workflow
    {
        /// <summary>
        /// Gets/sets the unique id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets/sets the title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets/sets the optional description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets/sets if this is the default workflow.
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// Gets/sets if this workflow is enabled.
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Gets/sets the created date.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets/sets the last modification date.
        /// </summary>
        public DateTime LastModified { get; set; }

        /// <summary>
        /// Gets/sets the available stages.
        /// </summary>
        public IList<WorkflowStage> Stages { get; set; } = new List<WorkflowStage>();

        /// <summary>
        /// Gets/sets the available stage relations.
        /// </summary>
        public IList<WorkflowStageRelation> Relations { get; set; } = new List<WorkflowStageRelation>();
    }
}
