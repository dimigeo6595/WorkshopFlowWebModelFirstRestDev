using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkshopFlow.DTO;
using WorkshopFlow.Services;

namespace WorkshopFlow.Controllers
{
    [ApiController]
    [Route("api/v1/items/{producedItemId:int}/bom")]
    public class BomLinesController : ControllerBase
    {
        private readonly IApplicationService _applicationService;

        public BomLinesController(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        /// <summary>
        /// Gets the BOM for a produced item.
        /// </summary>
        /// <param name="producedItemId">The produced item ID.</param>
        /// <returns>The BOM lines for the item.</returns>
        /// <response code="200">Returns the BOM lines.</response>
        /// <response code="400">If the item is not a manufactured item.</response>
        /// <response code="401">If the request is not authenticated.</response>
        /// <response code="403">If the user lacks permission.</response>
        /// <response code="404">If no item exists with the given ID.</response>
        [HttpGet]
        [Authorize(Policy = "VIEW_BOM")]
        [ProducesResponseType(typeof(IEnumerable<BomLineReadOnlyDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<BomLineReadOnlyDTO>>> GetBom(int producedItemId)
        {
            var bomLines = await _applicationService.BomLineService
                .GetBomByItemIdAsync(producedItemId);
            return Ok(bomLines);
        }

        /// <summary>
        /// Adds a component to the BOM of a produced item.
        /// </summary>
        /// <param name="producedItemId">The produced item ID.</param>
        /// <param name="dto">The BOM line to add.</param>
        /// <returns>The created BOM line.</returns>
        /// <response code="201">Returns the created BOM line.</response>
        /// <response code="400">If the request is invalid.</response>
        /// <response code="404">If no item exists with the given ID.</response>
        /// <response code="409">If the component already exists in the BOM.</response>
        [HttpPost]
        [Authorize(Policy = "EDIT_BOM")]
        [ProducesResponseType(typeof(BomLineReadOnlyDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<BomLineReadOnlyDTO>> AddBomLine(
            int producedItemId, [FromBody] BomLineInsertDTO dto)
        {
            var createdBomLine = await _applicationService.BomLineService
                .InsertBomLineAsync(producedItemId, dto);

            return CreatedAtAction(
                actionName: nameof(GetBom),
                routeValues: new { producedItemId },
                value: createdBomLine);
        }

        /// <summary>
        /// Updates a BOM line (quantity and notes only).
        /// </summary>
        /// <param name="producedItemId">The produced item ID.</param>
        /// <param name="bomLineId">The BOM line ID.</param>
        /// <param name="dto">The updated BOM line data.</param>
        /// <returns>The updated BOM line.</returns>
        /// <response code="200">Returns the updated BOM line.</response>
        /// <response code="400">If the request is invalid.</response>
        /// <response code="404">If no BOM line exists with the given ID.</response>
        [HttpPut("{bomLineId:int}")]
        [Authorize(Policy = "EDIT_BOM")]
        [ProducesResponseType(typeof(BomLineReadOnlyDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BomLineReadOnlyDTO>> UpdateBomLine(
            int producedItemId, int bomLineId, [FromBody] BomLineUpdateDTO dto)
        {
            var updatedBomLine = await _applicationService.BomLineService
                .UpdateBomLineAsync(producedItemId, bomLineId, dto);
            return Ok(updatedBomLine);
        }

        /// <summary>
        /// Removes a component from the BOM.
        /// </summary>
        /// <param name="producedItemId">The produced item ID.</param>
        /// <param name="bomLineId">The BOM line ID.</param>
        /// <response code="204">Delete successful.</response>
        /// <response code="404">If no BOM line exists with the given ID.</response>
        [HttpDelete("{bomLineId:int}")]
        [Authorize(Policy = "EDIT_BOM")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteBomLine(int producedItemId, int bomLineId)
        {
            await _applicationService.BomLineService
                .DeleteBomLineAsync(producedItemId, bomLineId);
            return NoContent();
        }
    }
}
