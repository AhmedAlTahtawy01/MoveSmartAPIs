using BusinessLayer;
using DataAccessLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static DataAccessLayer.VehicleDTO;

namespace Move_Smart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleController : ControllerBase
    {
        private readonly VehicleService _service;
        private readonly ILogger<VehicleController> _logger;

        public VehicleController(VehicleService service, ILogger<VehicleController> logger)
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
    }
}
