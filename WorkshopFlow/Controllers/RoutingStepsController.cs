using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkshopFlow.DTO;
using WorkshopFlow.Services;

namespace WorkshopFlow.Controllers
{
    [ApiController]
    [Route("api/v1/items/{producedItemId:int}/routing")]
    public class RoutingStepsController : ControllerBase
    {
        private readonly IApplicationService _applicationService;

        public RoutingStepsController(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        /// <summary>
        /// Gets the routing steps for a produced item.
        /// </summary>
        /// <response code="200">Returns the routing steps.</response>
        /// <response code="400">If the item is not a manufactured item.</response>
        /// <response code="401">If the request is not authenticated.</response>
        /// <response code="403">If the user lacks permission.</response>
        /// <response code="404">If no item exists with the given ID.</response>
        [HttpGet]
        [Authorize(Policy = "VIEW_ROUTING")]
        [ProducesResponseType(typeof(IEnumerable<RoutingStepReadOnlyDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<RoutingStepReadOnlyDTO>>> GetRouting(
            int producedItemId)
        {
            var steps = await _applicationService.RoutingStepService
                .GetRoutingByItemIdAsync(producedItemId);
            return Ok(steps);
        }

        /// <summary>
        /// Adds a routing step to a produced item.
        /// </summary>
        /// <response code="201">Returns the created routing step.</response>
        /// <response code="400">If the request is invalid.</response>
        /// <response code="404">If no item exists with the given ID.</response>
        /// <response code="409">If a step with the same sequence already exists.</response>
        [HttpPost]
        [Authorize(Policy = "EDIT_ROUTING")]
        [ProducesResponseType(typeof(RoutingStepReadOnlyDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<RoutingStepReadOnlyDTO>> AddRoutingStep(
            int producedItemId, [FromBody] RoutingStepInsertDTO dto)
        {
            var createdStep = await _applicationService.RoutingStepService
                .InsertRoutingStepAsync(producedItemId, dto);

            return CreatedAtAction(
                actionName: nameof(GetRouting),
                routeValues: new { producedItemId },
                value: createdStep);
        }

        /// <summary>
        /// Updates a routing step.
        /// </summary>
        /// <response code="200">Returns the updated routing step.</response>
        /// <response code="400">If the request is invalid.</response>
        /// <response code="404">If no routing step exists with the given ID.</response>
        /// <response code="409">If a step with the same sequence already exists.</response>
        [HttpPut("{stepId:int}")]
        [Authorize(Policy = "EDIT_ROUTING")]
        [ProducesResponseType(typeof(RoutingStepReadOnlyDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<RoutingStepReadOnlyDTO>> UpdateRoutingStep(
            int producedItemId, int stepId, [FromBody] RoutingStepUpdateDTO dto)
        {
            var updatedStep = await _applicationService.RoutingStepService
                .UpdateRoutingStepAsync(producedItemId, stepId, dto);
            return Ok(updatedStep);
        }

        /// <summary>
        /// Soft deletes a routing step.
        /// </summary>
        /// <response code="204">Delete successful.</response>
        /// <response code="404">If no routing step exists with the given ID.</response>
        [HttpDelete("{stepId:int}")]
        [Authorize(Policy = "EDIT_ROUTING")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteRoutingStep(int producedItemId, int stepId)
        {
            await _applicationService.RoutingStepService
                .DeleteRoutingStepAsync(producedItemId, stepId);
            return NoContent();
        }
    }
}

