using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Services;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using DataAccessLayer.Repositories;
using System.ComponentModel.DataAnnotations;
using BusinessLogicLayer.Services;
using Microsoft.AspNetCore.Authorization;
using Move_Smart.Models;

namespace Move_Smart.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ApplicationController : ControllerBase
    {
        private readonly ApplicationService _service;
        private readonly ILogger<ApplicationController> _logger;

        public ApplicationController(ApplicationService service, ILogger<ApplicationController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [Authorize (Policy = "RequireGeneralSupervisor")]
        [HttpGet]
        public async Task<IActionResult> GetAllApplications([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var applications = await _service.GetAllApplicationsAsync(pageNumber, pageSize);
                return Ok(applications);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error occurred while fetching applications");
                return BadRequest(new { message = "Error occurred while fetching all applications, Check the parameters." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred");
                return StatusCode(500, new { message = "Internal server error." });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetApplicationById([FromRoute] int id)
        {
            try
            {
                var application = await _service.GetApplicationByIdAsync(id);
                return Ok(application);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error occurred while fetching application");
                return BadRequest(new { message = "Error occurred while fetching application, Check the parameters." });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, "Application not found");
                return NotFound(new { message = "Application Not found." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("type/{type}")]
        public async Task<IActionResult> GetApplicationsByType([FromRoute] enApplicationType type)
        {
            try
            {
                var applications = await _service.GetApplicationsByApplicationTypeAsync(type);
                return Ok(applications);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error occurred while fetching applications");
                return BadRequest(new { message = "Error occurred while fetching applications by type, Check the parameters." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetApplicationsByUserId([FromRoute] int userId)
        {
            try
            {
                var applications = await _service.GetApplicationsByUserIdAsync(userId);
                return Ok(applications);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error occurred while fetching applications");
                return BadRequest(new { message = "Error occurred while fetching applications by user Id, Check the parameters." });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, "User not found");
                return NotFound(new { message = "User Not found." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetApplicationsByStatus([FromRoute] enStatus status)
        {
            try
            {
                var applications = await _service.GetApplicationsByStatusAsync(status);
                return Ok(applications);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error occurred while fetching applications");
                return BadRequest(new { message = "Error occurred while fetching applications by status, Check the parameters." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("count/type/{type}")]
        public async Task<IActionResult> CountApplicationsByType([FromRoute] enApplicationType type)
        {
            try
            {
                var count = await _service.CountApplicationsByTypeAsync(type);
                return Ok(new { count, type });
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error occurred while counting applications");
                return BadRequest(new { message = "Error occurred while counting applications by type, Check the parameters." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("count/status/{status}")]
        public async Task<IActionResult> CountApplicationsByStatus([FromRoute] enStatus status) 
        {
            try
            {
                var count = await _service.CountApplicationsByStatusAsync(status);
                return Ok(new { count, status });
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error occurred while counting applications");
                return BadRequest(new { message = "Error occurred while counting applications by status, Check the parameters." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [Authorize(Policy = "RequireGeneralSupervisor")]
        [HttpGet("count")]
        public async Task<IActionResult> CountAllApplications()
        {
            try
            {
                var count = await _service.CountAllApplicationsAsync();
                return Ok(new { count });
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error occurred while counting applications");
                return BadRequest(new { message = "Error occurred while counting applications, Check the parameters." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
    }
}