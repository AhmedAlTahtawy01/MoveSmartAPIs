﻿using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Services;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using DataAccessLayer.Repositories;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Move_Smart.Models;


namespace Move_Smart.Controllers
{
    [Authorize]
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

        [Authorize(Policy = "RequireAdministrativeSupervisor")]
        [HttpGet]
        public async Task<IActionResult> GetJobOrders([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                _logger.LogWarning("Invalid pagination parameters.");
                return BadRequest(new { message = "Page number and page size must be greater than 0." });
            }

            try
            {
                var jobOrders = await _service.GetAllJobOrdersAsync(pageNumber, pageSize);
                return Ok(jobOrders);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error occurred while fetching job orders");
                return BadRequest(new { message = "Error occurred while fetching Job Orders, Check the parameters." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [Authorize(Policy = "RequireAdministrativeSupervisor")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetJobOrderById([FromRoute] int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid job order ID.");
                return BadRequest(new { message = "Invalid jobOrder ID" });
            }

            try
            {
                var jobOrder = await _service.GetJobOrderByIdAsync(id);
                return Ok(jobOrder);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error occurred while fetching job order by ID");
                return BadRequest(new { message = "Error occurred while fetching job order by ID, Check the parameters." });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, $"No job order found with ID {id}");
                return NotFound(new { message = $"No job order found with ID {id}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving jobOrder");
                return StatusCode(500, new { message = "Internal server error." });
            }
        }

        [Authorize(Policy = "RequireAdministrativeSupervisor")]
        [HttpGet("vehicle/{vehicleId}")]
        public async Task<IActionResult> GetJobOrdersByVehicleId([FromRoute] int vehicleId)
        {
            if (vehicleId <= 0)
            {
                _logger.LogWarning("Invalid vehicle ID.");
                return BadRequest(new { message = "Invalid vehicle ID" });
            }

            try
            {
                var jobOrders = await _service.GetJobOrdersByVehicleIdAsync(vehicleId);
                return Ok(jobOrders);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error occurred while fetching job orders for vehicle ID");
                return BadRequest(new { message = "Error occurred while fetching job orders for vehicle ID, Check the parameters." });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, $"No job orders found for vehicle ID {vehicleId}");
                return NotFound(new { message = $"No job orders found for vehicle ID {vehicleId}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving jobOrders for vehicle ID {vehicleId}.");
                return StatusCode(500, new { message = "Internal server error" });
            }

        }

        [Authorize(Policy = "RequireAdministrativeSupervisor")]
        [HttpGet("driver/{driverId}")]
        public async Task<IActionResult> GetJobOrdersByDriverId([FromRoute] int driverId)
        {
            if (driverId <= 0)
            {
                _logger.LogWarning("Invalid driver ID.");
                return BadRequest(new { message = "Invalid driver ID" });
            }
            try
            {
                var jobOrders = await _service.GetJobOrdersByDriverIdAsync(driverId);
                return Ok(jobOrders);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error occurred while fetching job orders by driver ID");
                return BadRequest(new { message = "Error occurred while fetching job orders by driver ID, Check the parameters." });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, $"No job orders found for driver ID {driverId}");
                return NotFound(new { message = $"No job orders found for driver ID {driverId}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving job orders by driver ID {driverId}");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [Authorize(Policy = "RequireAdministrativeSupervisor")]
        [HttpGet("startdate/{startDate}")]
        public async Task<IActionResult> GetJobOrdersByStartDate([FromRoute] DateTime startDate)
        {
            if (startDate == default)
            {
                _logger.LogWarning("Invalid start date.");
                return BadRequest(new { message = "Invalid start date" });
            }
            try
            {
                var jobOrders = await _service.GetJobOrdersByStartDateAsync(startDate);
                return Ok(jobOrders);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error occurred while fetching job orders by start date");
                return BadRequest(new { message = "Error occurred while fetching job orders by start date, Check the parameters." });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, $"No job orders found with start date {startDate}");
                return NotFound(new { message = $"No job orders found with start date {startDate}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving job orders by start date");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [Authorize(Policy = "RequireAdministrativeSupervisor")]
        [HttpGet("destination/{destination}")]
        public async Task<IActionResult> GetJobOrdersByDestination(string destination)
        {
            if (string.IsNullOrWhiteSpace(destination))
            {
                _logger.LogWarning("Invalid destination.");
                return BadRequest(new { message = "Invalid destination" });
            }
            try
            {
                var jobOrders = await _service.GetJobOrdersByDestinationAsync(destination);
                return Ok(jobOrders);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error occurred while fetching job orders by destination");
                return BadRequest(new { message = "Error occurred while fetching job orders by destination, Check the parameters." });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, $"No job orders found with destination {destination}");
                return NotFound(new { message = $"No job orders found with destination {destination}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving job orders by destination");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [Authorize(Policy = "RequireAdministrativeSupervisor")]
        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetJobOrdersByStatus(enStatus status)
        {
            if (!Enum.IsDefined(typeof(enStatus), status))
            {
                _logger.LogWarning("Invalid status.");
                return BadRequest(new { message = "Invalid status" });
            }

            try
            {
                var jobOrders = await _service.GetJobOrdersByStatusAsync(status);
                return Ok(jobOrders);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error occurred while fetching job orders by status");
                return BadRequest(new { message = "Error occurred while fetching job orders by status, Check the parameters." });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, $"No job orders found with status {status}");
                return NotFound(new { message = $"No job orders found with status {status}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving job orders by status");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [Authorize(Policy = "RequireAdministrativeSupervisor")]
        [HttpGet("daterange")]
        public async Task<IActionResult> GetJobOrdersByDateRange([FromBody] DataRangeModel dateRange)
        {
            if (!ModelState.IsValid || dateRange.StartDate == default || dateRange.EndDate == default)
            {
                return BadRequest(new { message = "Invalid date range" });
            }
            try
            {
                var jobOrders = await _service.GetJobOrdersByDateRangeAsync(dateRange.StartDate, dateRange.EndDate);
                return Ok(jobOrders);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error occurred while fetching job orders by date range");
                return BadRequest(new { message = "Error occurred while fetching job orders by date range, Check the parameters." });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, $"No job orders found in the date range {dateRange.StartDate} to {dateRange.EndDate}");
                return NotFound(new { message = $"No job orders found in the date range {dateRange.StartDate} to {dateRange.EndDate}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving job orders by date range");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [Authorize(Policy = "RequireAdministrativeSupervisor")]
        [HttpGet("application/{applicationId}")]
        public async Task<IActionResult> GetJobOrdersByApplicationId([FromRoute] int applicationId)
        {
            try
            {
                var jobOrders = await _service.GetJobOrdersByApplicationIdAsync(applicationId);
                return Ok(jobOrders);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error occurred while fetching job orders by application ID");
                return BadRequest(new { message = "Error occurred while fetching job orders by application ID, Check the parameters." });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, $"No job orders found for application ID {applicationId}");
                return NotFound(new { message = $"No job orders found for application ID {applicationId}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving job orders by application ID");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [Authorize(Policy = "AdministrativeSupervisor")]
        [HttpPost]
        public async Task<IActionResult> CreateJobOrder([FromBody] JobOrderDTO jobOrder)
        {
            if (jobOrder == null)
            {
                _logger.LogWarning("Received null job order data.");
                return BadRequest(new { message = "Job order cannot be null" });
            }
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var jobOrderId = await _service.CreateJobOrderAsync(jobOrder);
                jobOrder.OrderId = jobOrderId; // Set the ID of the created job order
                return CreatedAtAction(nameof(GetJobOrderById), new { id = jobOrderId }, jobOrder);
            }
            // this is the catch statement in the ..'s class ..
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument provided.");
                return BadRequest(new { message = "Invalid argument provided." });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Key not found during job order creation.");
                return NotFound(new { message = "Key not found during job order creation." });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning($"Invalid Data {ex.Message}");
                return Conflict(new { message = $"Invalid Data {ex.Message}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating job order");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [Authorize(Policy = "AdministrativeSupervisor")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateJobOrder([FromRoute] int id, [FromBody] JobOrderDTO jobOrder)
        {
            if (id <= 0 || jobOrder == null || id != jobOrder.OrderId)
                return BadRequest(new { message = "Invalid job order ID or job order data" });

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
                return BadRequest(new { message = "Error occurred while updating job order" });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, $"No job order found with ID {id} for update");
                return NotFound(new { message = $"No job order found with ID {id} for update" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating job order");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [Authorize(Policy = "AdministrativeSupervisor")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJobOrder([FromRoute] int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid job order ID.");
                return BadRequest(new { message = "Invalid job order ID" });
            }

            try
            {
                await _service.DeleteJobOrderAsync(id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error occurred while deleting job order");
                return BadRequest(new { message = "Error occurred while deleting job order" });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, $"No job order found with ID {id} for deletion");
                return NotFound(new { message = $"No job order found with ID {id} for deletion" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting job order");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
    }
}
