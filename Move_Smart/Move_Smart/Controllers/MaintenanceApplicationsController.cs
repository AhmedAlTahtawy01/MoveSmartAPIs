using BusinessLayer;
using DataAccessLayer;
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


        [HttpGet("All", Name = "GetAllMaintenanceApplications")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<MaintenanceApplicationDTO>>> GetAllMaintenanceApplications()
        {
            List<MaintenanceApplicationDTO> maintenanceApplications = await _service.GetAllMaintenanceApplicationsAsync();

            if (maintenanceApplications == null || !maintenanceApplications.Any())
            {
                return NotFound("No maintenance applications found.");
            }

            return Ok(maintenanceApplications);
        }


        [HttpGet("ForVehicle/{vehicleID}", Name = "GetAllMaintenanceApplicationsForVehicle")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<MaintenanceApplicationDTO>>> GetAllMaintenanceApplicationsForVehicle(short vehicleID)
        {
            if (vehicleID <= 0)
            {
                return BadRequest($"Invalid ID [{vehicleID}]!");
            }

            List<MaintenanceApplicationDTO> maintenanceApplications = await _service.GetAllMaintenanceApplicationsForVehicleAsync(vehicleID);

            if (maintenanceApplications == null || !maintenanceApplications.Any())
            {
                return NotFound($"No maintenance applications found for vehicle with ID [{vehicleID}].");
            }

            return Ok(maintenanceApplications);
        }


        [HttpGet("{maintenanceApplicationID}", Name = "GetMaintenanceApplicationByMaintenanceApplicationID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<MaintenanceApplicationDTO>> GetMaintenanceApplicationByMaintenanceApplicationID(int maintenanceApplicationID)
        {
            if (maintenanceApplicationID <= 0)
            {
                return BadRequest($"Invalid ID [{maintenanceApplicationID}]!");
            }

            MaintenanceApplicationDTO? maintenanceApplication = await _service.GetMaintenanceApplicationByMaintenanceApplicationIDAsync(maintenanceApplicationID);

            if (maintenanceApplication == null)
            {
                return NotFound($"No maintenance application found with ID [{maintenanceApplicationID}].");
            }

            return Ok(maintenanceApplication);
        }


        [HttpPost(Name = "AddNewMaintenanceApplication")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<MaintenanceApplicationDTO>> AddNewMaintenanceApplication(MaintenanceApplicationDTO dto)
        {
            if (dto == null)
            {
                return BadRequest("MaintenanceApplicationDTO cannot be null.");
            }

            if (await _service.AddNewMaintenanceApplicationAsync(dto) == null)
            {
                return BadRequest("Failed to add new maintenance application!");
            }

            return CreatedAtRoute("GetMaintenanceApplicationByMaintenanceApplicationID", new { maintenanceApplicationID = dto.MaintenanceApplicationID }, dto);
        }


        [HttpPut(Name = "UpdateMaintenanceApplication")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateMaintenanceApplication(MaintenanceApplicationDTO dto)
        {
            if (dto == null)
            {
                return BadRequest("MaintenanceApplicationDTO cannot be null.");
            }

            if (dto.MaintenanceApplicationID <= 0)
            {
                return BadRequest($"Invalid ID [{dto.MaintenanceApplicationID}]");
            }

            if (!await _service.IsMaintenanceApplicationExistsAsync(dto.MaintenanceApplicationID ?? 0))
            {
                return NotFound($"No maintenance application found with ID [{dto.MaintenanceApplicationID}].");
            }
            
            if (!await _service.UpdateMaintenanceApplicationAsync(dto))
            {
                return BadRequest($"Failed to update maintenance application with ID [{dto.MaintenanceApplicationID}]!");
            }
            
            return Ok($"Maintenance application with ID [{dto.MaintenanceApplicationID}] updated successfully.");
        }


        [HttpDelete("{maintenanceApplicationID}", Name = "DeleteMaintenanceApplication")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> DeleteMaintenanceApplication(int maintenanceApplicationID)
        {
            if (maintenanceApplicationID <= 0)
            {
                return BadRequest($"Invalid ID [{maintenanceApplicationID}]!");
            }
            
            if (!await _service.IsMaintenanceApplicationExistsAsync(maintenanceApplicationID))
            {
                return NotFound($"No maintenance application found with ID [{maintenanceApplicationID}].");
            }
            
            if (!await _service.DeleteMaintenanceApplicationAsync(maintenanceApplicationID))
            {
                return BadRequest($"Failed to delete maintenance application with ID [{maintenanceApplicationID}]!");
            }
            
            return Ok($"Maintenance application with ID [{maintenanceApplicationID}] deleted successfully.");
        }
    }
}
