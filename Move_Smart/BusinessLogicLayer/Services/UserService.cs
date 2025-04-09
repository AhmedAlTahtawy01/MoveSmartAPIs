using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using DataAccessLayer.Repositories;
using System.Linq;
using Microsoft.Extensions.Logging;
using BCrypt.Net;
using ZstdSharp.Unsafe;
using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Security;

namespace BusinessLayer.Services
{
    public enum EnPermissions
    {
        All = ~0,
        None = 0,
        ReadAll = 1,
        ApproveApplications = 2,
        UpdateConsumablesAndSpareParts = 4,
        DeleteConsumablesAndSpareParts = 8,
        UpdateDrivers = 16,
        UpdateVehicles = 32,
        ManipulateConsumablesAndSpareParts = 64,
        ManipulatePatrols = 128,
        ManipulateJobOrder = 256,
        ManipulateMaintenance = 512,
        ManipulateConsumablesPurchaseOrdersAndSparePartsPurchaseOrders = 1024,
        ManipulateConsumablesWithdrawApplicationsAndSparePartsWithdrawApplications = 2048,
        ManipulateMissions = 4096,
        ManipulateSubscriptions = 8192,
        ViewReports = 16384
    }


    public class UserService
    {
        private readonly UserRepo _repo;
        private readonly ILogger<UserService> _logger;

        public UserService(UserRepo repo, ILogger<UserService> logger)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo), "Data access layer cannot be null.");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger cannot be null.");
        }

        private int _SetAccessRight(EnUserRole role)
        {
            switch (role)
            {
                case EnUserRole.SuperUser:
                    return (int)EnPermissions.All;

                case EnUserRole.HospitalManager:
                    return (int)EnPermissions.ApproveApplications | (int)EnPermissions.ReadAll | (int)EnPermissions.ViewReports;
                    
                case EnUserRole.GeneralManager:
                    return (int)EnPermissions.ApproveApplications | (int)EnPermissions.ReadAll | (int)EnPermissions.UpdateVehicles | (int)EnPermissions.ViewReports;
                    
                case EnUserRole.GeneralSupervisor:
                    return (int)EnPermissions.ApproveApplications | (int)EnPermissions.ReadAll | (int)EnPermissions.ManipulateMissions;
                    
                case EnUserRole.AdministrativeSupervisor:
                    return (int)EnPermissions.ManipulateJobOrder;
                    
                case EnUserRole.PatrolsSupervisor:
                    return (int)EnPermissions.ManipulatePatrols | (int)EnPermissions.ManipulateSubscriptions;
                    
                case EnUserRole.WorkshopSupervisor:
                    return (int)EnPermissions.ManipulateConsumablesAndSpareParts | (int)EnPermissions.ManipulateMaintenance |
                                  (int)EnPermissions.ManipulateConsumablesPurchaseOrdersAndSparePartsPurchaseOrders |
                                  (int)EnPermissions.ManipulateConsumablesWithdrawApplicationsAndSparePartsWithdrawApplications;
                    
                default:
                    return (int)EnPermissions.None;
                    
            }
        }

        private void _ValidateUserDTO(UserDTO dto)
        {
            if (dto == null)
            {
                _logger.LogError("User DTO cannot be null.");
                throw new ArgumentNullException(nameof(dto), "User DTO cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                _logger.LogError("Name is required.");
                throw new InvalidOperationException("Name is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.NationalNo))
            {
                _logger.LogError("National Number is required.");
                throw new InvalidOperationException("National Number is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.Password))
            {
                _logger.LogError("Password is required.");
                throw new InvalidOperationException("Password is required.");
            }
            if (!dto.NationalNo.All(char.IsDigit) || dto.NationalNo.Length != 14)
            {
                _logger.LogError("National Number must be 14 digits.");
                throw new InvalidOperationException("National Number must be 14 digits.");
            }
        }

        private bool _CheckPermission(int accessRight, params EnPermissions[] permissions)
        {
            foreach (var permission in permissions)
                if ((accessRight & (int)permission) != (int)permission)
                    return false;
            return true;
        }

        public async Task<int> CreateUserAsync(UserDTO dto)
        {
            if (dto == null)
            {
                _logger.LogError("User DTO cannot be null.");
                throw new ArgumentNullException(nameof(dto), "User DTO cannot be null.");
            }

            if (dto.UserId > 0)
            {
                _logger.LogError("User ID must be 0 for new user creation.");
                throw new InvalidOperationException("User ID must be 0 for new user creation.");
            }

            try
            {
                _ValidateUserDTO(dto);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError($"Validation failed: {ex.Message}");
                throw;
            }

            if (await _repo.NationalNoExistsAsync(dto.NationalNo))
            {
                _logger.LogError($"National Number {dto.NationalNo} already exists.");
                throw new InvalidOperationException($"National Number {dto.NationalNo} already exists.");
            }

            dto.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            dto.AccessRight = _SetAccessRight(dto.Role);

            _logger.LogInformation($"Creating user with National Number: {dto.NationalNo}");
            return await _repo.CreateUserAsync(dto);

        }

        public async Task<bool> UpdateUserInfoAsync(UserDTO dto)
        {
            if (dto == null)
            {
                _logger.LogError("User DTO cannot be null.");
                throw new ArgumentNullException(nameof(dto), "User DTO cannot be null.");
            }

            if (dto.UserId <= 0)
            {
                _logger.LogError("User ID must be greater than 0 for update.");
                throw new ArgumentException("User ID must be greater than 0 for update.", nameof(dto.UserId));
            }

            try
            {
                _ValidateUserDTO(dto);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError($"Validation failed: {ex.Message}");
                throw;
            }

            var existingUser = await _repo.GetUserByIdAsync(dto.UserId);
            if (existingUser == null)
            {
                _logger.LogError($"User with ID {dto.UserId} not found.");
                throw new KeyNotFoundException($"User with ID {dto.UserId} not found.");
            }
            if (dto.NationalNo != existingUser.NationalNo && await _repo.NationalNoExistsAsync(dto.NationalNo, dto.UserId))
            {
                _logger.LogError($"National Number {dto.NationalNo} is already in use.");
                throw new InvalidOperationException($"National Number {dto.NationalNo} is already in use.");
            }
            
            dto.Role = existingUser.Role;
            dto.AccessRight = existingUser.AccessRight;

            if (!string.IsNullOrWhiteSpace(dto.Password))
                dto.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            _logger.LogInformation($"Updating user info for User ID: {dto.UserId}");
            return await _repo.UpdateUserInfoAsync(dto);
        }

        public async Task<bool> UpdateAllUserInfoAsync(UserDTO dto, int requestingUserId)
        {
            if (dto == null)
            {
                _logger.LogError("User DTO cannot be null.");
                throw new ArgumentNullException(nameof(dto), "User DTO cannot be null.");
            }

            if (dto.UserId <= 0)
            {
                _logger.LogError("User ID must be greater than 0 for update.");
                throw new InvalidOperationException("User ID must be greater than 0 for update.");
            }

            try
            {
                _ValidateUserDTO(dto);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError($"Validation failed: {ex.Message}");
                throw;
            }

            var existingUser = await _repo.GetUserByIdAsync(dto.UserId);
            if (existingUser == null)
            {
                _logger.LogError($"User with ID {dto.UserId} not found.");
                throw new KeyNotFoundException($"User with ID {dto.UserId} not found.");
            }

            var requestingUser = await _repo.GetUserByIdAsync(requestingUserId);
            if (requestingUser == null || !_CheckPermission(requestingUser.AccessRight, EnPermissions.All))
            {
                _logger.LogError($"User with ID {requestingUserId} does not have permission to update all user info.");
                throw new UnauthorizedAccessException($"User with ID {requestingUserId} does not have permission to update all user info.");
            }

            if (dto.NationalNo != existingUser.NationalNo && await _repo.NationalNoExistsAsync(dto.NationalNo, dto.UserId))
            {
                _logger.LogError($"National Number {dto.NationalNo} is already in use.");
                throw new InvalidOperationException($"National Number {dto.NationalNo} is already in use.");
            }

            if (!string.IsNullOrWhiteSpace(dto.Password))
                dto.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            _logger.LogInformation($"Updating all user info for User ID: {dto.UserId}");
            return await _repo.UpdateAllUserInfoAsync(dto);
        }

        public async Task<List<UserDTO>> GetAllUsersAsync(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                _logger.LogWarning("Invalid pagination parameters.");
                throw new ArgumentException("Page number and page size must be greater than 0.");
            }

            _logger.LogInformation($"Retrieving all users (Page {pageNumber}, Size {pageSize}).");
            return await _repo.GetAllUsersAsync(pageNumber, pageSize);
        }

        public async Task<UserDTO> GetUserByIdAsync(int userId)
        {
            if (userId <= 0)
            {
                _logger.LogError("User ID must be greater than 0.");
                throw new ArgumentException("User ID must be greater than 0.", nameof(userId));
            }

            var user = await _repo.GetUserByIdAsync(userId);

            if (user == null)
            {
                _logger.LogError($"User with ID {userId} not found.");
                throw new KeyNotFoundException($"User with ID {userId} not found.");
            }

            _logger.LogInformation($"Retrieving user with ID: {userId}");
            return user;
        }

        public async Task<UserDTO> GetUserByNationalNoAsync(string nationalNo)
        {
            if (string.IsNullOrWhiteSpace(nationalNo))
            {
                _logger.LogError("National Number cannot be null or empty.");
                throw new ArgumentException("National Number cannot be null or empty.", nameof(nationalNo));
            }

            var user = await _repo.GetUserByNationalNoAsync(nationalNo.Trim());

            if (user == null)
            {
                _logger.LogError($"User with National Number {nationalNo} not found.");
                throw new KeyNotFoundException($"User with National Number {nationalNo} not found.");
            }

            _logger.LogInformation($"Retrieving user with National Number: {nationalNo}");
            return user;
        }

        public async Task<bool> DeleteUserAsync(int userId, int requestingUserId)
        {
            if (userId <= 0)
            {
                _logger.LogError("User ID must be greater than 0.");
                throw new ArgumentException("User ID must be greater than 0.", nameof(userId));
            }

            var requestingUser = await _repo.GetUserByIdAsync(requestingUserId);
            if (requestingUser == null || !_CheckPermission(requestingUser.AccessRight, EnPermissions.All))
            {
                _logger.LogError($"User with ID {requestingUserId} does not have permission to delete users.");
                throw new UnauthorizedAccessException($"User with ID {requestingUserId} does not have permission to delete users.");
            }

            _logger.LogInformation($"Deleting user with ID: {userId}");
            return await _repo.DeleteUserAsync(userId);
        }

        public async Task<UserDTO> LoginAsync(string nationalNo, string password)
        {
            if (string.IsNullOrWhiteSpace(nationalNo))
            {
                _logger.LogError("National Number cannot be null or empty.");
                throw new ArgumentException("National Number cannot be null or empty.", nameof(nationalNo));
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                _logger.LogError("Password cannot be null or empty.");
                throw new ArgumentException("Password cannot be null or empty.", nameof(password));
            }
            var user = await _repo.GetUserByNationalNoAsync(nationalNo.Trim());
            if (user == null)
            {
                _logger.LogError($"Login failed: User with National Number {nationalNo} not found.");
                throw new InvalidOperationException($"User with National Number {nationalNo} not found.");
            }
            if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                _logger.LogError("Login failed: Invalid password.");
                throw new UnauthorizedAccessException("Invalid password.");
            }

            _logger.LogInformation($"User with National Number {nationalNo} logged in successfully.");
            return user;
        }

        public async Task<bool> ChangePasswordAsync(int userId, string newPassword)
        {
            if (userId <= 0)
            {
                _logger.LogError("User ID must be greater than 0.");
                throw new ArgumentException("User ID must be greater than 0.", nameof(userId));
            }
            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
            {
                _logger.LogError("Password must be at least 6 characters.");
                throw new ArgumentException("Password must be at least 6 characters.", nameof(newPassword));
            }
            var user = await _repo.GetUserByIdAsync(userId);
            if (user == null)
            {
                _logger.LogError($"User with ID {userId} not found.");
                throw new KeyNotFoundException($"User with ID {userId} not found.");
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);

            _logger.LogInformation($"Changing password for user with ID: {userId}");
            return await _repo.UpdateUserInfoAsync(user);
        }
    }
}