using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkshopFlow.Data;
using WorkshopFlow.DTO;

namespace WorkshopFlow.Controllers
{
    [ApiController]
    [Route("api/v1/uom")]
    public class UnitOfMeasuresController : ControllerBase
    {
        private readonly WorkshopFlowContext _context;

        public UnitOfMeasuresController(WorkshopFlowContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all units of measure for dropdowns.
        /// </summary>
        /// <response code="200">Returns all units of measure.</response>
        /// <response code="401">If the request is not authenticated.</response>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(IEnumerable<UnitOfMeasureReadOnlyDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<UnitOfMeasureReadOnlyDTO>>> GetUnitOfMeasures()
        {
            var uoms = await _context.UnitOfMeasures
                .Where(u => !u.IsDeleted)
                .OrderBy(u => u.Name)
                .Select(u => new UnitOfMeasureReadOnlyDTO
                {
                    Id = u.Id,
                    Name = u.Name,
                    Symbol = u.Symbol
                })
                .ToListAsync();

            return Ok(uoms);
        }
    }
}
