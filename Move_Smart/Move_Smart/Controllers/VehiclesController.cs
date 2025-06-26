using BusinessLayer;
using DataAccessLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static DataAccessLayer.VehicleDTO;

namespace Move_Smart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehiclesController : ControllerBase
    {
        private readonly VehicleService _service;
        private readonly ILogger<VehiclesController> _logger;

        public VehiclesController(VehicleService service, ILogger<VehiclesController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [Authorize(Policy = "AllVehicles")]
        [HttpGet("All", Name = "GetAllVehicles")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<VehicleDTO>>> GetAllVehiclesAsync()
        {
            List<VehicleDTO> vehicles = await _service.GetAllVehiclesAsync();

            if (vehicles == null || !vehicles.Any())
            {
                return NotFound(new { message = "No vehicles found." });
            }

            return Ok(vehicles);
        }


        [Authorize(Policy = "AllVehicles")]
        [HttpGet("OfType/{vehicleType}", Name = "GetAllVehiclesOfType")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<VehicleDTO>>> GetAllVehiclesOfType(enVehicleType vehicleType)
        {
            List<VehicleDTO> vehicles = await _service.GetAllVehiclesOfTypeAsync(vehicleType);

            if (vehicles == null || !vehicles.Any())
            {
                return NotFound(new { message = $"No vehicles found of type [{vehicleType}]." });
            }

            return Ok(vehicles);
        }


        [Authorize(Policy = "AllVehicles")]
        [HttpGet("WithStatus/{vehicleStatus}", Name = "GetAllVehiclesWithStatus")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<VehicleDTO>>> GetAllVehiclesWithStatus(enVehicleStatus vehicleStatus)
        {
            List<VehicleDTO> vehicles = await _service.GetAllVehiclesWithStatusAsync(vehicleStatus);

            if (vehicles == null || !vehicles.Any())
            {
                return NotFound(new { message = $"No vehicles found with status [{vehicleStatus}]." });
            }

            return Ok(vehicles);
        }


        [Authorize(Policy = "AllVehicles")]
        [HttpGet("UsingFuelOfType/{fuelType}", Name = "GetAllVehiclesUsingFuelOfType")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<VehicleDTO>>> GetAllVehiclesUsingFuelOfType(enFuelType fuelType)
        {
            List<VehicleDTO> vehicles = await _service.GetAllVehiclesUsingFuelOfTypeAsync(fuelType);

            if (vehicles == null || !vehicles.Any())
            {
                return NotFound(new { message = $"No vehicles found using fuel of type [{fuelType}]." });
            }

            return Ok(vehicles);
        }


        [Authorize(Policy = "AllVehicles")]
        [HttpGet("{vehicleID}", Name = "GetVehicleByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VehicleDTO?>> GetVehicleByID(short vehicleID)
        {
            VehicleDTO? vehicle = await _service.GetVehicleByIDAsync(vehicleID);

            if (vehicle == null)
            {
                _logger.LogWarning("Vehicle not found: {VehicleID}", vehicleID);
                return NotFound(new { message = $"Vehicle with ID [{vehicleID}] not found." });
            }

            return Ok(vehicle);
        }


        [Authorize(Policy = "AllVehicles")]
        [HttpGet("Count", Name = "GetNumberOfVehicles")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<short>> GetNumberOfVehicles()
        {
            short numberOfVehicles = await _service.GetNumberOfVehiclesAsync();

            if (numberOfVehicles == 0)
            {
                _logger.LogWarning("No vehicles found.");
                return NotFound(new { message = "No vehicles found." });
            }

            return Ok(numberOfVehicles);
        }


        [Authorize(Policy = "AllVehicles")]
        [HttpGet("Count/WithStatus/{vehicleStatus}", Name = "GetNumberOfVehiclesOfType")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<short>> GetNumberOfVehiclesWithStatus(enVehicleStatus vehicleStatus)
        {
            short numberOfVehicles = await _service.GetNumberOfVehiclesWithStatusAsync(vehicleStatus);

            if (numberOfVehicles == 0)
            {
                _logger.LogWarning("No vehicles found with status: {VehicleStatus}", vehicleStatus);
                return NotFound(new { message = $"No vehicles found with status [{vehicleStatus}]." });
            }


            return Ok(numberOfVehicles);
        }


        [Authorize(Policy = "RequireGeneralManager")]
        [HttpPost(Name = "AddNewVehicle")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<VehicleDTO>> AddNewVehicle(VehicleDTO dto)
        {
            if (await _service.AddNewVehicleAsync(dto) == null)
            {
                _logger.LogWarning("Failed to add new vehicle.");
                return BadRequest(new { message = "Failed to add new vehicle." });
            }

            return CreatedAtRoute("GetVehicleByID", new { vehicleID = dto.VehicleID }, dto);
        }

        
        [Authorize(Policy = "RequireGeneralSupervisor")]
        [HttpPut(Name = "UpdateVehicle")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateVehicle(VehicleDTO dto)
        {
            _logger.LogInformation("Updating vehicle: {VehicleID}", dto.VehicleID);
            if (dto.VehicleID <= 0)
            {
                _logger.LogWarning("Invalid vehicle ID: {VehicleID}", dto.VehicleID);
                return BadRequest(new { message = $"Invalid vehicle ID [{dto.VehicleID}]." });
            }

            if(!await _service.IsVehicleExistsAsync(dto.VehicleID ?? 0))
            {
                _logger.LogWarning("Vehicle not found: {VehicleID}", dto.VehicleID);
                return NotFound(new { message = $"Vehicle with ID [{dto.VehicleID}] not found!" });
            }

            if (!await _service.UpdateVehicleAsync(dto))
            {
                _logger.LogWarning("Update failed: {VehicleID}", dto.VehicleID);
                return BadRequest(new { message = $"Update for vehicle with ID [{dto.VehicleID}] failed" });
            }

            return Ok(new { message = $"Vehicle with ID [{dto.VehicleID}] updated successfully" });
        }


        [Authorize(Policy = "RequireGeneralManager")]
        [HttpDelete("ByID/{vehicleID}", Name = "DeleteVehicleByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> DeleteVehicle(short vehicleID)
        {
            _logger.LogInformation("Deleting vehicle: {VehicleID}", vehicleID);
            if (vehicleID <= 0)
            {
                _logger.LogWarning("Invalid vehicle ID: {VehicleID}", vehicleID);
                return BadRequest(new { message = $"Invalid vehicle ID [{vehicleID}]." });
            }

            if (!await _service.IsVehicleExistsAsync(vehicleID))
            {
                _logger.LogWarning("Vehicle not found: {VehicleID}", vehicleID);
                return NotFound(new { message = $"Vehicle with ID [{vehicleID}] not found!" });
            }
            
            if (!await _service.DeleteVehicleAsync(vehicleID))
            {
                _logger.LogWarning("Delete failed: {VehicleID}", vehicleID);
                return BadRequest(new { message = $"Can't delete vehicle with ID [{vehicleID}]" });
            }

            return Ok(new { message = $"Vehicle with ID [{vehicleID}] deleted successfully" });
        }


        [Authorize(Policy = "RequireGeneralManager")]
        [HttpDelete("ByPlateNumbers/{plateNumbers}", Name = "DeleteVehicleByPlateNumbers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> DeleteVehicle(string plateNumbers)
        {
            _logger.LogInformation("Deleting vehicle by plate numbers: {PlateNumbers}", plateNumbers);
            if (plateNumbers.Length != 6 && plateNumbers.Length != 7)
            {
                _logger.LogWarning("Invalid vehicle plate numbers: {PlateNumbers}", plateNumbers);
                return BadRequest(new { message = $"Invalid vehicle plate numbers [{plateNumbers}]." });
            }

            if (!await _service.IsVehicleExistsAsync(plateNumbers))
            {
                _logger.LogWarning("Vehicle not found: {PlateNumbers}", plateNumbers);
                return NotFound(new { message = $"Vehicle with plate numbers [{plateNumbers}] not found!" });
            }
            
            if (!await _service.DeleteVehicleAsync(plateNumbers))
            {
                _logger.LogWarning("Delete failed: {PlateNumbers}", plateNumbers);
                return BadRequest(new { message = $"Can't delete vehicle with plate numbers [{plateNumbers}]!" });
            }

            return Ok(new { message = $"Vehicle with plate numbers [{plateNumbers}] deleted successfully" });
        }
    }
}
