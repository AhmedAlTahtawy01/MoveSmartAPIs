using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Services;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using DataAccessLayer.Repositories;
using System.ComponentModel.DataAnnotations;
using BusinessLogicLayer.Services;
using Microsoft.AspNetCore.Authorization;

namespace Move_Smart.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MissionController : ControllerBase
    {
        private readonly MissionService _service;
        private readonly ILogger<MissionController> _logger;

        public MissionController(MissionService service, ILogger<MissionController> logger)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service), "Service cannot be null.");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger cannot be null.");
        }

        [Authorize(Policy = "GeneralSupervisor")]
        [HttpPost]
        public async Task<IActionResult> CreateMission([FromBody] MissionDTO dto)
        {
            if (dto == null)
            {
                _logger.LogWarning("Received null DTO.");
                return BadRequest(new { message = "Mission data cannot be null." });
            }

            try
            {
                var missionId = await _service.CreateMissionAsync(dto.MissionNoteId, dto);
                return CreatedAtAction(nameof(GetMissionById), new { missionId }, dto);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Invalid argument provided.");
                return BadRequest(new { message = "Invalid argument provided." });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError($"Invalid Data {ex.Message}");
                return Conflict(new { message = $"Invalid Data {ex.Message}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating mission.");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [Authorize(Policy = "RequireGeneralSupervisor")]
        [HttpGet]
        public async Task<IActionResult> GetAllMissions([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                _logger.LogWarning("Invalid pagination parameters.");
                return BadRequest(new { message = "Page number and size must be greater than 0." });
            }

            try
            {
                var missions = await _service.GetAllMissionsAsync(pageNumber, pageSize);
                return Ok(missions);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Invalid argument provided.");
                return BadRequest(new { message = "Invalid argument provided." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving missions.");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [Authorize(Policy = "RequireGeneralSupervisor")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMissionById([FromRoute] int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid mission ID.");
                return BadRequest(new { message = "Mission ID must be greater than 0." });
            }

            try
            {
                var mission = await _service.GetMissionByIdAsync(id);
                return Ok(mission);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Invalid argument provided.");
                return BadRequest(new { message = "Invalid argument provided." });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, $"No mission found with ID {id}.");
                return NotFound(new { message = $"No mission found with ID {id}." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving mission.");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [Authorize(Policy = "RequireGeneralSupervisor")]
        [HttpGet("missionNote/{id}")]
        public async Task<IActionResult> GetMissionsByMissionNoteId([FromRoute] int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid mission note ID.");
                return BadRequest(new { message = "Mission note ID must be greater than 0." });
            }
            try
            {
                var missions = await _service.GetMissionsByNoteIdAsync(id);
                return Ok(missions);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Invalid argument provided.");
                return BadRequest(new { message = "Invalid argument provided." });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, $"No mission found with ID {id}.");
                return NotFound(new { message = $"No mission found with ID {id}." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving missions.");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [Authorize(Policy = "RequireGeneralSupervisor")]
        [HttpGet("vehicle/{id}")]
        public async Task<IActionResult> GetMissionsByVehicleId([FromRoute] int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid vehicle ID.");
                return BadRequest(new { message = "Vehicle ID must be greater than 0." });
            }
            try
            {
                var missions = await _service.GetMissionsByVehicleIdAsync(id);
                return Ok(missions);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Invalid argument provided.");
                return BadRequest(new { message = "Invalid argument provided." });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, $"No mission found with ID {id}.");
                return NotFound(new { message = $"No mission found with ID {id}." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving missions.");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [Authorize(Policy = "RequireGeneralSupervisor")]
        [HttpGet("startDate/{startDate}")]
        public async Task<IActionResult> GetMissionsByStartDate([FromRoute] DateTime startDate)
        {
            if (startDate == default)
            {
                _logger.LogWarning("Invalid start date.");
                return BadRequest(new { message = "Start date must be a valid date." });
            }

            try
            {
                var missions = await _service.GetMissionsByStartDateAsync(startDate);
                return Ok(missions);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Invalid argument provided.");
                return BadRequest(new { message = "Invalid argument provided." });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, $"No mission found with start date {startDate}.");
                return NotFound(new { message = $"No mission found with start date {startDate}." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving missions.");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [Authorize(Policy = "RequireGeneralSupervisor")]
        [HttpGet("destination/{destination}")]
        public async Task<IActionResult> GetMissionsByDestination([FromRoute] string destination)
        {
            if (string.IsNullOrWhiteSpace(destination))
            {
                _logger.LogWarning("Invalid destination.");
                return BadRequest(new { message = "Destination cannot be null or empty." });
            }

            try
            {
                var missions = await _service.GetMissionsByDestinationAsync(destination);
                return Ok(missions);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Invalid argument provided.");
                return BadRequest(new { message = "Invalid argument provided." });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, $"No mission found with destination {destination}.");
                return NotFound(new { message = $"No mission found with destination {destination}." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving missions.");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [Authorize(Policy = "GeneralSupervisor")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMission([FromRoute] int id, [FromBody] MissionDTO dto)
        {
            if (dto == null)
            {
                _logger.LogWarning("Received null DTO.");
                return BadRequest(new { message = "Mission data cannot be null." });
            }
            if (id <= 0)
            {
                _logger.LogWarning("Invalid mission ID.");
                return BadRequest(new { message = "Mission ID must be greater than 0." });
            }
            if (dto.MissionId != id)
            {
                _logger.LogWarning("Mission ID in DTO does not match route ID.");
                return BadRequest(new { message = "Mission ID in DTO must match route ID." });
            }

            try
            {
                var updated = await _service.UpdateMissionAsync(dto);
                return Ok(updated);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Invalid argument provided.");
                return BadRequest(new { message = "Invalid argument provided." });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, $"No mission found with ID {id}.");
                return NotFound(new { message = $"No mission found with ID {id}." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating mission.");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [Authorize(Policy = "GeneralSupervisor")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMission([FromRoute] int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid mission ID.");
                return BadRequest(new { message = "Mission ID must be greater than 0." });
            }

            try
            {
                await _service.DeleteMissionAsync(id);
                return Ok(new { message = $"Mission with ID {id} deleted successfully." });
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Invalid argument provided.");
                return BadRequest(new { message = "Invalid argument provided." });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, $"No mission found with ID {id}.");
                return NotFound(new { message = $"No mission found with ID {id}." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting mission.");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
    }
}
