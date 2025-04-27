using BusinessLogicLayer.Services;
using DataAccessLayer;
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


        [HttpGet("AllForEmployee/{employeeID}", Name = "GetPatrolsSubscriptionForEmployee")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<PatrolsSubscriptionDTO>>> GetPatrolsSubscriptionForEmployee(int employeeID)
        {
            List<PatrolsSubscriptionDTO> patrolsSubscriptions = await _service.GetAllPatrolsSubscriptionsForEmployeeAsync(employeeID);

            if (patrolsSubscriptions == null || !patrolsSubscriptions.Any())
            {
                return NotFound($"No patrols subscriptions found for employee with ID [{employeeID}].");
            }

            return Ok(patrolsSubscriptions);
        }


        [HttpGet("AllForPatrol/{patrolID}", Name = "GetPatrolsSubscriptionForPatrol")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<PatrolsSubscriptionDTO>>> GetPatrolsSubscriptionForPatrol(short patrolID)
        {
            List<PatrolsSubscriptionDTO> patrolsSubscriptions = await _service.GetAllPatrolsSubscriptionsForPatrolAsync(patrolID);
         
            if (patrolsSubscriptions == null || !patrolsSubscriptions.Any())
            {
                return NotFound($"No patrols subscriptions found for patrol with ID [{patrolID}].");
            }
            
            return Ok(patrolsSubscriptions);
        }


        [HttpGet("{subscriptionID}", Name = "GetPatrolSubscriptionByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PatrolsSubscriptionDTO>> GetPatrolSubscriptionByID(int subscriptionID)
        {
            if(subscriptionID <= 0)
            {
                return BadRequest($"Invalid ID [{subscriptionID}]");
            }

            PatrolsSubscriptionDTO? patrolsSubscription = await _service.GetPatrolSubscriptionByIDAsync(subscriptionID);
         
            if (patrolsSubscription == null)
            {
                return NotFound($"No patrol subscription found with ID [{subscriptionID}].");
            }
            
            return Ok(patrolsSubscription);
        }


        [HttpPost(Name = "AddNewPatrolSubscription")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PatrolsSubscriptionDTO>> AddNewPatrolSubscription(PatrolsSubscriptionDTO dto)
        {
            if (dto == null)
            {
                return BadRequest("PatrolsSubscriptionDTO cannot be null.");
            }

            if (await _service.AddNewPatrolSubscriptionAsync(dto) == null)
            {
                return BadRequest("Failed to create new patrol subscription.");
            }

            return CreatedAtRoute("GetPatrolSubscriptionByID", new { subscriptionID = dto.SubscriptionID }, dto);
        }


        [HttpPut(Name = "UpdatePatrolSubscription")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdatePatrolSubscription(PatrolsSubscriptionDTO dto)
        {
            if (dto == null)
            {
                return BadRequest("PatrolsSubscriptionDTO cannot be null.");
            }

            if (!await _service.IsPatrolSubscriptionExists(dto.SubscriptionID ?? 0))
            {
                return NotFound($"No patrol subscription found with ID [{dto.SubscriptionID}].");
            }

            if (!await _service.UpdatePatrolSubscriptionAsync(dto))
            {
                return BadRequest($"Failed to update patrol subscription with ID [{dto.SubscriptionID}]!");
            }
         
            return Ok($"Patrol subscription with ID [{dto.SubscriptionID}] updated successfully.");
        }


        [HttpDelete("{subscriptionID}", Name = "DeletePatrolSubscription")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> DeletePatrolSubscription(int subscriptionID)
        {
            if (subscriptionID <= 0)
            {
                return BadRequest($"Invalid ID [{subscriptionID}]");
            }
            
            if (!await _service.IsPatrolSubscriptionExists(subscriptionID))
            {
                return NotFound($"No patrol subscription found with ID [{subscriptionID}].");
            }
            
            if (!await _service.DeletePatrolSubscriptionAsync(subscriptionID))
            {
                return BadRequest($"Failed to delete patrol subscription with ID [{subscriptionID}]!");
            }

            return Ok($"Patrol subscription with ID [{subscriptionID}] deleted successfully.");
        }
    }
}
