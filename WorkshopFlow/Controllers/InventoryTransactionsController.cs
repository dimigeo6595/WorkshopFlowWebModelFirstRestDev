using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WorkshopFlow.DTO;
using WorkshopFlow.Services;

namespace WorkshopFlow.Controllers
{
    [ApiController]
    [Route("api/v1/inventory")]
    public class InventoryTransactionsController : ControllerBase
    {
        private readonly IApplicationService _applicationService;

        public InventoryTransactionsController(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        /// <summary>
        /// Gets all inventory transactions for an item.
        /// </summary>
        /// <response code="200">Returns the transactions.</response>
        /// <response code="401">If the request is not authenticated.</response>
        /// <response code="403">If the user lacks permission.</response>
        /// <response code="404">If no item exists with the given ID.</response>
        [HttpGet("items/{itemId:int}")]
        [Authorize(Policy = "VIEW_INVENTORY")]
        [ProducesResponseType(typeof(IEnumerable<InventoryTransactionReadOnlyDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<InventoryTransactionReadOnlyDTO>>> GetTransactionsByItem(
            int itemId)
        {
            var transactions = await _applicationService.InventoryTransactionService
                .GetTransactionsByItemAsync(itemId);
            return Ok(transactions);
        }

        /// <summary>
        /// Gets all inventory transactions for a work order.
        /// </summary>
        /// <response code="200">Returns the transactions.</response>
        /// <response code="401">If the request is not authenticated.</response>
        /// <response code="403">If the user lacks permission.</response>
        /// <response code="404">If no work order exists with the given ID.</response>
        [HttpGet("workorders/{workOrderId:int}")]
        [Authorize(Policy = "VIEW_INVENTORY")]
        [ProducesResponseType(typeof(IEnumerable<InventoryTransactionReadOnlyDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<InventoryTransactionReadOnlyDTO>>> GetTransactionsByWorkOrder(
            int workOrderId)
        {
            var transactions = await _applicationService.InventoryTransactionService
                .GetTransactionsByWorkOrderAsync(workOrderId);
            return Ok(transactions);
        }

        /// <summary>
        /// Creates a manual inventory transaction (Purchase or Adjustment only).
        /// </summary>
        /// <response code="201">Returns the created transaction.</response>
        /// <response code="400">If the transaction type is not Purchase or Adjustment.</response>
        /// <response code="404">If no item exists with the given ID.</response>
        [HttpPost]
        [Authorize(Policy = "ADJUST_INVENTORY")]
        [ProducesResponseType(typeof(InventoryTransactionReadOnlyDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<InventoryTransactionReadOnlyDTO>> CreateManualTransaction(
            [FromBody] InventoryTransactionInsertDTO dto)
        {
            // Παίρνουμε το id του logged-in user από το JWT token
            var createdByUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var transaction = await _applicationService.InventoryTransactionService
                .InsertManualTransactionAsync(dto, createdByUserId);

            return StatusCode(StatusCodes.Status201Created, transaction);
        }
    }
}