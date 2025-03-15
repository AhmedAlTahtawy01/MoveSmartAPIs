using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using DataAccessLayer.Repositories;
using System.Linq;
using Microsoft.Extensions.Logging;
using BCrypt.Net;

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
        // Private properties
        private readonly UserRepo _dal;
        private readonly ILogger<UserService> _logger;

        // Public properties
        public int UserId { get; set; }
        public string Name { get; set; }
        public string NationalNo { get; set; }
        public string Password { get; set; }

        // These two properties are for the Super User only (Super User can read all permissions and set permissions)
        // They can't be edited by the normal user
        public EnUserRole Role { get; private set; }
        public int AccessRight { get; private set; }

        // Database properties
        public enum EnMode
        {
            AddNew = 0,
            Update = 1
        }
        public EnMode Mode { get; private set; } = EnMode.AddNew;
        public UserDTO UserDTO => new UserDTO(UserId, NationalNo, Password, Name, Role, AccessRight);

        // Constructor
        public UserService(UserDTO userDTO, UserRepo dal, ILogger<UserService> logger, EnMode mode = EnMode.AddNew)
        {
            UserId = userDTO.UserId;
            Name = userDTO.Name?.Trim();
            NationalNo = userDTO.NationalNo?.Trim();
            Password = userDTO.Password;
            Role = userDTO.Role;
            AccessRight = userDTO.AccessRight;

            _dal = dal ?? throw new ArgumentNullException(nameof(dal));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Mode = mode;
        }

        // Private methods
        private string _HashPassword(string password) => BCrypt.Net.BCrypt.HashPassword(password);

        private bool _VerifyPassword(string password, string hashedPassword) => BCrypt.Net.BCrypt.Verify(password, hashedPassword);

        private bool _CheckPermission(params EnPermissions[] permissions)
        {
            foreach (var permission in permissions)
                if ((AccessRight & (int)permission) != (int)permission)
                    return false;
            return true;
        }

        // This function can be called from the super user only (developer)
        // It will be used in the Super User Interface (UI)
        private void _SetPermissions(params EnPermissions[] permissions)
        {
            if (!_CheckPermission(EnPermissions.All))
                throw new UnauthorizedAccessException("Only superusers can modify permissions.");
            AccessRight = permissions.Length > 0 ? permissions.Aggregate(0, (acc, p) => acc | (int)p) : (int)EnPermissions.None;
        }

        // This function is the equivalent of the Super User's SetPermissions function
        // But it sets the permissions based on the user's role
        // This function will be used in the normal case, when creating a new user
        private void _SetAccessRight(EnUserRole role)
        {
            switch (role)
            {
                case EnUserRole.SuperUser:
                    AccessRight = (int)EnPermissions.All;
                    break;
                case EnUserRole.HospitalManager:
                    AccessRight = (int)EnPermissions.ApproveApplications | (int)EnPermissions.ReadAll | (int)EnPermissions.ViewReports;
                    break;
                case EnUserRole.GeneralManager:
                    AccessRight = (int)EnPermissions.ApproveApplications | (int)EnPermissions.ReadAll | (int)EnPermissions.UpdateVehicles | (int)EnPermissions.ViewReports;
                    break;
                case EnUserRole.GeneralSupervisor:
                    AccessRight = (int)EnPermissions.ApproveApplications | (int)EnPermissions.ReadAll | (int)EnPermissions.ManipulateMissions;
                    break;
                case EnUserRole.AdministrativeSupervisor:
                    AccessRight = (int)EnPermissions.ManipulateJobOrder;
                    break;
                case EnUserRole.PatrolsSupervisor:
                    AccessRight = (int)EnPermissions.ManipulatePatrols | (int)EnPermissions.ManipulateSubscriptions;
                    break;
                case EnUserRole.WorkshopSupervisor:
                    AccessRight = (int)EnPermissions.ManipulateConsumablesAndSpareParts | (int)EnPermissions.ManipulateMaintenance |
                                  (int)EnPermissions.ManipulateConsumablesPurchaseOrdersAndSparePartsPurchaseOrders |
                                  (int)EnPermissions.ManipulateConsumablesWithdrawApplicationsAndSparePartsWithdrawApplications;
                    break;
                default:
                    AccessRight = (int)EnPermissions.None;
                    break;
            }
        }

        private bool _UserValidations()
        {
            if (string.IsNullOrWhiteSpace(Name))
                throw new Exception("Name is required.");
            if (string.IsNullOrWhiteSpace(NationalNo))
                throw new Exception("National Number is required.");
            if (string.IsNullOrWhiteSpace(Password))
                throw new Exception("Password is required.");
            if (!NationalNo.All(char.IsDigit) || NationalNo.Length != 14)
                throw new Exception("National Number must be 14 digits.");
            return true;
        }

        private async Task<bool> _CreateUserAsync()
        {
            if (await _dal.NationalNoExistsAsync(NationalNo))
                throw new Exception($"National Number {NationalNo} already exists.");
            _SetAccessRight(Role);
            UserId = await _dal.CreateUserAsync(UserDTO);
            if (UserId > 0)
            {
                Mode = EnMode.Update;
                return true;
            }
            return false;
        }

        // This function will be available for the normal user to update his/her information
        // This function will be used in the normal user interface (UI)
        private async Task<bool> _UpdateUserInfoAsync()
        {
            var existingUser = await _dal.GetUserByIdAsync(UserId);
            if (existingUser == null)
                throw new Exception($"User with ID {UserId} not found.");
            if (NationalNo != existingUser.NationalNo && await _dal.NationalNoExistsAsync(NationalNo, UserId))
                throw new Exception($"National Number {NationalNo} is already in use.");
            Role = existingUser.Role;
            AccessRight = existingUser.AccessRight;
            return await _dal.UpdateUserInfoAsync(UserDTO);
        }

        // This function will be available for the Super User to update any user's information
        // They will be used in the Super User Interface (UI)
        private async Task<bool> _UpdateAllUserInfoAsync()
        {
            var existingUser = await _dal.GetUserByIdAsync(UserId);
            if (existingUser == null)
                throw new Exception($"User with ID {UserId} not found.");
            if (NationalNo != existingUser.NationalNo && await _dal.NationalNoExistsAsync(NationalNo, UserId))
                throw new Exception($"National Number {NationalNo} is already in use.");
            if (!_CheckPermission(EnPermissions.All))
                throw new UnauthorizedAccessException("Only superusers can update all user info.");
            return await _dal.UpdateAllUserInfoAsync(UserDTO);
        }

        // Public methods
        public async Task<bool> SaveAsync()
        {
            if (!_UserValidations())
                throw new Exception("User validation failed.");
            if (Mode == EnMode.Update)
            {
                var existingUser = await _dal.GetUserByIdAsync(UserId);
                if (existingUser == null)
                    throw new Exception($"User with ID {UserId} not found.");
                if (!string.IsNullOrWhiteSpace(Password))
                    Password = _HashPassword(Password);
                return Role == EnUserRole.SuperUser ? await _UpdateAllUserInfoAsync() : await _UpdateUserInfoAsync();
            }
            else
            {
                Password = _HashPassword(Password);
                return await _CreateUserAsync();
            }
        }

        public async Task<List<UserService>> GetAllUsersAsync(int pageNumber = 1, int pageSize = 10)
        {
            var userDTOs = await _dal.GetAllUsersAsync(pageNumber, pageSize);
            return userDTOs.Select(dto => new UserService(dto, _dal, _logger, EnMode.Update)).ToList();
        }

        public async Task<UserService> GetUserByIdAsync(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("User ID must be greater than 0.", nameof(userId));
            var userDTO = await _dal.GetUserByIdAsync(userId);
            return userDTO != null ? new UserService(userDTO, _dal, _logger, EnMode.Update) : null;
        }

        public async Task<UserService> GetUserByNationalNoAsync(string nationalNo)
        {
            var userDTO = await _dal.GetUserByNationalNoAsync(nationalNo?.Trim());
            return userDTO != null ? new UserService(userDTO, _dal, _logger, EnMode.Update) : null;
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            if (!_CheckPermission(EnPermissions.All))
                throw new UnauthorizedAccessException("Only superusers can delete users.");
            return await _dal.DeleteUserAsync(userId);
        }

        public async Task<UserService> LoginAsync(string nationalNo, string password)
        {
            var userDTO = await _dal.GetUserByNationalNoAsync(nationalNo?.Trim());
            if (userDTO == null)
                throw new Exception($"User with National Number {nationalNo} not found.");
            if (!_VerifyPassword(password, userDTO.Password))
                throw new Exception("Invalid password.");
            return new UserService(userDTO, _dal, _logger, EnMode.Update);
        }

        public async Task<bool> ChangePasswordAsync(string newPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
                throw new Exception("Password must be at least 6 characters.");
            Password = _HashPassword(newPassword);
            return await _dal.UpdateUserInfoAsync(UserDTO);
        }
    }
}