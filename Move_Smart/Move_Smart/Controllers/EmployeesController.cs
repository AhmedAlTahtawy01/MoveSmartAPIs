using BusinessLayer;
using DataAccessLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Move_Smart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly EmployeeService _service;
        private readonly ILogger<EmployeesController> _logger;

        public EmployeesController(EmployeeService service, ILogger<EmployeesController> logger)
        {
            _service = service;
            _logger = logger;
        }


        [Authorize(Policy = "RequirePatrolsSupervisor")]
        [HttpGet("All", Name = "GetAllEmployees")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<EmployeeDTO>>> GetAllEmployees()
        {
            List<EmployeeDTO> employees = await _service.GetAllEmployeesAsync();

            if (employees == null || !employees.Any())
            {
                return NotFound(new { message = "No employees found!" });
            }

            return Ok(employees);
        }


        [Authorize(Policy = "RequirePatrolsSupervisor")]
        [HttpGet("WhoAreUsingBus/{busID}", Name = "GetAllEmployeesWhoAreUsingBus")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<EmployeeDTO>>> GetAllEmployeesWhoAreUsingBus(byte busID)
        {
            List<EmployeeDTO> employees = await _service.GetAllEmployeesWhoAreUsingBusAsync(busID);

            if (employees == null || !employees.Any())
            {
                return NotFound(new { message = $"No employees found using bus with ID [{busID}]!" });
            }

            return Ok(employees);
        }


        [Authorize(Policy = "RequirePatrolsSupervisor")]
        [HttpGet("ByID/{employeeID}", Name = "GetEmployeeByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<EmployeeDTO>> GetEmployeeByID(int employeeID)
        {
            if(employeeID <= 0)
            {
                return BadRequest(new { message = $"Invalid ID [{employeeID}]" });
            }

            EmployeeDTO? employee = await _service.GetEmployeeByIDAsync(employeeID);
            
            if (employee == null)
            {
                return NotFound(new { message = $"No employee found with ID [{employeeID}]!" });
            }
     
            return Ok(employee);
        }


        [Authorize(Policy = "RequirePatrolsSupervisor")]
        [HttpGet("ByNationalNo/{nationalNo}", Name = "GetEmployeeByNationalNo")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<EmployeeDTO>> GetEmployeeByNationalNo(string nationalNo)
        {
            if (nationalNo.Length != 14 || nationalNo.Any(ch => char.IsLetter(ch)))
            {
                return BadRequest(new { message = $"Invalid NationalNo [{nationalNo}]" });
            }

            EmployeeDTO? employee = await _service.GetEmployeeByNationalNoAsync(nationalNo);

            if (employee == null)
            {
                return NotFound(new { message = $"No employee found with NationalNo [{nationalNo}]!" });
            }

            return Ok(employee);
        }


        [Authorize(Policy = "RequirePatrolsSupervisor")]
        [HttpGet("ByPhone/{phone}", Name = "GetEmployeeByPhone")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<EmployeeDTO>> GetEmployeeByPhone(string phone)
        {
            if (phone.Length != 11 || phone.Any(ch => char.IsLetter(ch)))
            {
                return BadRequest(new { message = $"Invalid Phone Number [{phone}]" });
            }

            EmployeeDTO? employee = await _service.GetEmployeeByPhoneAsync(phone);

            if (employee == null)
            {
                return NotFound(new { message = $"No employee found with phone number [{phone}]!" });
            }

            return Ok(employee);
        }


        [Authorize(Policy = "RequirePatrolsSupervisor")]
        [HttpGet("/IsTransportationSubscriptionValid/{employeeID}", Name = "IsTransportationSubscriptionValid")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<bool>> IsTransportationSubscriptionValid(int employeeID)
        {
            if (employeeID <= 0)
            {
                return BadRequest(new { message = $"Invalid Employee ID [{employeeID}]" });
            }
            
            if (!await _service.IsTransportationSubscriptionValidAsync(employeeID))
            {
                return NotFound(new { message = $"Transportation subscription for employee with ID [{employeeID}] is not valid!" });
            }
            
            return Ok(true);
        }


        [Authorize(Policy = "RequirePatrolsSupervisor")]
        [HttpPost(Name = "AddNewEmployee")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<EmployeeDTO>> AddNewEmployee(EmployeeDTO dto)
        {
            if (dto == null)
            {
                return BadRequest(new { message = "EmployeeDTO cannot be null!" });
            }

            if (await _service.IsEmployeeExistsAsync(dto.NationalNo))
            {
                return BadRequest(new { message = $"Employee with NationalNo [{dto.NationalNo}] already exists!" });
            }

            if(await _service.AddNewEmployeeAsync(dto) == null)
            {
                return BadRequest(new { message = "Failed to add new employee!" });
            }

            return CreatedAtRoute("GetEmployeeByID", new { employeeID = dto.EmployeeID }, dto);
        }


        [Authorize(Policy = "RequirePatrolsSupervisor")]
        [HttpPut(Name = "UpdateEmployee")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EmployeeDTO>> UpdateEmployee(EmployeeDTO dto)
        {
            if (dto == null)
            {
                return BadRequest(new { message = "EmployeeDTO cannot be null!" });
            }

            if (dto.EmployeeID <= 0)
            {
                return BadRequest(new { message = $"Invalid Employee ID [{dto.EmployeeID}]" });
            }

            if(!await _service.IsEmployeeExistsAsync(dto.EmployeeID ?? 0))
            {
                return NotFound(new { message = $"No employee found with ID [{dto.EmployeeID}]!" });
            }

            if (!await _service.UpdateEmployeeAsync(dto))
            {
                return BadRequest(new { message = $"Failed to update employee with ID [{dto.EmployeeID}]!" });
            }

            return Ok(new { message = $"Employee with ID [{dto.EmployeeID}] updated successfully" });
        }


        [Authorize(Policy = "RequirePatrolsSupervisor")]
        [HttpDelete("ByID/{employeeID}", Name = "DeleteEmployeeByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> DeleteEmployeeByID(int employeeID)
        {
            if (employeeID <= 0)
            {
                return BadRequest(new { message = $"Invalid Employee ID [{employeeID}]" });
            }

            if (!await _service.IsEmployeeExistsAsync(employeeID))
            {
                return NotFound(new { message = $"No employee found with ID [{employeeID}]!" });
            }
            
            if (!await _service.DeleteEmployeeAsync(employeeID))
            {
                return NotFound(new { message = $"Failed to delete employee with ID [{employeeID}]!" });
            }
            
            return Ok(new { message = $"Employee with ID [{employeeID}] deleted successfully" });
        }


        [Authorize(Policy = "RequirePatrolsSupervisor")]
        [HttpDelete("ByNationalNo/{nationalNo}", Name = "DeleteEmployeeByNationalNo")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> DeleteEmployeeByNationalNo(string nationalNo)
        {
            if (nationalNo.Length != 14 || nationalNo.Any(ch => char.IsLetter(ch)))
            {
                return BadRequest(new { message = $"Invalid NationalNo [{nationalNo}]" });
            }

            if (!await _service.IsEmployeeExistsAsync(nationalNo))
            {
                return NotFound(new { message = $"No employee found with NationalNo [{nationalNo}]!" });
            }
            
            if (!await _service.DeleteEmployeeAsync(nationalNo))
            {
                return NotFound(new { message = $"Failed to delete employee with NationalNo [{nationalNo}]!" });
            }
            
            return Ok(new { message = $"Employee with NationalNo [{nationalNo}] deleted successfully" });
        }
    }
}
