using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Services;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using DataAccessLayer.Repositories;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace Move_Smart.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]

    partial class MissionVehicleController : ControllerBase
    {
        private readonly MissionsVehicleService _service;
        private readonly ILogger<MissionVehicleController> _logger;

        public MissionVehicleController(MissionsVehicleService service, ILogger<MissionVehicleController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [Authorize(Policy = "GeneralSupervisor")]
        [HttpPost]
        public async Task<IActionResult> CreateMissionsVehicle([FromBody] MissionsVehicleDTO dto)
        {
            if (dto == null)
            {
                _logger.LogWarning("MissionsVehicleDTO cannot be null.");
                return BadRequest(new { message = "MissionsVehicleDTO cannot be null." });
            }

            try
            {
                var result = await _service.CreateMissionsVehicleAsync(dto);
                return CreatedAtAction(nameof(GetMissionsVehicles), new { id = result }, dto);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Invalid argument provided.");
                return BadRequest(new { message = "Invalid argument provided." });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Invalid operation.");
                return BadRequest(new { message = "Invalid operation." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a MissionsVehicle.");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [Authorize(Policy = "RequireGeneralSupervisor")]
        [HttpGet]
        public async Task<IActionResult> GetMissionsVehicles([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                _logger.LogWarning("Page number and page size must be greater than 0.");
                return BadRequest(new { message = "Page number and page size must be greater than 0." });
            }

            try
            {
                var result = await _service.GetAllMissionsVehiclesAsync(pageNumber, pageSize);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Invalid argument provided.");
                return BadRequest(new { message = "Invalid argument provided." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving MissionsVehicles.");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [Authorize(Policy = "RequireGeneralSupervisor")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMissionsVehicleById([FromRoute] int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid ID provided.");
                return BadRequest(new { message = "ID must be greater than 0." });
            }

            try
            {
                var result = await _service.GetMissionsVehicleByIdAsync(id);
                return Ok(result);
            }

            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Invalid argument provided.");
                return BadRequest(new { message = "Invalid argument provided." });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, $"MissionsVehicle not found with ID {id}.");
                return NotFound(new { message = $"MissionsVehicle not found with ID {id}." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the MissionsVehicle.");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [Authorize(Policy = "RequireGeneralSupervisor")]
        [HttpGet("mission/{id}")]
        public async Task<IActionResult> GetMissionsVehiclesByMissionId([FromRoute] int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid ID provided.");
                return BadRequest(new { message = "ID must be greater than 0." });
            }

            try
            {
                var result = await _service.GetMissionsVehiclesByMissionIdAsync(id);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Invalid argument provided.");
                return BadRequest(new { message = "Invalid argument provided." });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, $"MissionsVehicle not found with ID {id}.");
                return NotFound(new { message = $"MissionsVehicle not found with ID {id}." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the MissionsVehicle.");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [Authorize(Policy = "RequireGeneralSupervisor")]
        [HttpGet("vehicle/{id}")]
        public async Task<IActionResult> GetMissionsVehiclesByVehicleId([FromRoute] int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid ID provided.");
                return BadRequest(new { message = "ID must be greater than 0." });
            }

            try
            {
                var result = await _service.GetMissionsVehiclesByVehicleIdAsync(id);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Invalid argument provided.");
                return BadRequest(new { message = "Invalid argument provided." });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "MissionsVehicle not found.");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the MissionsVehicle.");
                return StatusCode(500, "Internal server error");
            }
        }

        [Authorize(Policy = "GeneralSupervisor")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMissionsVehicle([FromRoute] int id, [FromBody] MissionsVehicleDTO dto)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid ID provided.");
                return BadRequest("ID must be greater than 0.");
            }
            if (dto == null)
            {
                _logger.LogWarning("MissionsVehicleDTO cannot be null.");
                return BadRequest("MissionsVehicleDTO cannot be null.");
            }
            if (dto.MissionVehicleId != id)
            {
                _logger.LogWarning("Mismatched ID in DTO.");
                return BadRequest("Mismatched ID in DTO.");
            }

            try
            {
                var result = await _service.UpdateMissionsVehicleAsync(dto);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Invalid argument provided.");
                return BadRequest(new { message = "Invalid argument provided." });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, $"MissionsVehicle not found with ID {id}.");
                return NotFound(new { message = $"MissionsVehicle not found with ID {id}." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the MissionsVehicle.");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [Authorize(Policy = "GeneralSupervisor")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMissionVehicle([FromRoute] int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid ID provided.");
                return BadRequest(new { message = "ID must be greater than 0." });
            }

            try
            {
                var result = await _service.DeleteMissionsVehicleAsync(id);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Invalid argument provided.");
                return BadRequest(new { message = "Invalid argument provided." });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, $"MissionsVehicle not found with ID {id}.");
                return NotFound(new { message = $"MissionsVehicle not found with ID {id}." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the MissionsVehicle.");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
    }
}
