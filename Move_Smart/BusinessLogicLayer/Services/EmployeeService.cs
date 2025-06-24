using DataAccessLayer;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DataAccessLayer.EmployeeDTO;
using static DataAccessLayer.VehicleDTO;

namespace BusinessLayer
{
    public class EmployeeService
    {
        protected readonly ILogger<EmployeeService> _employeeLogger;
        protected readonly EmployeeRepo _employeeRepo;

        public EmployeeService(EmployeeRepo employeeRepo, ILogger<EmployeeService> employeeLogger)
        {
            _employeeRepo = employeeRepo ?? throw new ArgumentNullException(nameof(employeeRepo), "Data access layer cannot be null.");
            _employeeLogger = employeeLogger ?? throw new ArgumentNullException(nameof(employeeLogger), "Logger cannot be null.");
        }

        private void _ValidateEmployeeDTO(EmployeeDTO dto)
        {
            if (dto == null)
            {
                _employeeLogger.LogError("EmployeeDTO cannot be null.");
                throw new ArgumentNullException(nameof(dto), "EmployeeDTO cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.NationalNo) || string.IsNullOrWhiteSpace(dto.Phone) || string.IsNullOrWhiteSpace(dto.JobTitle))
            {
                _employeeLogger.LogError("Name, NationalNo, JobTitle, Phone Can't Be Empty.");
                throw new ArgumentException("Name, NationalNo, JobTitle, Phone Can't Be Empty.");
            }

            if (dto.NationalNo.Any(ch => char.IsLetter(ch)))
            {
                _employeeLogger.LogError("NationalNo Must Be 14 Numbers.");
                throw new ArgumentException("NationalNo Must Be 14 Numbers.");
            }

            if (dto.NationalNo.Length != 14)
            {
                _employeeLogger.LogError("NationalNo Must Be 14 Numbers.");
                throw new ArgumentException("NationalNo Must Be 14 Numbers.");
            }

            if (dto.Phone.Any(ch => char.IsLetter(ch)))
            {
                _employeeLogger.LogError("Phone Must Be 11 Numbers.");
                throw new ArgumentException("Phone Must Be 11 Numbers.");
            }

            if (dto.Phone.Length != 11)
            {
                _employeeLogger.LogError("Phone Must Be 11 Numbers.");
                throw new ArgumentException("Phone Must Be 11 Numbers.");
            }

            if (!Enum.IsDefined(typeof(enTransportationSubscriptionStatus), dto.TransportationSubscriptionStatus))
            {
                _employeeLogger.LogError("Validation Failed: Invalid status.");
                throw new InvalidOperationException("Invalid status.");
            }
        }

        public async Task<int?> AddNewEmployeeAsync(EmployeeDTO dto)
        {
            try
            {
                _ValidateEmployeeDTO(dto);
            }
            catch (Exception ex)
            {
                _employeeLogger.LogError(ex, "Error adding new employee.");
                return null;
            }

            if(await _employeeRepo.IsEmployeeExistsAsync(dto.NationalNo))
            {
                _employeeLogger.LogError($"Employee with NationalNo [{dto.NationalNo}] already exists.");
                return null;
            }

            dto.EmployeeID = await _employeeRepo.AddNewEmployeeAsync(dto);
            return dto.EmployeeID;
        }

        public async Task<bool> UpdateEmployeeAsync(EmployeeDTO dto)
        {
            try
            {
                _ValidateEmployeeDTO(dto);
            }
            catch (Exception ex)
            {
                _employeeLogger.LogError(ex, "Error updating employee.");
                return false;
            }

            if (!await _employeeRepo.IsEmployeeExistsAsync(dto.EmployeeID ?? 0))
            {
                _employeeLogger.LogError($"Employee with ID [{dto.EmployeeID}] doesn't exist.");
                return false;
            }

            return await _employeeRepo.UpdateEmployeeAsync(dto);
        }

        public async Task<List<EmployeeDTO>> GetAllEmployeesAsync()
        {
            return await _employeeRepo.GetAllEmployeesAsync();
        }

        public async Task<List<EmployeeDTO>> GetAllEmployeesWhoAreUsingBusAsync(byte busID)
        {
            return await _employeeRepo.GetAllEmployeesWhoAreUsingBusAsync(busID);
        }

        public async Task<EmployeeDTO?> GetEmployeeByIDAsync(int employeeID)
        {
            return await _employeeRepo.GetEmployeeByIDAsync(employeeID);
        }

        public async Task<EmployeeDTO?> GetEmployeeByNationalNoAsync(string nationalNo)
        {
            return await _employeeRepo.GetEmployeeByNationalNoAsync(nationalNo);
        }

        public async Task<EmployeeDTO?> GetEmployeeByPhoneAsync(string phone)
        {
            return await _employeeRepo.GetEmployeeByPhoneAsync(phone);
        }

        public async Task<bool> DeleteEmployeeAsync(string nationalNo)
        {
            return await _employeeRepo.DeleteEmployeeAsync(nationalNo);
        }

        public async Task<bool> DeleteEmployeeAsync(int employeeID)
        {
            return await _employeeRepo.DeleteEmployeeAsync(employeeID);
        }

        public async Task<bool> IsTransportationSubscriptionValidAsync(int employeeID)
        {
            return await _employeeRepo.IsEmployeeTransportationSubscriptionValidAsync(employeeID);
        }

        public async Task<bool> IsEmployeeExistsAsync(string nationalNo)
        {
            return await _employeeRepo.IsEmployeeExistsAsync(nationalNo);
        }

        public async Task<bool> IsEmployeeExistsAsync(int employeeID)
        {
            return await _employeeRepo.IsEmployeeExistsAsync(employeeID);
        }
    }
}
