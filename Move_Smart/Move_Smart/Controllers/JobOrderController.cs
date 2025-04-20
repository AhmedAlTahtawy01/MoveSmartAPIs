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
        public async Task<IActionResult> GetJobOrderById([FromRoute] int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid job order ID.");
                return BadRequest("Invalid jobOrder ID");
            }

            try
            {
                var jobOrder = await _service.GetJobOrderByIdAsync(id);
                return Ok(jobOrder);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error occurred while fetching job order by ID");
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, $"No job order found with ID {id}");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving jobOrder");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpGet("vehicle/{id}")]
        public async Task<IActionResult> GetJobOrdersByVehicleId([FromRoute] int vehicleId)
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
        public async Task<IActionResult> GetJobOrdersByDriverId([FromRoute] int driverId)
        {
            if (driverId <= 0)
            {
                _logger.LogWarning("Invalid driver ID.");
                return BadRequest("Invalid driver ID");
            }
            try
            {
                var jobOrders = await _service.GetJobOrdersByDriverIdAsync(driverId);
                return Ok(jobOrders);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error occurred while fetching job orders by driver ID");
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, $"No job orders found for driver ID {driverId}");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving job orders by driver ID {driverId}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("startdate/{startDate}")]
        public async Task<IActionResult> GetJobOrdersByStartDate([FromRoute] DateTime startDate)
        {
            if (startDate == default)
            {
                _logger.LogWarning("Invalid start date.");
                return BadRequest("Invalid start date");
            }
            try
            {
                var jobOrders = await _service.GetJobOrdersByStartDateAsync(startDate);
                return Ok(jobOrders);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error occurred while fetching job orders by start date");
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, $"No job orders found with start date {startDate}");
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
                _logger.LogWarning("Invalid destination.");
                return BadRequest("Invalid destination");
            }
            try
            {
                var jobOrders = await _service.GetJobOrdersByDestinationAsync(destination);
                return Ok(jobOrders);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error occurred while fetching job orders by destination");
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, $"No job orders found with destination {destination}");
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
                return CreatedAtAction(nameof(GetJobOrderById), new { jobOrderId }, jobOrder);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument provided.");
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning($"Invalid Data {ex.Message}");
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating job order");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateJobOrder([FromRoute] int id, [FromBody] JobOrderDTO jobOrder)
        {
            if (id <= 0 || jobOrder == null || id != jobOrder.OrderId)
                return BadRequest("Invalid job order ID or job order data");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var updated = await _service.UpdateJobOrderAsync(jobOrder);
                return Ok(updated);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error occurred while updating job order");
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, $"No job order found with ID {id} for update");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating job order");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJobOrder([FromRoute] int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid job order ID.");
                return BadRequest("Invalid job order ID");
            }

            try
            {
                await _service.DeleteJobOrderAsync(id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error occurred while deleting job order");
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, $"No job order found with ID {id} for deletion");
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
