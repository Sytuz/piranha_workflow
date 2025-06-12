using System;

namespace Piranha.Manager.Models
{
    /// <summary>
    /// View model for displaying a change request transition in the UI.
    /// </summary>
    public class ChangeRequestTransitionViewModel
    {
        public DateTime TransitionedAt { get; set; }
        public string FromStageTitle { get; set; }
        public string ToStageTitle { get; set; }
        public string UserName { get; set; }
        public string Notes { get; set; }
        public string ActionType { get; set; }
    }
}
