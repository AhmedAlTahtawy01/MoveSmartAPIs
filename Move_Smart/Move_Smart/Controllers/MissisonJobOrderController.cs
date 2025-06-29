using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using DataAccessLayer.Repositories;
using System.ComponentModel.DataAnnotations;
using BusinessLogicLayer.Services;
using Microsoft.AspNetCore.Authorization;


namespace Move_Smart.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MissionJobOrderController : ControllerBase
    {
        private readonly MissionsJobOrderService _service;
        private readonly ILogger<MissionJobOrderController> _logger;
        public MissionJobOrderController(MissionsJobOrderService service, ILogger<MissionJobOrderController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [Authorize(Policy = "GeneralSupervisor")]
        [HttpPost]
        public async Task<IActionResult> CreateMissionJobOrder([FromBody] MissionsJobOrderDTO dto)
        {
            if (dto == null)
            {
                _logger.LogWarning("Received null DTO for CreateMissionJobOrder.");
                return BadRequest(new { message = "DTO cannot be null." });
            }

            try
            {
                var result = await _service.CreateMissionJobOrderAsync(dto);
                return CreatedAtAction(nameof(GetAllMissionsJobOrders), new { id = result }, dto);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Argument exception while creating MissionJobOrder.");
                return BadRequest(new { message = "Argument exception while creating MissionJobOrder." });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Invalid operation while creating MissionJobOrder.");
                return BadRequest(new { message = "Invalid operation while creating MissionJobOrder." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating MissionJobOrder.");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [Authorize(Policy = "RequireGeneralSupervisor")]
        [HttpGet]
        public async Task<IActionResult> GetAllMissionsJobOrders([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                _logger.LogWarning("Invalid pagination parameters.");
                return BadRequest(new { message = "Page number and page size must be greater than 0." });
            }

            try
            {
                var result = await _service.GetAllMissionsJobOrderAsync(pageNumber, pageSize);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Argument exception while fetching MissionJobOrders.");
                return BadRequest(new { message = "Argument exception while fetching MissionJobOrders." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching MissionJobOrders.");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [Authorize(Policy = "RequireGeneralSupervisor")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMissionJobOrderById([FromRoute] int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid ID parameter.");
                return BadRequest(new { message = "ID must be greater than 0." });
            }

            try
            {
                var result = await _service.GetMissionsJobOrderByIdAsync(id);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Argument exception while fetching MissionJobOrder by ID.");
                return BadRequest(new { message = "Argument exception while fetching MissionJobOrder by ID." });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, $"No MissionJobOrder found with ID {id}.");
                return NotFound(new { message = $"No MissionJobOrder found with ID {id}." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching MissionJobOrder by ID.");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [Authorize(Policy = "RequireGeneralSupervisor")]
        [HttpGet("mission/{id}")]
        public async Task<IActionResult> GetMissionJobOrderByMissionId([FromRoute] int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid ID parameter.");
                return BadRequest(new { message = "ID must be greater than 0." });
            }

            try
            {
                var result = await _service.GetMissionsJobOrderByMissionIdAsync(id);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Argument exception while fetching MissionJobOrder by Mission ID.");
                return BadRequest(new { message = "Argument exception while fetching MissionJobOrder by Mission ID." });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, $"No MissionJobOrder found with Mission ID {id}.");
                return NotFound(new { message = $"No MissionJobOrder found with Mission ID {id}." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching MissionJobOrder by Mission ID.");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [Authorize(Policy = "RequireGeneralSupervisor")]
        [HttpGet("joborder/{id}")]
        public async Task<IActionResult> GetMissionJobOrderByJobOrderId([FromRoute] int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid ID parameter.");
                return BadRequest(new { message = "ID must be greater than 0." });
            }

            try
            {
                var result = await _service.GetMissionsJobOrdersByJobOrderIdAsync(id);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Argument exception while fetching MissionJobOrder by Job Order ID.");
                return BadRequest(new { message = "Argument exception while fetching MissionJobOrder by Job Order ID." });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, $"No MissionJobOrder found with Job Order ID {id}.");
                return NotFound(new { message = $"No MissionJobOrder found with Job Order ID {id}." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching MissionJobOrder by Job Order ID.");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [Authorize(Policy = "GeneralSupervisor")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMissionJobOrderAsync([FromRoute] int id, [FromBody] MissionsJobOrderDTO dto)
        {
            if (id <= 0 || dto == null)
            {
                _logger.LogWarning("Invalid parameters for UpdateMissionJobOrder.");
                return BadRequest(new { message = "ID must be greater than 0 and DTO cannot be null." });
            }
            if (dto.OrderId != id)
            {
                _logger.LogWarning("Mismatch between URL ID and DTO ID.");
                return BadRequest(new { message = "ID in URL must match ID in DTO." });
            }

            try
            {
                var result = await _service.UpdateMissionJobOrderAsync(dto);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Argument exception while updating MissionJobOrder.");
                return BadRequest(new { message = "Argument exception while updating MissionJobOrder." });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, $"No MissionJobOrder found with ID {id}.");
                return NotFound(new { message = $"No MissionJobOrder found with ID {id}." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating MissionJobOrder.");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [Authorize(Policy = "GeneralSupervisor")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMissionJobOrder([FromRoute] int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid ID parameter.");
                return BadRequest(new { message = "ID must be greater than 0." });
            }

            try
            {
                var result = await _service.DeleteMissionsJobOrderAsync(id);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Argument exception while deleting MissionJobOrder.");
                return BadRequest(new { message = "Argument exception while deleting MissionJobOrder." });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, $"No MissionJobOrder found with ID {id}.");
                return NotFound(new { message = $"No MissionJobOrder found with ID {id}." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting MissionJobOrder.");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
    }

}