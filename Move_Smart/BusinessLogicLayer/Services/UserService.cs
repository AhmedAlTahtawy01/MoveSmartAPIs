using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using DataAccessLayer.Repositories;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
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
        private EnPermissions[] _EnPermissions = [ EnPermissions.None ];

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
        };
        public EnMode Mode { get; private set; } = EnMode.AddNew;
        public UserDTO UserDTO => new UserDTO(UserId, NationalNo, Password, Name, Role, AccessRight);


        // Constructor
        public UserService(UserDTO userDTO, UserRepo dal, ILogger<UserService> logger, EnMode mode = EnMode.AddNew)
        {
            UserId = userDTO.UserId;
            Name = userDTO.Name;
            NationalNo = userDTO.NationalNo;
            Password = userDTO.Password;
            Role = userDTO.Role;
            AccessRight = userDTO.AccessRight;

            _dal = dal;
            _logger = logger;
            Mode = mode;
        }


        // Private methods
        private string _HashPassword(string password) => BCrypt.Net.BCrypt.HashPassword(password);
        
        private bool _VerifyPassword(string password, string hashedPassword) => BCrypt.Net.BCrypt.Verify(password, hashedPassword); 

        private bool _CheckPermission(params EnPermissions[] permissions)
        {
            foreach (EnPermissions permission in permissions)
                if ((AccessRight & (int)permission) != (int)permission)
                    return false;

            return true;
        }

        // These two functions are for the Super User only (Super User can read all permissions and set permissions)
        // They will be used in the Super User Interface (UI)
        private void _ReadPermissions(params EnPermissions[] permissions)
        {
            AccessRight = permissions.Sum(p => (int)p);
        }
        private bool _SetPermissions(params EnPermissions[] permissions)
        {
            // Check if the user has the "-1" AccessRight (Super User Equivalent)
            if (!_CheckPermission(EnPermissions.All))
                throw new UnauthorizedAccessException("Only users with the \"All\" permission can modify permissions.");

            _EnPermissions = permissions;
            _ReadPermissions(_EnPermissions);
            return true;
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
                    AccessRight = (int)EnPermissions.ApproveApplications + (int)EnPermissions.ReadAll + (int)EnPermissions.ViewReports;
                    break;

                case EnUserRole.GeneralManager:
                    AccessRight = (int)EnPermissions.ApproveApplications + (int)EnPermissions.ReadAll + (int)EnPermissions.UpdateVehicles + (int)EnPermissions.ViewReports;
                    break;

                case EnUserRole.GeneralSupervisor:
                    AccessRight = (int)EnPermissions.ApproveApplications + (int)EnPermissions.ReadAll + (int)EnPermissions.ManipulateMissions;
                    break;

                case EnUserRole.AdministrativeSupervisor:
                    AccessRight = (int)EnPermissions.ManipulateJobOrder;
                    break;

                case EnUserRole.PatrolsSupervisor:
                    AccessRight = (int)EnPermissions.ManipulatePatrols + (int)EnPermissions.ManipulateSubscriptions;
                    break;

                case EnUserRole.WorkshopSupervisor:
                    AccessRight = (int)EnPermissions.ManipulateConsumablesAndSpareParts + (int)EnPermissions.ManipulateMaintenance
                    + (int)EnPermissions.ManipulateConsumablesPurchaseOrdersAndSparePartsPurchaseOrders + (int)EnPermissions.ManipulateConsumablesWithdrawApplicationsAndSparePartsWithdrawApplications;
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
                throw new Exception("National No must be digits only and 14 characters length.");

            return true;
        }

        private async Task<bool> _CreateUserAsync()
        {

            if (await _dal.NationalNoExistsAsync(NationalNo))
                throw new Exception($"User with National No {NationalNo} already exists.");

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
                throw new Exception("User not found.");

            // Check if the new National Number is already used by another user
            if (NationalNo!= existingUser.NationalNo && await _dal.NationalNoExistsAsync(NationalNo, UserId))
                throw new Exception($"National Number {NationalNo} is already in use by another user.");

            // Ensuring that the user can't change his/her role and access right
            Role = existingUser.Role;
            AccessRight = existingUser.AccessRight;

            return await _dal.UpdateUserInfoAsync(UserDTO);
        }

        // This function will be available for the Super User to update any user's information
        // They will be used in the Super User Interface (UI)
        private async Task<bool> _UpdateAllUserInfoAsync()
        {
            var exsitngUser = await _dal.GetUserByIdAsync(UserId);
            if (exsitngUser == null)
                throw new Exception("User not found.");

            // Check if the new National Number is already used by another user
            if (NationalNo != exsitngUser.NationalNo && await _dal.NationalNoExistsAsync(NationalNo, UserId))
                throw new Exception($"National Number {NationalNo} is already in use by another user.");

            _SetPermissions(_EnPermissions);
            return await _dal.UpdateAllUserInfoAsync(UserDTO);
        }



        // Public methods

        public async Task<bool> SaveAsync()
        {
            
            if (!_UserValidations())
                throw new Exception("User validations failed.");


            if (Mode == EnMode.Update)
            {
                var existingUser = await _dal.GetUserByIdAsync(UserId);

                if (existingUser == null)
                    throw new Exception("User not found.");

                if (Password != existingUser.Password)
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
            var userLists = new List<UserService>();

            foreach (var userDTO in userDTOs)
            {
                userLists.Add(new UserService(userDTO, _dal, _logger, EnMode.Update));
            }

            return userLists;
        }

        public async Task<UserService> GetUserByIdAsync(int userId)
        {
            if (userId <= 0)
                throw new Exception("Invalid User ID, User Id must be greater than 0.");

            var userDTO = await _dal.GetUserByIdAsync(userId);
            return userDTO != null ? new UserService(userDTO, _dal, _logger, EnMode.Update) : null;
        }

        public async Task<UserService> GetUserByNationalNoAsync(string nationalNo)
        {
            var userDTO = await _dal.GetUserByNationalNoAsync(nationalNo);
            return userDTO != null ? new UserService(userDTO, _dal, _logger, EnMode.Update) : null;
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            if (!_CheckPermission(EnPermissions.All))
                throw new UnauthorizedAccessException("Only users with the \"All\" permission can delete users.");

            return await _dal.DeleteUserAsync(userId);
        }

        public async Task<UserService> LoginAsync(string nationalNo, string password)
        {
            var userDTO = await _dal.GetUserByNationalNoAsync(nationalNo);
            
            if (userDTO == null)
                throw new Exception("User not found.");

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
