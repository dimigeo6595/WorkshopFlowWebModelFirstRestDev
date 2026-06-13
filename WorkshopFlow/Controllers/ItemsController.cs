using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkshopFlow.Core;
using WorkshopFlow.Core.Filters;
using WorkshopFlow.DTO;
using WorkshopFlow.Services;

namespace WorkshopFlow.Controllers
{
    [ApiController]
    [Route("api/v1/items")]
    public class ItemsController : ControllerBase
    {
        private readonly IApplicationService _applicationService;

        public ItemsController(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        /// <summary>
        /// Gets an item by its ID.
        /// </summary>
        /// <param name="id">The item ID.</param>
        /// <returns>The item details.</returns>
        /// <response code="200">Returns the requested item.</response>
        /// <response code="401">If the request is not authenticated.</response>
        /// <response code="403">If the user lacks permission.</response>
        /// <response code="404">If no item exists with the given ID.</response>
        [HttpGet("{id:int}")]
        [Authorize(Policy = "VIEW_ITEMS")]
        [ProducesResponseType(typeof(ItemReadOnlyDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ItemReadOnlyDTO>> GetItemById(int id)
        {
            var item = await _applicationService.ItemService.GetItemByIdAsync(id);
            return Ok(item);
        }

        /// <summary>
        /// Gets an item by its code.
        /// </summary>
        /// <param name="itemCode">The item code.</param>
        /// <returns>The item details.</returns>
        /// <response code="200">Returns the requested item.</response>
        /// <response code="401">If the request is not authenticated.</response>
        /// <response code="404">If no item exists with the given code.</response>
        [HttpGet("by-code/{itemCode}")]
        [Authorize(Policy = "VIEW_ITEMS")]
        [ProducesResponseType(typeof(ItemReadOnlyDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ItemReadOnlyDTO>> GetItemByCode(string itemCode)
        {
            var item = await _applicationService.ItemService.GetItemByCodeAsync(itemCode);
            return Ok(item);
        }

        /// <summary>
        /// Gets a paginated list of items with optional filtering.
        /// </summary>
        /// <param name="pageNumber">The page number (1-based). Default is 1.</param>
        /// <param name="pageSize">The number of items per page. Default is 10.</param>
        /// <param name="filters">Optional filters for name and item type.</param>
        /// <returns>A paginated list of items.</returns>
        /// <response code="200">Returns the paginated item list.</response>
        /// <response code="401">If the request is not authenticated.</response>
        /// <response code="403">If the user lacks permission.</response>
        [HttpGet]
        [Authorize(Policy = "VIEW_ITEMS")]
        [ProducesResponseType(typeof(PaginatedResult<ItemReadOnlyDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<PaginatedResult<ItemReadOnlyDTO>>> GetItems(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] ItemFiltersDTO? filters = null)
        {
            var result = await _applicationService.ItemService
                .GetPaginatedItemsFilteredAsync(pageNumber, pageSize, filters ?? new ItemFiltersDTO());
            return Ok(result);
        }

        /// <summary>
        /// Creates a new item.
        /// </summary>
        /// <response code="201">Returns the created item.</response>
        /// <response code="400">If the request is invalid.</response>
        /// <response code="409">If an item with the same code already exists.</response>
        [HttpPost]
        [Authorize(Policy = "INSERT_ITEM")]
        [ProducesResponseType(typeof(ItemReadOnlyDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ItemReadOnlyDTO>> CreateItem([FromBody] ItemInsertDTO dto)
        {
            var createdItem = await _applicationService.ItemService.InsertItemAsync(dto);

            return CreatedAtAction(
                actionName: nameof(GetItemById),
                routeValues: new { id = createdItem.Id },
                value: createdItem);
        }

        /// <summary>
        /// Updates an item.
        /// </summary>
        /// <response code="200">Returns the updated item.</response>
        /// <response code="404">If no item exists with the given ID.</response>
        [HttpPut("{id:int}")]
        [Authorize(Policy = "EDIT_ITEM")]
        [ProducesResponseType(typeof(ItemReadOnlyDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ItemReadOnlyDTO>> UpdateItem(int id, [FromBody] ItemUpdateDTO dto)
        {
            var updatedItem = await _applicationService.ItemService.UpdateItemAsync(id, dto);
            return Ok(updatedItem);
        }

        /// <summary>
        /// Calculates the weight of a manufactured item from its BOM.
        /// </summary>
        /// <response code="200">Returns the item with updated weight.</response>
        /// <response code="404">If no item exists with the given ID.</response>
        [HttpPost("{id:int}/calculate-weight")]
        [Authorize(Policy = "EDIT_ITEM")]
        [ProducesResponseType(typeof(ItemReadOnlyDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ItemReadOnlyDTO>> CalculateWeight(int id)
        {
            var item = await _applicationService.ItemService.CalculateWeightAsync(id);
            return Ok(item);
        }

        /// <summary>
        /// Soft deletes an item.
        /// </summary>
        /// <response code="204">Delete successful.</response>
        /// <response code="404">If no item exists with the given ID.</response>
        [HttpDelete("{id:int}")]
        [Authorize(Policy = "DELETE_ITEM")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteItem(int id)
        {
            await _applicationService.ItemService.DeleteItemAsync(id);
            return NoContent();
        }
    }
}