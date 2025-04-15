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
            if (pageNumber < 1 || pageSize < 1)
            {
                _logger.LogWarning("Invalid pagination parameters.");
                return BadRequest("Page number and page size must be greater than 0.");
            }

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
                return BadRequest("Invalid jobOrder ID");
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
                _logger.LogError(ex, "Error retrieving jobOrder");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpGet("vehicle/{id}")]
        public async Task<IActionResult> GetJobOrdersByVehicleId(int vehicleId)
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
                _logger.LogError(ex, $"Error retrieving jobOrders for vehicle ID {vehicleId}.");
                return StatusCode(500, "Internal server error");
            }

        }

        [HttpGet("driver/{id}")]
        public async Task<IActionResult> GetJobOrdersByDriverId(int driverId)
        {
            if (driverId <= 0)
            {
                return BadRequest("Invalid driver ID");
            }
            try
            {
                var jobOrders = await _service.GetJobOrdersByDriverIdAsync(driverId);
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
                _logger.LogError(ex, $"Error retrieving job orders by driver ID {driverId}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("startdate/{startDate}")]
        public async Task<IActionResult> GetJobOrdersByStartDate(DateTime startDate)
        {
            if (startDate == default)
            {
                return BadRequest("Invalid start date");
            }
            try
            {
                var jobOrders = await _service.GetJobOrdersByStartDateAsync(startDate);
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
                _logger.LogError(ex, "Error retrieving job orders by start date");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("destination/{destination}")]
        public async Task<IActionResult> GetJobOrdersByDestination(string destination)
        {
            if (string.IsNullOrWhiteSpace(destination))
            {
                return BadRequest("Invalid destination");
            }
            try
            {
                var jobOrders = await _service.GetJobOrdersByDestinationAsync(destination);
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
                _logger.LogError(ex, "Error retrieving job orders by destination");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetJobOrdersByStatus(enStatus status)
        {
            if (!Enum.IsDefined(typeof(enStatus), status))
            {
                return BadRequest("Invalid status");
            }

            try
            {
                var jobOrders = await _service.GetJobOrdersByStatusAsync(status);
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
                _logger.LogError(ex, "Error retrieving job orders by status");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("daterange")]
        public async Task<IActionResult> GetJobOrdersByDateRange([Required] DateTime startDate, [Required] DateTime endDate)
        {
            if (!ModelState.IsValid || startDate == default || endDate == default)
            {
                return BadRequest("Invalid date range");
            }
            try
            {
                var jobOrders = await _service.GetJobOrdersByDateRangeAsync(startDate, endDate);
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
                _logger.LogError(ex, "Error retrieving job orders by date range");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateJobOrder([FromBody] JobOrderDTO jobOrder)
        {
            if (jobOrder == null)
                return BadRequest("Job order cannot be null");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var jobOrderId = await _service.CreateJobOrderAsync(jobOrder);
                return CreatedAtAction(nameof(GetJobOrderById), new { id = jobOrderId }, jobOrder);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating job order");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateJobOrder(int id, [FromBody] JobOrderDTO jobOrder)
        {
            if (id <= 0 || jobOrder == null || id != jobOrder.OrderId)
                return BadRequest("Invalid job order ID or job order data");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _service.UpdateJobOrderAsync(jobOrder);
                return NoContent();
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
                _logger.LogError(ex, "Error updating job order");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJobOrder(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid job order ID");
            }

            try
            {
                await _service.DeleteJobOrderAsync(id);
                return NoContent();
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
                _logger.LogError(ex, "Error deleting job order");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
