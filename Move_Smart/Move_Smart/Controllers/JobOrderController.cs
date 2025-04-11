using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Services;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using DataAccessLayer.Repositories;
using System.ComponentModel.DataAnnotations;


namespace Move_Smart.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class JobOrderController : ControllerBase
    {
        private readonly JobOrderService _service;
        private readonly ILogger<JobOrderController> _logger;

        public JobOrderController(JobOrderService service, ILogger<JobOrderController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllJobOrders([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var jobOrders = await _service.GetAllJobOrdersAsync(pageNumber, pageSize);
                return Ok(jobOrders);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error occurred while fetching job orders");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetJobOrderById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid user ID");
            }

            try
            {
                var jobOrder = await _service.GetJobOrderByIdAsync(id);
                return Ok(jobOrder);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpGet("vehicle/{id}")]
        public async Task<IActionResult> GetJobOrdersByVehicleIdAsync(int vehicleId)
        {
            if (vehicleId <= 0)
            {
                return BadRequest("Invalid vehicle ID");
            }

            try
            {
                var jobOrders = await _service.GetJobOrdersByVehicleIdAsync(vehicleId);
                return Ok(jobOrders);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving vehicle");
                return StatusCode(500, "Internal server error");
            }

        }
    }
}
