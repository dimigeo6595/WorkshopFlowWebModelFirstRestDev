using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WorkshopFlow.Core;
using WorkshopFlow.Core.Filters;
using WorkshopFlow.DTO;
using WorkshopFlow.Services;

namespace WorkshopFlow.Controllers
{
    [ApiController]
    [Route("api/v1/workorders")]
    public class WorkOrdersController : ControllerBase
    {
        private readonly IApplicationService _applicationService;

        public WorkOrdersController(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        /// <summary>
        /// Gets a paginated list of work orders with optional filtering.
        /// </summary>
        /// <response code="200">Returns the paginated work order list.</response>
        /// <response code="401">If the request is not authenticated.</response>
        /// <response code="403">If the user lacks permission.</response>
        [HttpGet]
        [Authorize(Policy = "VIEW_WORK_ORDERS")]
        [ProducesResponseType(typeof(PaginatedResult<WorkOrderReadOnlyDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<PaginatedResult<WorkOrderReadOnlyDTO>>> GetWorkOrders(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] WorkOrderFiltersDTO? filters = null)
        {
            var result = await _applicationService.WorkOrderService
                .GetPaginatedWorkOrdersAsync(pageNumber, pageSize, filters ?? new WorkOrderFiltersDTO());
            return Ok(result);
        }

        /// <summary>
        /// Gets a work order by its ID.
        /// </summary>
        /// <response code="200">Returns the work order.</response>
        /// <response code="401">If the request is not authenticated.</response>
        /// <response code="403">If the user lacks permission.</response>
        /// <response code="404">If no work order exists with the given ID.</response>
        [HttpGet("{id:int}")]
        [Authorize(Policy = "VIEW_WORK_ORDERS")]
        [ProducesResponseType(typeof(WorkOrderReadOnlyDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<WorkOrderReadOnlyDTO>> GetWorkOrderById(int id)
        {
            var workOrder = await _applicationService.WorkOrderService.GetWorkOrderByIdAsync(id);
            return Ok(workOrder);
        }

        /// <summary>
        /// Creates a new work order in Draft status.
        /// </summary>
        /// <response code="201">Returns the created work order.</response>
        /// <response code="400">If the request is invalid.</response>
        /// <response code="404">If the item does not exist.</response>
        [HttpPost]
        [Authorize(Policy = "INSERT_WORK_ORDER")]
        [ProducesResponseType(typeof(WorkOrderReadOnlyDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<WorkOrderReadOnlyDTO>> CreateWorkOrder(
            [FromBody] WorkOrderInsertDTO dto)
        {
            // Παίρνουμε το id του logged-in user από το JWT token
            var createdByUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var createdWorkOrder = await _applicationService.WorkOrderService
                .InsertWorkOrderAsync(dto, createdByUserId);

            return CreatedAtAction(
                actionName: nameof(GetWorkOrderById),
                routeValues: new { id = createdWorkOrder.Id },
                value: createdWorkOrder);
        }

        /// <summary>
        /// Updates a work order. Only Draft work orders can be updated.
        /// </summary>
        /// <response code="200">Returns the updated work order.</response>
        /// <response code="400">If the request is invalid or work order is not in Draft status.</response>
        /// <response code="404">If no work order exists with the given ID.</response>
        [HttpPut("{id:int}")]
        [Authorize(Policy = "EDIT_WORK_ORDER")]
        [ProducesResponseType(typeof(WorkOrderReadOnlyDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<WorkOrderReadOnlyDTO>> UpdateWorkOrder(
            int id, [FromBody] WorkOrderUpdateDTO dto)
        {
            var updatedWorkOrder = await _applicationService.WorkOrderService
                .UpdateWorkOrderAsync(id, dto);
            return Ok(updatedWorkOrder);
        }

        /// <summary>
        /// Deletes a work order. Only Draft work orders can be deleted.
        /// </summary>
        /// <response code="204">Delete successful.</response>
        /// <response code="400">If the work order is not in Draft status.</response>
        /// <response code="404">If no work order exists with the given ID.</response>
        [HttpDelete("{id:int}")]
        [Authorize(Policy = "EDIT_WORK_ORDER")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteWorkOrder(int id)
        {
            await _applicationService.WorkOrderService.DeleteWorkOrderAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Releases a work order. Checks stock availability and creates operations.
        /// </summary>
        /// <response code="200">Returns the released work order.</response>
        /// <response code="400">If the work order is not in Draft status.</response>
        /// <response code="404">If no work order exists with the given ID.</response>
        /// <response code="409">If there is insufficient stock for components.</response>
        [HttpPost("{id:int}/release")]
        [Authorize(Policy = "EDIT_WORK_ORDER")]
        [ProducesResponseType(typeof(WorkOrderReadOnlyDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<WorkOrderReadOnlyDTO>> ReleaseWorkOrder(int id)
        {
            var workOrder = await _applicationService.WorkOrderService.ReleaseWorkOrderAsync(id);
            return Ok(workOrder);
        }

        /// <summary>
        /// Cancels a work order. Returns consumed stock if Released or InProgress.
        /// </summary>
        /// <response code="204">Cancel successful.</response>
        /// <response code="400">If the work order is Completed.</response>
        /// <response code="404">If no work order exists with the given ID.</response>
        [HttpPost("{id:int}/cancel")]
        [Authorize(Policy = "EDIT_WORK_ORDER")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CancelWorkOrder(int id)
        {
            await _applicationService.WorkOrderService.CancelWorkOrderAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Gets all operations for a work order.
        /// </summary>
        /// <response code="200">Returns the operations.</response>
        /// <response code="401">If the request is not authenticated.</response>
        /// <response code="404">If no work order exists with the given ID.</response>
        [HttpGet("{id:int}/operations")]
        [Authorize(Policy = "VIEW_WORK_ORDERS")]
        [ProducesResponseType(typeof(IEnumerable<WorkOrderOperationReadOnlyDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<WorkOrderOperationReadOnlyDTO>>> GetOperations(int id)
        {
            var operations = await _applicationService.WorkOrderService.GetOperationsAsync(id);
            return Ok(operations);
        }

        /// <summary>
        /// Assigns an operator to a work order operation.
        /// </summary>
        /// <response code="200">Returns the updated operation.</response>
        /// <response code="400">If the operation is not in Pending status.</response>
        /// <response code="404">If no operation exists with the given ID.</response>
        [HttpPatch("{id:int}/operations/{operationId:int}/assign")]
        [Authorize(Policy = "ASSIGN_WORK_ORDER")]
        [ProducesResponseType(typeof(WorkOrderOperationReadOnlyDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<WorkOrderOperationReadOnlyDTO>> AssignOperation(
            int id, int operationId, [FromBody] WorkOrderOperationAssignDTO dto)
        {
            var operation = await _applicationService.WorkOrderService
                .AssignOperationAsync(id, operationId, dto);
            return Ok(operation);
        }

        /// <summary>
        /// Starts a work order operation.
        /// </summary>
        /// <response code="200">Returns the updated operation.</response>
        /// <response code="400">If the operation cannot be started.</response>
        /// <response code="404">If no operation exists with the given ID.</response>
        [HttpPatch("{id:int}/operations/{operationId:int}/start")]
        [Authorize(Policy = "START_WORK_ORDER")]
        [ProducesResponseType(typeof(WorkOrderOperationReadOnlyDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<WorkOrderOperationReadOnlyDTO>> StartOperation(
            int id, int operationId)
        {
            var operation = await _applicationService.WorkOrderService
                .StartOperationAsync(id, operationId);
            return Ok(operation);
        }

        /// <summary>
        /// Completes a work order operation.
        /// </summary>
        /// <response code="200">Returns the updated operation.</response>
        /// <response code="400">If the operation is not in InProgress status.</response>
        /// <response code="404">If no operation exists with the given ID.</response>
        [HttpPatch("{id:int}/operations/{operationId:int}/complete")]
        [Authorize(Policy = "COMPLETE_WORK_ORDER")]
        [ProducesResponseType(typeof(WorkOrderOperationReadOnlyDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<WorkOrderOperationReadOnlyDTO>> CompleteOperation(
            int id, int operationId)
        {
            var operation = await _applicationService.WorkOrderService
                .CompleteOperationAsync(id, operationId);
            return Ok(operation);
        }
    }
}