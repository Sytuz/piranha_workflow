using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Piranha.Manager.Models;
using Piranha.Models;
using Piranha.Services;

namespace Piranha.Manager.Controllers
{
    /// <summary>
    /// Api controller for workflow management.
    /// </summary>
    [Area("Manager")]
    [Route("manager/api/workflow")]
    [Authorize(Policy = Permission.Admin)]
    [ApiController]
    public class WorkflowApiController : Controller
    {
        private readonly IWorkflowService _service;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="service">The workflow service</param>
        public WorkflowApiController(IWorkflowService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all available workflows.
        /// </summary>
        /// <returns>The workflows</returns>
        [HttpGet]
        [Route("list")]
        public async Task<IActionResult> GetList()
        {
            return Ok(await _service.GetAllAsync());
        }

        /// <summary>
        /// Gets the workflow with the specified id.
        /// </summary>
        /// <param name="id">The workflow id</param>
        /// <returns>The workflow</returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            return Ok(await _service.GetByIdAsync(id));
        }

        /// <summary>
        /// Creates or updates the given workflow.
        /// </summary>
        /// <param name="model">The workflow</param>
        /// <returns>The saved workflow</returns>
        [HttpPost]
        [Route("save")]
        public async Task<IActionResult> Save([FromBody] Workflow model)
        {
            try
            {
                await _service.SaveAsync(model);
                return Ok(await _service.GetByIdAsync(model.Id));
            }
            catch (Exception e)
            {
                var error = new ErrorMessage
                {
                    Body = e.Message
                };
                return BadRequest(error);
            }
        }

        /// <summary>
        /// Creates a new workflow with standard stages.
        /// </summary>
        /// <param name="model">The model</param>
        /// <returns>The new workflow</returns>
        [HttpPost]
        [Route("create-standard")]
        public async Task<IActionResult> CreateStandard([FromBody] StandardWorkflowModel model)
        {
            try
            {
                var workflow = await _service.CreateStandardWorkflowAsync(model.Title, model.Description);
                return Ok(workflow);
            }
            catch (Exception e)
            {
                var error = new ErrorMessage
                {
                    Body = e.Message
                };
                return BadRequest(error);
            }
        }

        /// <summary>
        /// Deletes the workflow with the specified id.
        /// </summary>
        /// <param name="id">The workflow id</param>
        /// <returns>The result of the operation</returns>
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Checks if the given workflow title is unique.
        /// </summary>
        /// <param name="title">The title</param>
        /// <param name="id">Optional id to exclude</param>
        /// <returns>If the title is unique</returns>
        [HttpGet]
        [Route("title-unique")]
        public async Task<IActionResult> TitleUnique(string title, Guid? id = null)
        {
            return Ok(await _service.IsUniqueTitleAsync(title, id));
        }
    }

    /// <summary>
    /// Model for creating a standard workflow.
    /// </summary>
    public class StandardWorkflowModel
    {
        /// <summary>
        /// Gets/sets the workflow title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets/sets the optional description.
        /// </summary>
        public string Description { get; set; }
    }
}
