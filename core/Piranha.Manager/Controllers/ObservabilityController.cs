using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Piranha.Manager.Services;

namespace Piranha.Manager.Controllers
{
    /// <summary>
    /// Direct controller for observability dashboard (bypasses Manager area authentication).
    /// </summary>
    [Route("observability")]
    [AllowAnonymous]
    public class ObservabilityController : Controller
    {
        private readonly ITelemetryService _telemetryService;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="telemetryService">The telemetry service</param>
        public ObservabilityController(ITelemetryService telemetryService)
        {
            _telemetryService = telemetryService;
        }

        /// <summary>
        /// Gets the observability dashboard page.
        /// </summary>
        /// <returns>The dashboard view</returns>
        public IActionResult Index()
        {
            return View("/Areas/Manager/Pages/Observability.cshtml");
        }
    }
}
