using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkshopFlow.Data;
using WorkshopFlow.DTO;

namespace WorkshopFlow.Controllers
{
    [ApiController]
    [Route("api/v1/roles")]
    public class RolesController : ControllerBase
    {
        private readonly WorkshopFlowContext _context;

        public RolesController(WorkshopFlowContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all roles for dropdowns.
        /// </summary>
        /// <response code="200">Returns all roles.</response>
        /// <response code="401">If the request is not authenticated.</response>
        [HttpGet]
        [Authorize(Policy = "VIEW_USERS")]
        [ProducesResponseType(typeof(IEnumerable<RoleReadOnlyDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<RoleReadOnlyDTO>>> GetRoles()
        {
            var roles = await _context.Roles
                .OrderBy(r => r.Name)
                .Select(r => new RoleReadOnlyDTO
                {
                    Id = r.Id,
                    Name = r.Name
                })
                .ToListAsync();

            return Ok(roles);
        }
    }
}

