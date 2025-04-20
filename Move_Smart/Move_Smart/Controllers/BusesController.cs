using BusinessLayer;
using DataAccessLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Move_Smart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusesController : ControllerBase
    {
        private readonly BusService _service;
        private readonly ILogger<BusesController> _logger;

        public BusesController(BusService service, ILogger<BusesController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet("All", Name = "GetAllBuses")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<BusDTO>>> GetAllBuses()
        {
            List<BusDTO> buses = await _service.GetAllBusesAsync();

            if (buses == null || !buses.Any())
            {
                return NotFound("No buses found.");
            }

            return Ok(buses);
        }


        [HttpGet("All/OfCapacity/{capacity}", Name = "GetAllBusesOfType")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<BusDTO>>> GetBusesByCapacity(byte capacity)
        {
            List<BusDTO> buses = await _service.GetBusesByCapacityAsync(capacity);

            if (buses == null || !buses.Any())
            {
                return NotFound($"No buses found with capacity [{capacity}] seats.");
            }

            return Ok(buses);
        }


        [HttpGet("All/WithAvailableSpace/{availableSpace}", Name = "GetAllBusesWithAvailableSpace")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<BusDTO>>> GetBusesByAvailableSpace(byte availableSpace)
        {
            List<BusDTO> buses = await _service.GetBusesByAvailableSpaceAsync(availableSpace);
            
            if (buses == null || !buses.Any())
            {
                return NotFound($"No buses found with available space [{availableSpace}] seats.");
            }
         
            return Ok(buses);
        }


        [HttpGet("ByID/{busID}", Name = "GetBusByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BusDTO?>> GetBusByID(byte busID)
        {
            BusDTO? bus = await _service.GetBusByIDAsync(busID);

            if (bus == null)
            {
                return NotFound($"No bus found with ID [{busID}].");
            }
            
            return Ok(bus);
        }


        [HttpGet("ByPlateNumbers/{plateNumbers}", Name = "GetBusByPlateNumbers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BusDTO?>> GetBusByPlateNumbers(string plateNumbers)
        {
            BusDTO? bus = await _service.GetBusByPlateNumbersAsync(plateNumbers);
            
            if (bus == null)
            {
                return NotFound($"No bus found with Plate Numbers [{plateNumbers}].");
            }

            return Ok(bus);
        }


        [HttpPost(Name = "AddNewBus")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BusDTO>> AddNewBus(BusDTO dto)
        {
            if (dto == null)
            {
                return BadRequest("BusDTO can't be null.");
            }

            dto.VehicleID = await _service.AddNewVehicleAsync(dto.Vehicle) ?? 0;

            if(dto.VehicleID == 0)
            {
                return BadRequest("Failed to add new vehicle.");
            }

            if(await _service.AddNewBusAsync(dto) == null)
            {
                await _service.DeleteVehicleAsync(dto.VehicleID);
                return BadRequest("Failed to add new bus.");
            }

            return CreatedAtRoute("GetBusByID", new { busID = dto.BusID}, dto);
        }


        [HttpPut(Name = "UpdateBus")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateBus(BusDTO dto)
        {
            if (dto == null)
            {
                return BadRequest("BusDTO can't be null.");
            }

            if (dto.BusID <= 0)
            {
                return BadRequest($"Invalid bus ID [{dto.BusID}]");
            }

            if (!await _service.UpdateVehicleAsync(dto.Vehicle))
            {
                return BadRequest($"Failed to update vehicle with ID [{dto.VehicleID}]");
            }

            if (!await _service.UpdateBusAsync(dto))
            {
                return BadRequest($"Failed to update bus with ID [{dto.BusID}]");
            }

            return Ok($"Bus with plate numbers [{dto.Vehicle.PlateNumbers}] updated successfully");
        }


        [HttpDelete("ByID/{busID}", Name = "DeleteBusByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> DeleteBusByID(byte busID)
        {
            if (busID <= 0)
            {
                return BadRequest($"Invalid bus ID [{busID}]");
            }

            if (!await _service.IsBusExists(busID))
            {
                return NotFound($"Bus with ID [{busID}] not found!");
            }

            if (!await _service.DeleteBusAsync(busID))
            {
                return BadRequest($"Can't delete bus with ID [{busID}]");
            }
            
            return Ok($"Bus with ID [{busID}] deleted successfully");
        }


        [HttpDelete("ByPlateNumbers/{plateNumbers}", Name = "DeleteBusByPlateNumbers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> DeleteBusByPlateNumbers(string plateNumbers)
        {
            if (plateNumbers.Length != 6 && plateNumbers.Length != 7)
            {
                return BadRequest($"Invalid plate numbers [{plateNumbers}]");
            }

            if (!await _service.IsBusExists(plateNumbers))
            {
                return NotFound($"Bus with plate numbers [{plateNumbers}] not found!");
            }
            
            if (!await _service.DeleteBusAsync(plateNumbers))
            {
                return BadRequest($"Can't delete bus with plate numbers [{plateNumbers}]");
            }
    
            return Ok($"Bus with plate numbers [{plateNumbers}] deleted successfully");
        }
    }
}
