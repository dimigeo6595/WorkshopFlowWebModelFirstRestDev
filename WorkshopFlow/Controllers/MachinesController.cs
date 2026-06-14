using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkshopFlow.DTO;
using WorkshopFlow.Services;

namespace WorkshopFlow.Controllers
{
    [ApiController]
    [Route("api/v1/workstations/{workstationId:int}/machines")]
    public class MachinesController : ControllerBase
    {
        private readonly IApplicationService _applicationService;

        public MachinesController(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        /// <summary>
        /// Gets all machines for a workstation.
        /// </summary>
        /// <response code="200">Returns all machines.</response>
        /// <response code="401">If the request is not authenticated.</response>
        /// <response code="403">If the user lacks permission.</response>
        /// <response code="404">If no workstation exists with the given ID.</response>
        [HttpGet]
        [Authorize(Policy = "VIEW_MACHINES")]
        [ProducesResponseType(typeof(IEnumerable<MachineReadOnlyDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<MachineReadOnlyDTO>>> GetMachines(int workstationId)
        {
            var machines = await _applicationService.MachineService
                .GetMachinesByWorkstationAsync(workstationId);
            return Ok(machines);
        }

        /// <summary>
        /// Gets a machine by its ID.
        /// </summary>
        /// <response code="200">Returns the machine.</response>
        /// <response code="401">If the request is not authenticated.</response>
        /// <response code="404">If no machine exists with the given ID.</response>
        [HttpGet("{id:int}")]
        [Authorize(Policy = "VIEW_MACHINES")]
        [ProducesResponseType(typeof(MachineReadOnlyDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MachineReadOnlyDTO>> GetMachineById(int workstationId, int id)
        {
            var machine = await _applicationService.MachineService.GetMachineByIdAsync(id);
            return Ok(machine);
        }

        /// <summary>
        /// Creates a new machine in a workstation.
        /// </summary>
        /// <response code="201">Returns the created machine.</response>
        /// <response code="400">If the request is invalid.</response>
        /// <response code="404">If no workstation exists with the given ID.</response>
        /// <response code="409">If a machine with the same code already exists.</response>
        [HttpPost]
        [Authorize(Policy = "EDIT_MACHINES")]
        [ProducesResponseType(typeof(MachineReadOnlyDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<MachineReadOnlyDTO>> CreateMachine(
            int workstationId, [FromBody] MachineInsertDTO dto)
        {
            var createdMachine = await _applicationService.MachineService
                .InsertMachineAsync(workstationId, dto);

            return CreatedAtAction(
                actionName: nameof(GetMachineById),
                routeValues: new { workstationId, id = createdMachine.Id },
                value: createdMachine);
        }

        /// <summary>
        /// Updates a machine.
        /// </summary>
        /// <response code="200">Returns the updated machine.</response>
        /// <response code="400">If the request is invalid.</response>
        /// <response code="404">If no machine exists with the given ID.</response>
        [HttpPut("{id:int}")]
        [Authorize(Policy = "EDIT_MACHINES")]
        [ProducesResponseType(typeof(MachineReadOnlyDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MachineReadOnlyDTO>> UpdateMachine(
            int workstationId, int id, [FromBody] MachineUpdateDTO dto)
        {
            var updatedMachine = await _applicationService.MachineService
                .UpdateMachineAsync(workstationId, id, dto);
            return Ok(updatedMachine);
        }

        /// <summary>
        /// Soft deletes a machine.
        /// </summary>
        /// <response code="204">Delete successful.</response>
        /// <response code="404">If no machine exists with the given ID.</response>
        [HttpDelete("{id:int}")]
        [Authorize(Policy = "EDIT_MACHINES")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteMachine(int workstationId, int id)
        {
            await _applicationService.MachineService.DeleteMachineAsync(workstationId, id);
            return NoContent();
        }
    }
}

