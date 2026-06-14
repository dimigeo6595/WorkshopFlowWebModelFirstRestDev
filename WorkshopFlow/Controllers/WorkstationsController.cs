using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkshopFlow.DTO;
using WorkshopFlow.Services;

namespace WorkshopFlow.Controllers
{
    [ApiController]
    [Route("api/v1/workstations")]
    public class WorkstationsController : ControllerBase
    {
        private readonly IApplicationService _applicationService;

        public WorkstationsController(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        /// <summary>
        /// Gets all workstations.
        /// </summary>
        /// <response code="200">Returns all workstations.</response>
        /// <response code="401">If the request is not authenticated.</response>
        /// <response code="403">If the user lacks permission.</response>
        [HttpGet]
        [Authorize(Policy = "VIEW_MACHINES")]
        [ProducesResponseType(typeof(IEnumerable<WorkstationReadOnlyDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<WorkstationReadOnlyDTO>>> GetWorkstations()
        {
            var workstations = await _applicationService.WorkstationService.GetAllWorkstationsAsync();
            return Ok(workstations);
        }

        /// <summary>
        /// Gets a workstation by its ID.
        /// </summary>
        /// <response code="200">Returns the workstation.</response>
        /// <response code="401">If the request is not authenticated.</response>
        /// <response code="403">If the user lacks permission.</response>
        /// <response code="404">If no workstation exists with the given ID.</response>
        [HttpGet("{id:int}")]
        [Authorize(Policy = "VIEW_MACHINES")]
        [ProducesResponseType(typeof(WorkstationReadOnlyDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<WorkstationReadOnlyDTO>> GetWorkstationById(int id)
        {
            var workstation = await _applicationService.WorkstationService.GetWorkstationByIdAsync(id);
            return Ok(workstation);
        }

        /// <summary>
        /// Creates a new workstation.
        /// </summary>
        /// <response code="201">Returns the created workstation.</response>
        /// <response code="400">If the request is invalid.</response>
        /// <response code="409">If a workstation with the same code already exists.</response>
        [HttpPost]
        [Authorize(Policy = "EDIT_MACHINES")]
        [ProducesResponseType(typeof(WorkstationReadOnlyDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<WorkstationReadOnlyDTO>> CreateWorkstation(
            [FromBody] WorkstationInsertDTO dto)
        {
            var createdWorkstation = await _applicationService.WorkstationService
                .InsertWorkstationAsync(dto);

            return CreatedAtAction(
                actionName: nameof(GetWorkstationById),
                routeValues: new { id = createdWorkstation.Id },
                value: createdWorkstation);
        }

        /// <summary>
        /// Updates a workstation.
        /// </summary>
        /// <response code="200">Returns the updated workstation.</response>
        /// <response code="400">If the request is invalid.</response>
        /// <response code="404">If no workstation exists with the given ID.</response>
        [HttpPut("{id:int}")]
        [Authorize(Policy = "EDIT_MACHINES")]
        [ProducesResponseType(typeof(WorkstationReadOnlyDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<WorkstationReadOnlyDTO>> UpdateWorkstation(
            int id, [FromBody] WorkstationUpdateDTO dto)
        {
            var updatedWorkstation = await _applicationService.WorkstationService
                .UpdateWorkstationAsync(id, dto);
            return Ok(updatedWorkstation);
        }

        /// <summary>
        /// Soft deletes a workstation.
        /// </summary>
        /// <response code="204">Delete successful.</response>
        /// <response code="404">If no workstation exists with the given ID.</response>
        [HttpDelete("{id:int}")]
        [Authorize(Policy = "EDIT_MACHINES")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteWorkstation(int id)
        {
            await _applicationService.WorkstationService.DeleteWorkstationAsync(id);
            return NoContent();
        }
    }
}

