using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Piranha.Models;
using Piranha.Manager.Models;
using Piranha.Manager.Services;
using System.ComponentModel.DataAnnotations;
using System.Linq;

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
        private readonly WorkflowService _service;
        private readonly IApi _api;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="service">The workflow service</param>
        /// <param name="api">The main API</param>
        public WorkflowApiController(WorkflowService service, IApi api)
        {
            _service = service;
            _api = api;
        }

        /// <summary>
        /// Gets all available workflows.
        /// </summary>
        /// <returns>The workflows</returns>
        [HttpGet]
        [Route("list")]
        public async Task<IActionResult> GetList()
        {
            try
            {
                return Ok(await _service.GetAllAsync());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorMessage
                {
                    Body = $"Error retrieving workflows: {ex.Message}"
                });
            }
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
        /// Toggles the enabled state of the workflow with the specified id.
        /// </summary>
        /// <param name="id">The workflow id</param>
        /// <returns>The updated workflow</returns>
        [HttpPost]
        [Route("{id}/toggle-enabled")]
        public async Task<IActionResult> ToggleEnabled(Guid id)
        {
            try
            {
                await _service.ToggleEnabledAsync(id);
                return Ok(await _service.GetByIdAsync(id));
            }
            catch (ValidationException e)
            {
                var error = new ErrorMessage { Body = e.Message };
                return BadRequest(error);
            }
            catch (KeyNotFoundException e)
            {
                var error = new ErrorMessage { Body = e.Message };
                return NotFound(error);
            }
            catch (Exception e)
            {
                var error = new ErrorMessage { Body = e.Message };
                return StatusCode(500, error);
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
            try
            {
                await _service.DeleteAsync(id);
                return NoContent();
            }
            catch (ValidationException e)
            {
                // Return a 400 Bad Request for validation errors
                return BadRequest(new { message = e.Message });
            }
            catch (Exception e)
            {
                // Log the exception details (e.g., using a logging framework)
                // For now, return a 500 error with the exception message
                return StatusCode(500, new { message = e.Message, details = e.ToString() });
            }
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
