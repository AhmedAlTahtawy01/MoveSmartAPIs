using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Services;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using DataAccessLayer.Repositories;
using System.ComponentModel.DataAnnotations;

namespace Move_Smart.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]

    partial class MissionVehicleController : ControllerBase
    {
        private readonly MissionsVehicleService _service;
        private readonly ILogger<MissionVehicleController> _logger;

        public MissionVehicleController(MissionsVehicleService service, ILogger<MissionVehicleController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateMissionsVehicle([FromBody] MissionsVehicleDTO dto)
        {
            if (dto == null)
            {
                return BadRequest("MissionsVehicleDTO cannot be null.");
            }

            try
            {
                var result = await _service.CreateMissionsVehicleAsync(dto);
                return CreatedAtAction(nameof(GetMissionsVehicles), new { id = result }, dto);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument provided.");
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation.");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a MissionsVehicle.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMissionsVehicles([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                _logger.LogWarning("Page number and page size must be greater than 0.");
                return BadRequest("Page number and page size must be greater than 0.");
            }

            try
            {
                var result = await _service.GetAllMissionsVehiclesAsync(pageNumber, pageSize);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument provided.");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving MissionsVehicles.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMissionsVehicleById([FromRoute] int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid ID provided.");
                return BadRequest("ID must be greater than 0.");
            }

            try
            {
                var result = await _service.GetMissionsVehicleByIdAsync(id);
                return Ok(result);
            }

            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument provided.");
                return BadRequest(ex.Message);
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

        [HttpGet("mission/{id}")]
        public async Task<IActionResult> GetMissionsVehiclesByMissionId([FromRoute] int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid ID provided.");
                return BadRequest("ID must be greater than 0.");
            }

            try
            {
                var result = await _service.GetMissionsVehiclesByMissionIdAsync(id);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument provided.");
                return BadRequest(ex.Message);
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

        [HttpGet("vehicle/{id}")]
        public async Task<IActionResult> GetMissionsVehiclesByVehicleId([FromRoute] int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid ID provided.");
                return BadRequest("ID must be greater than 0.");
            }

            try
            {
                var result = await _service.GetMissionsVehiclesByVehicleIdAsync(id);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument provided.");
                return BadRequest(ex.Message);
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
                _logger.LogWarning(ex, "Invalid argument provided.");
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "MissionsVehicle not found.");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the MissionsVehicle.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMissionVehicle([FromRoute] int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid ID provided.");
                return BadRequest("ID must be greater than 0.");
            }

            try
            {
                var result = await _service.DeleteMissionsVehicleAsync(id);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument provided.");
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "MissionsVehicle not found.");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the MissionsVehicle.");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
