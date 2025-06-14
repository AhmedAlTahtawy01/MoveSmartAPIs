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
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserService _service;
        private readonly ITokenService _tokenService;
        private readonly ILogger<UserController> _logger;

        public UserController(UserService service, ITokenService tokenService, ILogger<UserController> logger)
        {
            _service = service;
            _tokenService = tokenService;
            _logger = logger;
        }

        [Authorize(Policy = "RequireHospitalManager")]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var users = await _service.GetAllUsersAsync(pageNumber, pageSize);
                return Ok(users);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error occurred while fetching users");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred");
                return StatusCode(500, "Internal server error");
            }
        }

        [Authorize(Policy = "RequireHospitalManager")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById([FromRoute] int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid user ID: {Id}", id);
                return BadRequest("Invalid user ID");
            }

            try
            {
                var user = await _service.GetUserByIdAsync(id);
                return Ok(user);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error occurred while fetching user with ID: {Id}", id);
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "User not found with ID: {Id}", id);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user");
                return StatusCode(500, "Internal server error");
            }
        }

        [Authorize(Policy = "RequireGeneralManager")]
        [HttpGet("count")]
        public async Task<IActionResult> CountUsers()
        {
            try
            {
                int count = await _service.CountUsersAsync();
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting users.");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [Authorize(Policy = "RequireHospitalManager")]
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserDTO user)
        {
            try
            {
                int userId = await _service.CreateUserAsync(user);
                return CreatedAtAction(nameof(GetUserById), new { id = userId }, user);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user.");
                return StatusCode(500, "Internal server error");
            }
        }

        [Authorize]
        [HttpPut("password/{id}")]
        public async Task<IActionResult> ChangeUserPassword(int id, [FromBody] ChangePasswordModel model)
        {
            if (id <= 0 || model == null)
            {
                _logger.LogWarning("User Id must be greater than 0.");
                return BadRequest("User Id mismatch");
            }

            try
            {
                await _service.UpdateUserPasswordAsync(id, model.OldPassword, model.NewPassword);
                return Ok(new { message = "Password updated successfully" });
            }

            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error updating user password");
                return BadRequest( new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access while updating user password");
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user password");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserInfo(int id, [FromBody] UserDTO user)
        {
            if (id <= 0 || user == null || id != user.UserId)
            {
                _logger.LogWarning("User Id mismatch: {Id} vs {UserId}", id, user?.UserId);
                return BadRequest("User Id mismatch");
            }

            try
            {
                await _service.UpdateUserInfoAsync(user);
                return NoContent();
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "Error updating user");
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error updating user");
                return Conflict(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "User not found with ID: {Id}", id);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user");
                return StatusCode(500, "Internal server error");
            }
        }

        [Authorize(Roles = "SuperUser")]
        [HttpPut("all/{id}")]
        public async Task<IActionResult> UpdateAllUserInfo(int id, [FromBody] UserDTO user)
        {
            if (id <= 0 || user == null || id != user.UserId)
            {
                _logger.LogWarning("User Id mismatch: {Id} vs {UserId}", id, user?.UserId);
                return BadRequest("User Id mismatch");
            }
            try
            {

                await _service.UpdateAllUserInfoAsync(user);
                return NoContent();
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "Error updating user");
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error updating user");
                return Conflict(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "User not found with ID: {Id}", id);
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access while updating user");
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user");
                return StatusCode(500, "Internal server error");
            }
        }

        [Authorize(Policy = "RequireHospitalManager")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid user ID");
            }

            try
            {
                bool deleted = await _service.DeleteUserAsync(id);
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
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user");
                return StatusCode(500, "Internal server error");
            }
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid login model state: {ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            try
            {
                var user = await _service.LoginAsync(loginModel.NationalNo, loginModel.Password);

                var jwt = _tokenService.GenerateToken(user);

                var response = new LoginResponseModel
                {
                    Token = jwt,
                    UserId = user.UserId,
                    Name = user.Name,
                    Role = user.Role.ToString()
                };
                
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error in Arguments.");
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Unauthorized.");
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging in user");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}

