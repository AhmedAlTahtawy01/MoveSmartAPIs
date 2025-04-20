using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Services;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using DataAccessLayer.Repositories;
using System.ComponentModel.DataAnnotations;
using BusinessLogicLayer.Services;


namespace Move_Smart.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class MaintenanceController : ControllerBase
    {
        private readonly MaintenanceService _service;
        private readonly ILogger<MaintenanceController> _logger;

        public MaintenanceController(MaintenanceService service, ILogger<MaintenanceController> logger)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service), "Service cannot be null.");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger cannot be null.");
        }

        [HttpPost]
        public async Task<IActionResult> CreateMaintenance([FromBody] MaintenanceDTO dto)
        {
            if (dto == null)
            {
                _logger.LogWarning("Received null maintenance DTO.");
                return BadRequest("Maintenance data cannot be null.");
            }

            try
            {
                var id = await _service.CreateMaintenanceAsync(dto);
                return CreatedAtAction(nameof(GetMaintenanceById), new { id }, dto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating maintenance.");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMaintenances([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                _logger.LogWarning("Invalid pagination parameters.");
                return BadRequest("Page number and page size must be greater than 0.");
            }

            try
            {
                var maintenances = await _service.GetAllMaintenancesAsync(pageNumber, pageSize);
                return Ok(maintenances);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid arguments for retrieving maintenances.");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all maintenances.");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMaintenanceById([FromRoute] int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid maintenance ID.");
                return BadRequest("Maintenance ID must be greater than 0.");
            }

            try
            {
                var maintenance = await _service.GetMaintenanceByIdAsync(id);
                return Ok(maintenance);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid arguments for retrieving maintenance by ID.");
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, $"No maintenance found with ID {id}.");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving maintenance by ID.");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpGet("date/{date}")]
        public async Task<IActionResult> GetMaintenanceByDate(DateTime date)
        {
            if (date == default)
            {
                _logger.LogWarning("Invalid date parameter.");
                return BadRequest("Date cannot be default value.");
            }

            try
            {
                var maintenances = await _service.GetMaintenancesByDateAsync(date);
                return Ok(maintenances);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid arguments for retrieving maintenance by date.");
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, $"No maintenance found for date {date}.");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving maintenance by date.");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpGet("maintenance-application-id/{maintenanceApplicationId}")]
        public async Task<IActionResult> GetMaintenanceByMaintenanceApplicationId(int maintenanceApplicationId)
        {
            if (maintenanceApplicationId <= 0)
            {
                _logger.LogWarning("Invalid maintenance application ID.");
                return BadRequest("Maintenance application ID must be greater than 0.");
            }

            try
            {
                var maintenances = await _service.GetMaintenanceByMaintenanceApplicationIdAsync(maintenanceApplicationId);
                return Ok(maintenances);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid arguments for retrieving maintenance by application ID.");
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, $"No maintenance found for application ID {maintenanceApplicationId}.");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving maintenance by application ID.");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMaintenance(int id, [FromBody] MaintenanceDTO dto)
        {
            if (dto == null)
            {
                _logger.LogWarning("Received null maintenance DTO.");
                return BadRequest("Maintenance data cannot be null.");
            }
            if (id <= 0)
            {
                _logger.LogWarning("Invalid maintenance ID.");
                return BadRequest("Maintenance ID must be greater than 0.");
            }
            if (dto.MaintenanceId != id)
            {
                _logger.LogWarning("Mismatch between ID in URL and ID in DTO.");
                return BadRequest("ID in URL and ID in DTO must match.");
            }

            try
            {
                dto.MaintenanceId = id;
                var updated = await _service.UpdateMaintenanceAsync(dto);
                return Ok(updated);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid arguments for updating maintenance.");
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, $"No maintenance found with ID {id} for update.");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating maintenance.");
                return StatusCode(500, "Internal server error.");
            }

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMaintenance(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid maintenance ID.");
                return BadRequest("Maintenance ID must be greater than 0.");
            }

            try 
            {
                await _service.DeleteMaintenanceAsync(id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid arguments for deleting maintenance.");
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, $"No maintenance found with ID {id} for deletion.");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting maintenance.");
                return StatusCode(500, "Internal server error.");
            }
        }
    }
}
