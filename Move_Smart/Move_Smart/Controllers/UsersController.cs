//using Microsoft.AspNetCore.Mvc;
//using BusinessLayer.Services;
//using System.Threading.Tasks;
//using System.Collections.Generic;
//using System;
//using DataAccessLayer.Repositories;
//using Microsoft.Extensions.Logging;

//namespace Move_Smart.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class UsersController : ControllerBase
//    {
//        private readonly UserRepo _userRepo;
//        private readonly ILogger<UserService> _logger;

//        public UsersController(UserRepo userRepo, ILogger<UserService> logger)
//        {
//            _userRepo = userRepo ?? throw new ArgumentNullException(nameof(userRepo));
//            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
//        }

//        [HttpPost("login")]
//        public async Task<IActionResult> Login([FromBody] LoginRequest request)
//        {
//            try
//            {
//                var userService = await new UserService(
//                    new UserDTO(0, request.NationalNo, "", "", EnUserRole.SuperUser, 0),
//                    _userRepo,
//                    _logger
//                ).LoginAsync(request.NationalNo, request.Password);

//                return Ok(new { UserId = userService.UserId, Role = userService.Role });
//            }
//            catch (Exception ex)
//            {
//                return BadRequest(ex.Message);
//            }
//        }

//        [HttpPost]
//        public async Task<IActionResult> CreateUser([FromBody] UserCreateRequest request)
//        {
//            try
//            {
//                var userDTO = new UserDTO(0, request.NationalNo, request.Password, request.Name, request.Role, 0);
//                var userService = new UserService(userDTO, _userRepo, _logger, UserService.EnMode.AddNew);
//                var result = await userService.SaveAsync();
//                return result ? Ok(new { UserId = userService.UserId }) : BadRequest("Failed to create user");
//            }
//            catch (Exception ex)
//            {
//                return BadRequest(ex.Message);
//            }
//        }

//        [HttpPut("{userId}")]
//        public async Task<IActionResult> UpdateUser(int userId, [FromBody] UserUpdateRequest request)
//        {
//            try
//            {
//                var existingUser = await _userRepo.GetUserByIdAsync(userId);
//                if (existingUser == null)
//                    return NotFound("User not found");

//                var userDTO = new UserDTO(
//                    userId,
//                    request.NationalNo,
//                    request.Password,
//                    request.Name,
//                    existingUser.Role,
//                    existingUser.AccessRight
//                );
//                var userService = new UserService(userDTO, _userRepo, _logger, UserService.EnMode.Update);
//                var result = await userService.SaveAsync();
//                return result ? Ok() : BadRequest("Failed to update user");
//            }
//            catch (Exception ex)
//            {
//                return BadRequest(ex.Message);
//            }
//        }

//        [HttpGet]
//        public async Task<IActionResult> GetAllUsers(int pageNumber = 1, int pageSize = 10)
//        {
//            try
//            {
//                var tempUserService = new UserService(
//                    new UserDTO(0, "", "", "", EnUserRole.GeneralSupervisor, 0),
//                    _userRepo,
//                    _logger
//                );
//                var users = await tempUserService.GetAllUsersAsync(pageNumber, pageSize);
//                var response = users.Select(u => new { u.UserId, u.NationalNo, u.Name, u.Role });
//                return Ok(response);
//            }
//            catch (Exception ex)
//            {
//                return BadRequest(ex.Message);
//            }
//        }

//        [HttpGet("{userId}")]
//        public async Task<IActionResult> GetUserById(int userId)
//        {
//            try
//            {
//                var tempUserService = new UserService(
//                    new UserDTO(0, "", "", "", EnUserRole.GeneralSupervisor, 0),
//                    _userRepo,
//                    _logger
//                );
//                var user = await tempUserService.GetUserByIdAsync(userId);
//                if (user == null)
//                    return NotFound();

//                return Ok(new { user.UserId, user.NationalNo, user.Name, user.Role });
//            }
//            catch (Exception ex)
//            {
//                return BadRequest(ex.Message);
//            }
//        }

//        [HttpDelete("{userId}")]
//        public async Task<IActionResult> DeleteUser(int userId)
//        {
//            try
//            {
//                var tempUserService = new UserService(
//                    new UserDTO(0, "", "", "", EnUserRole.SuperUser, (int)EnPermissions.All),
//                    _userRepo,
//                    _logger
//                );
//                var result = await tempUserService.DeleteUserAsync(userId);
//                return result ? Ok() : NotFound();
//            }
//            catch (Exception ex)
//            {
//                return BadRequest(ex.Message);
//            }
//        }

//        [HttpPut("change-password")]
//        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
//        {
//            try
//            {
//                var userDTO = await _userRepo.GetUserByIdAsync(request.UserId);
//                if (userDTO == null)
//                    return NotFound("User not found");

//                var userService = new UserService(userDTO, _userRepo, _logger, UserService.EnMode.Update);
//                var result = await userService.ChangePasswordAsync(request.NewPassword);
//                return result ? Ok() : BadRequest("Failed to change password");
//            }
//            catch (Exception ex)
//            {
//                return BadRequest(ex.Message);
//            }
//        }
//    }

//    public record LoginRequest(string NationalNo, string Password);
//    public record UserCreateRequest(string NationalNo, string Password, string Name, EnUserRole Role);
//    public record UserUpdateRequest(string NationalNo, string Password, string Name);
//    public record ChangePasswordRequest(int UserId, string NewPassword);
//}