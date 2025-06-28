using BusinessLayer;
using DataAccessLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static DataAccessLayer.DriverDTO;

namespace Move_Smart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriversController : ControllerBase
    {
        private readonly DriverService _service;
        private readonly ILogger<DriversController> _logger;

        public DriversController(DriverService service, ILogger<DriversController> logger)
        {
            _service = service;
            _logger = logger;
        }


        [Authorize(Policy = "Drivers")]
        [HttpGet("All", Name = "GetAllDrivers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<DriverDTO>>> GetAllDrivers()
        {
            List<DriverDTO> drivers = await _service.GetAllDriversAsync();

            if (drivers == null || !drivers.Any())
            {
                return NotFound(new { message = "No drivers found." });
            }

            return Ok(drivers);
        }


        [Authorize(Policy = "Drivers")]
        [HttpGet("WorkingOnVehicle/WithID/{vehicleID}", Name = "GetAllDriversWorkingOnVehicleWithID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<DriverDTO>>> GetAllDriversWorkingOnVehicleWithID(short vehicleID)
        {
            if (vehicleID <= 0)
            {
                return BadRequest(new { message = "Invalid vehicle ID!" });
            }

            List<DriverDTO> drivers = await _service.GetAllDriversWorkingOnVehicleAsync(vehicleID);

            if (drivers == null || !drivers.Any())
            {
                return NotFound(new { message = $"No drivers found working on vehicle with ID [{vehicleID}]." });
            }

            return Ok(drivers);
        }


        [Authorize(Policy = "Drivers")]
        [HttpGet("WorkingOnVehicle/WithPlateNumbers/{plateNumbers}", Name = "GetAllDriversWorkingOnVehicleWithPlateNumbers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<DriverDTO>>> GetAllDriversWorkingOnVehicleWithPlateNumbers(string plateNumbers)
        {
            if(plateNumbers.Length != 6 && plateNumbers.Length != 7)
            {
                return BadRequest(new { message = "Invalid plate numbers!" });
            }

            List<DriverDTO> drivers = await _service.GetAllDriversWorkingOnVehicleAsync(plateNumbers);

            if (drivers == null || !drivers.Any())
            {
                return NotFound(new { message = $"No drivers found working on vehicle with plate numbers [{plateNumbers}]." });
            }

            return Ok(drivers);
        }


        [Authorize(Policy = "Drivers")]
        [HttpGet("WithStatus/{status}", Name = "GetAllDriversWithStatus")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<DriverDTO>>> GetAllDriversWithStatus(enDriverStatus status)
        {
            List<DriverDTO> drivers = await _service.GetAllDriversWithStatusAsync(status);

            if (drivers == null || !drivers.Any())
            {
                return NotFound(new { message = $"No drivers found with status [{status}]." });
            }

            return Ok(drivers);
        }


        [Authorize(Policy = "Drivers")]
        [HttpGet("ByID/{driverID}", Name = "GetDriverByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DriverDTO>> GetDriverByID(int driverID)
        {
            if (driverID <= 0)
            {
                return BadRequest(new { message = "Invalid driver ID!" });
            }

            DriverDTO? driver = await _service.GetDriverByIDAsync(driverID);

            if (driver == null)
            {
                return NotFound(new { message = $"Driver with ID [{driverID}] not found." });
            }

            return Ok(driver);
        }


        [Authorize(Policy = "Drivers")]
        [HttpGet("ByNationalNo/{nationalNo}", Name = "GetDriverByNationalNo")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DriverDTO>> GetDriverByNationalNo(string nationalNo)
        {
            if (nationalNo.Length != 14 || nationalNo.Any(ch => char.IsLetter(ch)))
            {
                return BadRequest(new { message = "Invalid national number!" });
            }

            DriverDTO? driver = await _service.GetDriverByNationalNoAsync(nationalNo);

            if (driver == null)
            {
                return NotFound(new { message = $"Driver with national numbers [{nationalNo}] not found." });
            }

            return Ok(driver);
        }


        [Authorize(Policy = "Drivers")]
        [HttpGet("ByPhone/{phone}", Name = "GetDriverByPhone")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DriverDTO>> GetDriverByPhone(string phone)
        {
            if (phone.Length != 11 || phone.Any(ch => char.IsLetter(ch)))
            {
                return BadRequest(new { message = "Invalid phone number!" });
            }

            DriverDTO? driver = await _service.GetDriverByPhoneAsync(phone);

            if (driver == null)
            {
                return NotFound(new { message = $"Driver with phone number [{phone}] not found." });
            }

            return Ok(driver);
        }


        [Authorize(Policy = "Drivers")]
        [HttpGet("Count", Name = "GetNumberOfDrivers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<short>> GetNumberOfDrivers()
        {
            short numberOfDrivers = await _service.GetNumberOfDriversAsync();

            if (numberOfDrivers == 0)
            {
                return NotFound(new { message = "No Drivers Found!" });
            }

            return Ok(numberOfDrivers);
        }


        [Authorize(Policy = "Drivers")]
        [HttpGet("Count/WithStatus/{status}", Name = "GetNumberOfDriversByStatus")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<short>> GetNumberOfDriversByStatus(enDriverStatus status)
        {
            short numberOfDrivers = await _service.GetNumberOfDriversByStatusAsync(status);

            if (numberOfDrivers == 0)
            {
                return NotFound(new { message = $"No Drivers Found With Status [{status}]!" });
            }

            return Ok(numberOfDrivers);
        }


        [Authorize(Policy = "RequireGeneralManager")]
        [HttpPost(Name = "AddNewDriver")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DriverDTO>> AddNewDriver(DriverDTO dto)
        {
            if (dto == null)
            {
                return BadRequest(new { message = "DriverDTO cannot be null." });
            }

            if (await _service.IsDriverExistsAsync(dto.NationalNo))
            {
                return BadRequest(new { message = $"Driver with NationalNo [{dto.NationalNo}] already exists." });
            }

            if (await _service.AddNewDriverAsync(dto) == null)
            {
                return BadRequest(new { message = "Failed to add new driver." });
            }

            return CreatedAtRoute("GetDriverByID", new { driverID = dto.DriverID }, dto);
        }


        [Authorize(Policy = "RequireGeneralSupervisor")]
        [HttpPut(Name = "UpdateDriver")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateDriver(DriverDTO dto)
        {
            if (dto == null)
            {
                return BadRequest(new { message = "Driver DTO can't be null!" });
            }

            if (dto.DriverID <= 0)
            {
                return BadRequest(new { message = "Invalid driver ID!" });
            }

            if (!await _service.IsDriverExistsAsync(dto.DriverID ?? 0))
            {
                return BadRequest(new { message = $"Driver with ID [{dto.DriverID}] does not exist." });
            }

            if (!await _service.UpdateDriverAsync(dto))
            {
                return BadRequest(new { message = $"Failed to update driver with ID [{dto.DriverID}]." });
            }

            return Ok(new { message = $"Driver with ID [{dto.DriverID}] updated successfully." });
        }


        [Authorize(Policy = "RequireGeneralManager")]
        [HttpDelete("ByID/{driverID}", Name = "DeleteDriverByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> DeleteDriverByID(int driverID)
        {
            if (driverID <= 0)
            {
                return BadRequest(new { message = "Invalid driver ID!" });
            }

            if (!await _service.IsDriverExistsAsync(driverID))
            {
                return NotFound(new { message = $"Driver with ID [{driverID}] not found." });
            }
            
            if (!await _service.DeleteDriverAsync(driverID))
            {
                return BadRequest(new { message = $"Failed to delete driver with ID [{driverID}]." });
            }

            return Ok(new { message = $"Driver with ID [{driverID}] deleted successfully." });
        }


        [Authorize(Policy = "RequireGeneralManager")]
        [HttpDelete("ByNationalNo/{nationalNo}", Name = "DeleteDriverByNationalNo")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> DeleteDriverByNationalNo(string nationalNo)
        {
            if (nationalNo.Length != 14 || nationalNo.Any(ch => char.IsLetter(ch)))
            {
                return BadRequest(new { message = "Invalid national number!" });
            }

            if (!await _service.IsDriverExistsAsync(nationalNo))
            {
                return NotFound(new { message = $"Driver with NationalNo [{nationalNo}] not found." });
            }
            
            if (!await _service.DeleteDriverAsync(nationalNo))
            {
                return BadRequest(new { message = $"Failed to delete driver with NationalNo [{nationalNo}]." });
            }
            
            return Ok(new { message = $"Driver with NationalNo [{nationalNo}] deleted successfully." });
        }
    }
}
