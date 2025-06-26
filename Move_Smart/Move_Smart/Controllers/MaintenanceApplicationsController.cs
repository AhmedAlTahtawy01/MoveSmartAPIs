using BusinessLayer;
using DataAccessLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Move_Smart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaintenanceApplicationsController : ControllerBase
    {
        private readonly MaintenanceApplicationService _service;
        private readonly ILogger<MaintenanceApplicationsController> _logger;

        public MaintenanceApplicationsController(MaintenanceApplicationService service, ILogger<MaintenanceApplicationsController> logger)
        {
            _service = service;
            _logger = logger;
        }


        [Authorize(Policy = "RequireWorkshopSupervisor")]
        [HttpGet("All", Name = "GetAllMaintenanceApplications")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<MaintenanceApplicationDTO>>> GetAllMaintenanceApplications()
        {
            List<MaintenanceApplicationDTO> maintenanceApplications = await _service.GetAllMaintenanceApplicationsAsync();

            if (maintenanceApplications == null || !maintenanceApplications.Any())
            {
                return NotFound(new { message = "No maintenance applications found." });
            }

            return Ok(maintenanceApplications);
        }


        [Authorize(Policy = "RequireWorkshopSupervisor")]
        [HttpGet("ForVehicle/{vehicleID}", Name = "GetAllMaintenanceApplicationsForVehicle")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<MaintenanceApplicationDTO>>> GetAllMaintenanceApplicationsForVehicle(short vehicleID)
        {
            if (vehicleID <= 0)
            {
                return BadRequest(new { message = $"Invalid ID [{vehicleID}]!" });
            }

            List<MaintenanceApplicationDTO> maintenanceApplications = await _service.GetAllMaintenanceApplicationsForVehicleAsync(vehicleID);

            if (maintenanceApplications == null || !maintenanceApplications.Any())
            {
                return NotFound(new { message = $"No maintenance applications found for vehicle with ID [{vehicleID}]." });
            }

            return Ok(maintenanceApplications);
        }


        [Authorize(Policy = "RequireWorkshopSupervisor")]
        [HttpGet("{maintenanceApplicationID}", Name = "GetMaintenanceApplicationByMaintenanceApplicationID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<MaintenanceApplicationDTO>> GetMaintenanceApplicationByMaintenanceApplicationID(int maintenanceApplicationID)
        {
            if (maintenanceApplicationID <= 0)
            {
                return BadRequest(new { message = $"Invalid ID [{maintenanceApplicationID}]!" });
            }

            MaintenanceApplicationDTO? maintenanceApplication = await _service.GetMaintenanceApplicationByMaintenanceApplicationIDAsync(maintenanceApplicationID);

            if (maintenanceApplication == null)
            {
                return NotFound(new { message = $"No maintenance application found with ID [{maintenanceApplicationID}]." });
            }

            return Ok(maintenanceApplication);
        }


        [Authorize(Policy = "RequireWorkshopSupervisor")]
        [HttpPost(Name = "AddNewMaintenanceApplication")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<MaintenanceApplicationDTO>> AddNewMaintenanceApplication(MaintenanceApplicationDTO dto)
        {
            if (dto == null)
            {
                return BadRequest(new { message = "MaintenanceApplicationDTO cannot be null." });
            }

            if (await _service.AddNewMaintenanceApplicationAsync(dto) == null)
            {
                return BadRequest(new { message = "Failed to add new maintenance application!" });
            }

            return CreatedAtRoute("GetMaintenanceApplicationByMaintenanceApplicationID", new { maintenanceApplicationID = dto.MaintenanceApplicationID }, dto);
        }


        [Authorize(Policy = "RequireWorkshopSupervisor")]
        [HttpPut(Name = "UpdateMaintenanceApplication")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateMaintenanceApplication(MaintenanceApplicationDTO dto)
        {
            if (dto == null)
            {
                return BadRequest(new { message = "MaintenanceApplicationDTO cannot be null." });
            }

            if (dto.MaintenanceApplicationID <= 0)
            {
                return BadRequest(new { message = $"Invalid ID [{dto.MaintenanceApplicationID}]" });
            }

            if (!await _service.IsMaintenanceApplicationExistsAsync(dto.MaintenanceApplicationID ?? 0))
            {
                return NotFound(new { message = $"No maintenance application found with ID [{dto.MaintenanceApplicationID}]." });
            }
            
            if (!await _service.UpdateMaintenanceApplicationAsync(dto))
            {
                return BadRequest(new { message = $"Failed to update maintenance application with ID [{dto.MaintenanceApplicationID}]!" });
            }
            
            return Ok(new { message = $"Maintenance application with ID [{dto.MaintenanceApplicationID}] updated successfully." });
        }


        [Authorize(Policy = "RequireWorkshopSupervisor")]
        [HttpDelete("{maintenanceApplicationID}", Name = "DeleteMaintenanceApplication")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> DeleteMaintenanceApplication(int maintenanceApplicationID)
        {
            if (maintenanceApplicationID <= 0)
            {
                return BadRequest(new { message = $"Invalid ID [{maintenanceApplicationID}]!" });
            }
            
            if (!await _service.IsMaintenanceApplicationExistsAsync(maintenanceApplicationID))
            {
                return NotFound(new { message = $"No maintenance application found with ID [{maintenanceApplicationID}]." });
            }
            
            if (!await _service.DeleteMaintenanceApplicationAsync(maintenanceApplicationID))
            {
                return BadRequest(new { message = $"Failed to delete maintenance application with ID [{maintenanceApplicationID}]!" });
            }
            
            return Ok(new { message = $"Maintenance application with ID [{maintenanceApplicationID}] deleted successfully." });
        }
    }
}
