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
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MaintenanceController : ControllerBase
    {
        private readonly MaintenanceService _service;
        private readonly ILogger<MaintenanceController> _logger;

        public MaintenanceController(MaintenanceService service, ILogger<MaintenanceController> logger)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service), "Service cannot be null.");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger cannot be null.");
        }

        [Authorize(Policy = "WorkshopSupervisor")]
        [HttpPost]
        public async Task<IActionResult> CreateMaintenance([FromBody] MaintenanceDTO dto)
        {
            if (dto == null)
            {
                _logger.LogWarning("Received null maintenance DTO.");
                return BadRequest(new { message = "Maintenance data cannot be null." });
            }

            try
            {
                _logger.LogInformation("Creating maintenance.");
                var id = await _service.CreateMaintenanceAsync(dto);
                return CreatedAtAction(nameof(GetMaintenanceById), new { id }, dto);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error creating maintenance.");
                return BadRequest(new { message = "Invalid argument provided." });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Invalid operation while creating maintenance.");
                return Conflict(new { message = $"Invalid Data" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating maintenance.");
                return StatusCode(500, new { message = "Internal server error." });
            }
        }

        [Authorize(Policy = "RequireWorkshopSupervisor")]
        [HttpGet]
        public async Task<IActionResult> GetAllMaintenances([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                _logger.LogWarning("Invalid pagination parameters.");
                return BadRequest(new { message = "Page number and page size must be greater than 0." });
            }

            try
            {
                var maintenances = await _service.GetAllMaintenancesAsync(pageNumber, pageSize);
                return Ok(maintenances);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid arguments for retrieving maintenances.");
                return BadRequest(new { message = "Invalid argument provided." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all maintenances.");
                return StatusCode(500, new { message = "Internal server error." });
            }
        }

        [Authorize(Policy = "RequireWorkshopSupervisor")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMaintenanceById([FromRoute] int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid maintenance ID.");
                return BadRequest(new { message = "Maintenance ID must be greater than 0." });
            }

            try
            {
                var maintenance = await _service.GetMaintenanceByIdAsync(id);
                return Ok(maintenance);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Invalid arguments for retrieving maintenance by ID.");
                return BadRequest(new { message = "Invalid argument provided." });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, "No maintenance found with ID {MaintenanceId}.", id);
                return NotFound(new { message = $"No maintenance found with ID {id}." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving maintenance by ID.");
                return StatusCode(500, new { message = "Internal server error." });
            }
        }

        [Authorize(Policy = "RequireWorkshopSupervisor")]
        [HttpGet("vehicle/{vehicleId}")]
        public async Task<IActionResult> GetMaintenancesByVehicleId(int vehicleId)
        {
            try
            {
                var maintenances = await _service.GetMaintenancesByVehicleIdAsync(vehicleId);
                return Ok(maintenances);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Invalid arguments for retrieving maintenance by vehicle ID.");
                return BadRequest(new { message = "Invalid argument provided." });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, "No maintenance found for vehicle ID {VehicleId}.", vehicleId);
                return NotFound(new { message = $"No maintenance found for vehicle ID {vehicleId}." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving maintenance by vehicle ID.");
                return StatusCode(500, new { message = "Internal server error." });
            }
        }

        [Authorize(Policy = "RequireWorkshopSupervisor")]
        [HttpGet("date/{date}")]
        public async Task<IActionResult> GetMaintenanceByDate(DateTime date)
        {
            if (date == default)
            {
                _logger.LogError("Invalid date parameter.");
                return BadRequest(new { message = "Date cannot be default value." });
            }

            try
            {
                var maintenances = await _service.GetMaintenancesByDateAsync(date);
                return Ok(maintenances);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Invalid arguments for retrieving maintenance by date.");
                return BadRequest(new { message = "Invalid argument provided." });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, "No maintenance found for date {Date}.", date);
                return NotFound(new { message = $"No maintenance found for date {date}." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving maintenance by date.");
                return StatusCode(500, new { message = "Internal server error." });
            }
        }

        [Authorize(Policy = "RequireWorkshopSupervisor")]
        [HttpGet("maintenance-application-id/{maintenanceApplicationId}")]
        public async Task<IActionResult> GetMaintenanceByMaintenanceApplicationId(int maintenanceApplicationId)
        {
            if (maintenanceApplicationId <= 0)
            {
                _logger.LogError("Invalid maintenance application ID.");
                return BadRequest(new { message = "Maintenance application ID must be greater than 0." });
            }

            try
            {
                var maintenances = await _service.GetMaintenanceByMaintenanceApplicationIdAsync(maintenanceApplicationId);
                return Ok(maintenances);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Invalid arguments for retrieving maintenance by application ID.");
                return BadRequest(new { message = "Invalid argument provided." });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, "No maintenance found for application ID {MaintenanceApplicationId}.", maintenanceApplicationId);
                return NotFound(new { message = $"No maintenance found for application ID {maintenanceApplicationId}." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving maintenance by application ID.");
                return StatusCode(500, new { message = "Internal server error." });
            }
        }

        [Authorize(Policy = "WorkshopSupervisor")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMaintenance(int id, [FromBody] MaintenanceDTO dto)
        {
            if (dto == null)
            {
                _logger.LogError("Received null maintenance DTO.");
                return BadRequest(new { message = "Maintenance data cannot be null." });
            }
            if (id <= 0)
            {
                _logger.LogError("Invalid maintenance ID.");
                return BadRequest(new { message = "Maintenance ID must be greater than 0." });
            }
            if (dto.MaintenanceId != id)
            {
                _logger.LogError("Mismatch between ID in URL and ID in DTO.");
                return BadRequest(new { message = "ID in URL and ID in DTO must match." });
            }

            try
            {
                dto.MaintenanceId = id;
                var updated = await _service.UpdateMaintenanceAsync(dto);
                return Ok(updated);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Invalid arguments for updating maintenance.");
                return BadRequest(new { message = "Invalid argument provided." });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, "No maintenance found with ID {MaintenanceId} for update.", id);
                return NotFound(new { message = $"No maintenance found with ID {id} for update." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating maintenance.");
                return StatusCode(500, new { message = "Internal server error." });
            }

        }

        [Authorize(Policy = "WorkshopSupervisor")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMaintenance(int id)
        {
            if (id <= 0)
            {
                _logger.LogError("Invalid maintenance ID.");
                return BadRequest(new { message = "Maintenance ID must be greater than 0." });
            }

            try 
            {
                await _service.DeleteMaintenanceAsync(id);
                return Ok(new { message = $"Maintenance with ID {id} deleted successfully." });
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Invalid arguments for deleting maintenance.");
                return BadRequest(new { message = "Invalid argument provided." });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, "No maintenance found with ID {MaintenanceId} for deletion.", id);
                return NotFound(new { message = $"No maintenance found with ID {id} for deletion." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting maintenance.");
                return StatusCode(500, new { message = "Internal server error." });
            }
        }
    }
}
