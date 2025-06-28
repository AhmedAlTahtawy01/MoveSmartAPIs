using BusinessLogicLayer.Services;
using DataAccessLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Move_Smart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatrolsSubscriptionsController : ControllerBase
    {
        private readonly PatrolsSubscriptionService _service;
        private readonly ILogger<PatrolsSubscriptionsController> _logger;

        public PatrolsSubscriptionsController(PatrolsSubscriptionService service, ILogger<PatrolsSubscriptionsController> logger)
        {
            _service = service;
            _logger = logger;
        }


        [Authorize(Policy = "RequirePatrolsSupervisor")]
        [HttpGet("AllForEmployee/{employeeID}", Name = "GetPatrolsSubscriptionForEmployee")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<PatrolsSubscriptionDTO>>> GetPatrolsSubscriptionForEmployee(int employeeID)
        {
            List<PatrolsSubscriptionDTO> patrolsSubscriptions = await _service.GetAllPatrolsSubscriptionsForEmployeeAsync(employeeID);

            if (patrolsSubscriptions == null || !patrolsSubscriptions.Any())
            {
                return NotFound(new { message = $"No patrols subscriptions found for employee with ID [{employeeID}]." });
            }

            return Ok(patrolsSubscriptions);
        }


        [Authorize(Policy = "RequirePatrolsSupervisor")]
        [HttpGet("AllForPatrol/{patrolID}", Name = "GetPatrolsSubscriptionForPatrol")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<PatrolsSubscriptionDTO>>> GetPatrolsSubscriptionForPatrol(short patrolID)
        {
            List<PatrolsSubscriptionDTO> patrolsSubscriptions = await _service.GetAllPatrolsSubscriptionsForPatrolAsync(patrolID);
         
            if (patrolsSubscriptions == null || !patrolsSubscriptions.Any())
            {
                return NotFound(new { message = $"No patrols subscriptions found for patrol with ID [{patrolID}]." });
            }
            
            return Ok(patrolsSubscriptions);
        }


        [Authorize(Policy = "RequirePatrolsSupervisor")]
        [HttpGet("{subscriptionID}", Name = "GetPatrolSubscriptionByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PatrolsSubscriptionDTO>> GetPatrolSubscriptionByID(int subscriptionID)
        {
            if(subscriptionID <= 0)
            {
                return BadRequest(new { message = $"Invalid ID [{subscriptionID}]" });
            }

            PatrolsSubscriptionDTO? patrolsSubscription = await _service.GetPatrolSubscriptionByIDAsync(subscriptionID);
         
            if (patrolsSubscription == null)
            {
                return NotFound(new { message = $"No patrol subscription found with ID [{subscriptionID}]." });
            }
            
            return Ok(patrolsSubscription);
        }


        [Authorize(Policy = "RequirePatrolsSupervisor")]
        [HttpPost(Name = "AddNewPatrolSubscription")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PatrolsSubscriptionDTO>> AddNewPatrolSubscription(PatrolsSubscriptionDTO dto)
        {
            if (dto == null)
            {
                return BadRequest(new { message = "PatrolsSubscriptionDTO cannot be null." });
            }

            if (await _service.AddNewPatrolSubscriptionAsync(dto) == null)
            {
                return BadRequest(new { message = "Failed to create new patrol subscription." });
            }

            return CreatedAtRoute("GetPatrolSubscriptionByID", new { subscriptionID = dto.SubscriptionID }, dto);
        }


        [Authorize(Policy = "RequirePatrolsSupervisor")]
        [HttpPut(Name = "UpdatePatrolSubscription")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdatePatrolSubscription(PatrolsSubscriptionDTO dto)
        {
            if (dto == null)
            {
                return BadRequest(new { message = "PatrolsSubscriptionDTO cannot be null." });
            }

            if (!await _service.IsPatrolSubscriptionExists(dto.SubscriptionID ?? 0))
            {
                return NotFound(new { message = $"No patrol subscription found with ID [{dto.SubscriptionID}]." });
            }

            if (!await _service.UpdatePatrolSubscriptionAsync(dto))
            {
                return BadRequest(new { message = $"Failed to update patrol subscription with ID [{dto.SubscriptionID}]!" });
            }
         
            return Ok(new { message = $"Patrol subscription with ID [{dto.SubscriptionID}] updated successfully." });
        }


        [Authorize(Policy = "RequirePatrolsSupervisor")]
        [HttpDelete("{subscriptionID}", Name = "DeletePatrolSubscription")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> DeletePatrolSubscription(int subscriptionID)
        {
            if (subscriptionID <= 0)
            {
                return BadRequest(new { message = $"Invalid ID [{subscriptionID}]" });
            }
            
            if (!await _service.IsPatrolSubscriptionExists(subscriptionID))
            {
                return NotFound(new { message = $"No patrol subscription found with ID [{subscriptionID}]." });
            }
            
            if (!await _service.DeletePatrolSubscriptionAsync(subscriptionID))
            {
                return BadRequest(new { message = $"Failed to delete patrol subscription with ID [{subscriptionID}]!" });
            }

            return Ok(new { message = $"Patrol subscription with ID [{subscriptionID}] deleted successfully." });
        }

        [Authorize(Policy = "RequirePatrolsSupervisor")]
        [HttpGet("NumberOfPatrolsSubscriptions", Name = "GetNumberOfPatrolsSubscriptions")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> GetNumberOfPatrolsSubscriptions()
        {
            int count = await _service.GetNumberOfPartolsSubscriptionsAsync();
            return Ok(count);
        }
    }
}
