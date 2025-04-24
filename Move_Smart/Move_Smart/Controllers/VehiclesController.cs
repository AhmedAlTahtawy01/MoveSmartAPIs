using BusinessLayer;
using DataAccessLayer;
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

        [HttpGet("All", Name = "GetAllVehicles")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<VehicleDTO>>> GetAllVehiclesAsync()
        {
            List<VehicleDTO> vehicles = await _service.GetAllVehiclesAsync();

            if (vehicles == null || !vehicles.Any())
            {
                return NotFound("No vehicles found.");
            }

            return Ok(vehicles);
        }


        [HttpGet("OfType/{vehicleType}", Name = "GetAllVehiclesOfType")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<VehicleDTO>>> GetAllVehiclesOfType(enVehicleType vehicleType)
        {
            List<VehicleDTO> vehicles = await _service.GetAllVehiclesOfTypeAsync(vehicleType);

            if (vehicles == null || !vehicles.Any())
            {
                return NotFound($"No vehicles found of type [{vehicleType}].");
            }

            return Ok(vehicles);
        }


        [HttpGet("WithStatus/{vehicleStatus}", Name = "GetAllVehiclesWithStatus")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<VehicleDTO>>> GetAllVehiclesWithStatus(enVehicleStatus vehicleStatus)
        {
            List<VehicleDTO> vehicles = await _service.GetAllVehiclesWithStatusAsync(vehicleStatus);

            if (vehicles == null || !vehicles.Any())
            {
                return NotFound($"No vehicles found with status [{vehicleStatus}].");
            }

            return Ok(vehicles);
        }


        [HttpGet("UsingFuelOfType/{fuelType}", Name = "GetAllVehiclesUsingFuelOfType")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<VehicleDTO>>> GetAllVehiclesUsingFuelOfType(enFuelType fuelType)
        {
            List<VehicleDTO> vehicles = await _service.GetAllVehiclesUsingFuelOfTypeAsync(fuelType);

            if (vehicles == null || !vehicles.Any())
            {
                return NotFound($"No vehicles found using fuel of type [{fuelType}].");
            }

            return Ok(vehicles);
        }


        [HttpGet("{vehicleID}", Name = "GetVehicleByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VehicleDTO?>> GetVehicleByID(short vehicleID)
        {
            VehicleDTO? vehicle = await _service.GetVehicleByIDAsync(vehicleID);

            if (vehicle == null)
            {
                return NotFound($"Vehicle with ID [{vehicleID}] not found.");
            }

            return Ok(vehicle);
        }


        [HttpGet("Count", Name = "GetNumberOfVehicles")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<short>> GetNumberOfVehicles()
        {
            short numberOfVehicles = await _service.GetNumberOfVehiclesAsync();

            if (numberOfVehicles == 0)
            {
                return NotFound("No vehicles found.");
            }

            return Ok(numberOfVehicles);
        }


        [HttpGet("Count/WithStatus/{vehicleStatus}", Name = "GetNumberOfVehiclesOfType")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<short>> GetNumberOfVehiclesWithStatus(enVehicleStatus vehicleStatus)
        {
            short numberOfVehicles = await _service.GetNumberOfVehiclesWithStatusAsync(vehicleStatus);

            if (numberOfVehicles == 0)
            {
                return NotFound($"No vehicles found with status [{vehicleStatus}].");
            }

            return Ok(numberOfVehicles);
        }


        [HttpPost(Name = "AddNewVehicle")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<VehicleDTO>> AddNewVehicle(VehicleDTO dto)
        {
            if (await _service.AddNewVehicleAsync(dto) == null)
            {
                return BadRequest("Failed to add new vehicle.");
            }

            return CreatedAtRoute("GetVehicleByID", new { vehicleID = dto.VehicleID }, dto);
        }


        [HttpPut(Name = "UpdateVehicle")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateVehicle(VehicleDTO dto)
        {
            if (dto.VehicleID <= 0)
            {
                return BadRequest($"Invalid vehicle ID [{dto.VehicleID}].");
            }

            if(!await _service.IsVehicleExistsAsync(dto.VehicleID ?? 0))
            {
                return NotFound($"Vehicle with ID [{dto.VehicleID}] not found!");
            }

            if (!await _service.UpdateVehicleAsync(dto))
            {
                return BadRequest($"Update for vehicle with ID [{dto.VehicleID}] failed");
            }

            return Ok($"Vehicle with ID [{dto.VehicleID}] updated successfully");
        }


        [HttpDelete("ByID/{vehicleID}", Name = "DeleteVehicleByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> DeleteVehicle(short vehicleID)
        {
            if (vehicleID <= 0)
            {
                return BadRequest($"Invalid vehicle ID [{vehicleID}].");
            }

            if (!await _service.IsVehicleExistsAsync(vehicleID))
            {
                return NotFound($"Vehicle with ID [{vehicleID}] not found!");
            }
            
            if (!await _service.DeleteVehicleAsync(vehicleID))
            {
                return BadRequest($"Can't delete vehicle with ID [{vehicleID}]");
            }

            return Ok($"Vehicle with ID [{vehicleID}] deleted successfully");
        }


        [HttpDelete("ByPlateNumbers/{plateNumbers}", Name = "DeleteVehicleByPlateNumbers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> DeleteVehicle(string plateNumbers)
        {
            if (plateNumbers.Length != 6 && plateNumbers.Length != 7)
            {
                return BadRequest($"Invalid vehicle plate numbers [{plateNumbers}].");
            }

            if (!await _service.IsVehicleExistsAsync(plateNumbers))
            {
                return NotFound($"Vehicle with plate numbers [{plateNumbers}] not found!");
            }
            
            if (!await _service.DeleteVehicleAsync(plateNumbers))
            {
                return BadRequest($"Can't delete vehicle with plate numbers [{plateNumbers}]!");
            }

            return Ok($"Vehicle with plate numbers [{plateNumbers}] deleted successfully");
        }
    }
}
