
using BusinessLayer;
using DataAccessLayer;
using Microsoft.AspNetCore.Mvc;

namespace Move_Smart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VacationsController : ControllerBase
    {
        private readonly ILogger<VacationsController> _logger;
        private readonly VacationService _service;

        public VacationsController(ILogger<VacationsController> logger, VacationService service)
        {
            _logger = logger;
            _service = service;
        }


        [HttpGet("All", Name = "GetAllVacations")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<VacationDTO>>> GetAllVacations()
        {
            List<VacationDTO> vacations = await _service.GetAllVacationsAsync();

            if (vacations == null || !vacations.Any())
            {
                return NotFound("No vacations found.");
            }

            return Ok(vacations);
        }


        [HttpGet("ForDriver/{driverID}", Name = "GetAllVacationsForDriver")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<VacationDTO>>> GetAllVacationsForDriver(int driverID)
        {
            List<VacationDTO> vacations = await _service.GetAllVacationsForDriverAsync(driverID);

            if (vacations == null || !vacations.Any())
            {
                return NotFound($"No vacations found for driver with ID {driverID}.");
            }

            return Ok(vacations);
        }


        [HttpGet("ValidForDriver/{driverID}", Name = "GetAllValidVacationsForDriver")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<VacationDTO>>> GetAllValidVacationsForDriver(int driverID)
        {
            List<VacationDTO> vacations = await _service.GetAllValidVacationsForDriverAsync(driverID);

            if (vacations == null || !vacations.Any())
            {
                return NotFound($"No valid vacations found for driver with ID {driverID}.");
            }

            return Ok(vacations);
        }


        [HttpGet("{vacationID}", Name = "GetVacationByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VacationDTO>> GetVacationByID(int vacationID)
        {
            VacationDTO? vacation = await _service.GetVacationByIDAsync(vacationID);

            if (vacation == null)
            {
                return NotFound($"Vacation with ID {vacationID} not found.");
            }
            
            return Ok(vacation);
        }


        [HttpPost(Name = "AddNewVacation")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<VacationDTO>> AddNewVacation(VacationDTO dto)
        {
            if (dto == null)
            {
                return BadRequest("VacationDTO cannot be null!");
            }

            if (await _service.AddNewVacationAsync(dto) == null)
            {
                return BadRequest("Failed to add new vacation.");
            }

            return CreatedAtRoute("GetVacationByID", new { vacationID = dto.VacationID }, dto);
        }


        [HttpPut(Name = "UpdateVacation")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateVacation(VacationDTO dto)
        {
            if (dto == null)
            {
                return BadRequest("VacationDTO cannot be null!");
            }

            if (dto.VacationID <= 0)
            {
                return BadRequest($"Invalid ID [{dto.VacationID}]!");
            }
            
            if (!await _service.IsVacationExistsAsync(dto.VacationID ?? 0))
            {
                return NotFound($"Vacation with ID {dto.VacationID} not found!");
            }

            if (!await _service.UpdateVacationAsync(dto))
            {
                return BadRequest($"Failed to update vacation with ID [{dto.VacationID}]!");
            }

            return Ok($"Vacation with ID [{dto.VacationID}] updaed successfully.");
        }


        [HttpDelete("{vacationID}", Name = "DeleteVacation")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> DeleteVacation(int vacationID)
        {
            if (vacationID <= 0)
            {
                return BadRequest($"Invalid ID [{vacationID}]!");
            }

            if (!await _service.IsVacationExistsAsync(vacationID))
            {
                return NotFound($"Vacation with ID {vacationID} not found!");
            }
            
            if (!await _service.DeleteVacationAsync(vacationID))
            {
                return BadRequest($"Failed to delete vacation with ID [{vacationID}]!");
            }
            
            return Ok($"Vacation with ID [{vacationID}] deleted successfully.");
        }


        [HttpGet("IsDriverInVacation/{driverID}", Name = "IsDriverInVacation")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<bool>> IsDriverInVacation(int driverID)
        {
            if (driverID <= 0)
            {
                return BadRequest($"Invalid ID [{driverID}]!");
            }
            
            return await _service.IsDriverInVacationAsync(driverID) ? Ok(true) : Ok(false);
        }
    }
}
