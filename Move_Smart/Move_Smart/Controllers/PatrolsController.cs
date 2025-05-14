using BusinessLayer;
using DataAccessLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Move_Smart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatrolsController : ControllerBase
    {
        private readonly ILogger<PatrolsController> _logger;
        private readonly PatrolService _service;

        public PatrolsController(PatrolService service, ILogger<PatrolsController> logger)
        {
            _service = service;
            _logger = logger;
        }


        [Authorize(Policy = "RequirePatrolsSupervisor")]
        [HttpGet("All", Name = "GetAllPatrols")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<PatrolDTO>>> GetAllPatrols()
        {
            List<PatrolDTO> patrols = await _service.GetAllPatrolsAsync();

            if (patrols == null || !patrols.Any())
            {
                return NotFound("No patrols found!");
            }

            return Ok(patrols);
        }


        [Authorize(Policy = "RequirePatrolsSupervisor")]
        [HttpGet("{patrolID}", Name = "GetPatrolByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PatrolDTO>> GetPatrolByID(short patrolID)
        {
            if(patrolID <= 0)
            {
                return BadRequest($"Invalid ID [{patrolID}]");
            }

            PatrolDTO? patrol = await _service.GetPatrolByIDAsync(patrolID);

            if (patrol == null)
            {
                return NotFound($"Patrol with ID [{patrolID}] not found!");
            }

            return Ok(patrol);
        }


        [Authorize(Policy = "RequireGeneralSupervisor")]
        [HttpPost(Name = "AddNewPatrol")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PatrolDTO>> AddNewPatrol(PatrolDTO dto)
        {
            if (dto == null)
            {
                return BadRequest("PatrolDTO cannot be null!");
            }

            if(await _service.IsPatrolExistsAsync(dto.PatrolID ?? 0))
            {
                return BadRequest($"Patrol with ID [{dto.PatrolID}] already exists!");
            }

            if(await _service.AddNewPatrolAsync(dto) == null)
            {
                return BadRequest("Failed to add new patrol!");
            }

            return CreatedAtRoute("GetPatrolByID", new { patrolID = dto.PatrolID }, dto);
        }


        [Authorize(Policy = "RequireGeneralSupervisor")]
        [HttpPut(Name = "UpdatePatrol")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdatePatrol(PatrolDTO dto)
        {
            if (dto == null)
            {
                return BadRequest("PatrolDTO cannot be null!");
            }

            if(!await _service.IsPatrolExistsAsync(dto.PatrolID ?? 0))
            {
                return NotFound($"No patrols found with ID [{dto.PatrolID}]!");
            }

            if(!await _service.UpdatePatrolAsync(dto))
            {
                return BadRequest($"Failed to update patrol with ID [{dto.PatrolID}]!");
            }

            return Ok($"Patrol with ID [{dto.PatrolID}] updated successfully");
        }


        [Authorize(Policy = "RequireGeneralSupervisor")]
        [HttpDelete("{patrolID}", Name = "DeletePatrol")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> DeletePatrol(short patrolID)
        {
            if(patrolID <= 0)
            {
                return BadRequest($"Invalid ID [{patrolID}]!");
            }

            if(!await _service.IsPatrolExistsAsync(patrolID))
            {
                return NotFound($"No patrol found with ID [{patrolID}]!");
            }

            if(!await _service.DeletePatrolAsync(patrolID))
            {
                return BadRequest($"Failed to delete patrol with ID [{patrolID}]!");
            }

            return Ok($"Patrol with ID [{patrolID}] deleted successfully");
        }
    }
}
