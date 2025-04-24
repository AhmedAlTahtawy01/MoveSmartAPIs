using DataAccessLayer;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DataAccessLayer.DriverDTO;

namespace BusinessLayer
{
    public class DriverService
    {
        protected readonly ILogger<DriverService> _driverLogger;
        protected readonly DriverRepo _driverRepo;

        public DriverService(DriverRepo driverRepo, ILogger<DriverService> driverLogger)
        {
            _driverRepo = driverRepo ?? throw new ArgumentNullException(nameof(driverRepo), "Data access layer cannot be null.");
            _driverLogger = driverLogger ?? throw new ArgumentNullException(nameof(driverLogger), "Logger cannot be null.");
        }

        private void _ValidateDriverDTO(DriverDTO dto)
        {
            if (dto == null)
            {
                _driverLogger.LogError("DriverDTO cannot be null.");
                throw new ArgumentNullException(nameof(dto), "DriverDTO cannot be null.");
            }

            if(string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.NationalNo) || string.IsNullOrWhiteSpace(dto.Phone))
            {
                _driverLogger.LogError("Name, NationalNo, Phone Can't Be Empty.");
                throw new ArgumentException("Name, NationalNo, Phone Can't Be Empty.");
            }

            if(dto.NationalNo.Any(ch => char.IsLetter(ch)))
            {
                _driverLogger.LogError("NationalNo Must Be 14 Numbers.");
                throw new ArgumentException("NationalNo Must Be 14 Numbers.");
            }

            if(dto.NationalNo.Length != 14)
            {
                _driverLogger.LogError("NationalNo Must Be 14 Numbers.");
                throw new ArgumentException("NationalNo Must Be 14 Numbers.");
            }

            if (dto.Phone.Any(ch => char.IsLetter(ch)))
            {
                _driverLogger.LogError("Phone Must Be 11 Numbers.");
                throw new ArgumentException("Phone Must Be 11 Numbers.");
            }

            if (dto.Phone.Length != 11)
            {
                _driverLogger.LogError("Phone Must Be 11 Numbers.");
                throw new ArgumentException("Phone Must Be 11 Numbers.");
            }
        }

        public async Task<int?> AddNewDriverAsync(DriverDTO dto)
        {
            try
            {
                _ValidateDriverDTO(dto);
            }
            catch (Exception ex)
            {
                _driverLogger.LogError(ex, "Validation failed for DriverDTO.");
                return null;
            }

            if(await _driverRepo.IsDriverExistsAsync(dto.NationalNo))
            {
                _driverLogger.LogError($"Driver with NationalNo [{dto.NationalNo}] already exists.");
                return null;
            }

            dto.DriverID = await _driverRepo.AddNewDriverAsync(dto);
            return dto.DriverID;
        }

        public async Task<bool> UpdateDriverAsync(DriverDTO dto)
        {
            try
            {
                _ValidateDriverDTO(dto);
            }
            catch (Exception ex)
            {
                _driverLogger.LogError(ex, "Validation failed for DriverDTO.");
                return false;
            }

            if (!await _driverRepo.IsDriverExistsAsync(dto.DriverID ?? 0))
            {
                _driverLogger.LogError($"Driver with ID {dto.DriverID} doesn't exist.");
                return false;
            }

            return await _driverRepo.UpdateDriverAsync(dto);
        }

        public async Task<List<DriverDTO>> GetAllDriversAsync()
        {
            return await _driverRepo.GetAllDriversAsync();
        }

        public async Task<List<DriverDTO>> GetAllDriversWorkingOnVehicleAsync(short vehicleID)
        {
            return await _driverRepo.GetDriversByVehicleIDAsync(vehicleID);
        }

        public async Task<List<DriverDTO>> GetAllDriversWorkingOnVehicleAsync(string plateNumbers)
        {
            return await _driverRepo.GetDriversByVehiclePlateNumbersAsync(plateNumbers);
        }

        public async Task<List<DriverDTO>> GetAllDriversWithStatusAsync(enDriverStatus status)
        {
            return await _driverRepo.GetDriversByStatusAsync(status);
        }

        public async Task<DriverDTO?> GetDriverByIDAsync(int driverID)
        {
            return await _driverRepo.GetDriverByIDAsync(driverID);
        }

        public async Task<DriverDTO?> GetDriverByNationalNoAsync(string nationalNo)
        {
            return await _driverRepo.GetDriverByNationalNoAsync(nationalNo);
        }

        public async Task<DriverDTO?> GetDriverByPhoneAsync(string phone)
        {
            return await _driverRepo.GetDriverByPhoneAsync(phone);
        }

        public async Task<short> GetNumberOfDriversAsync()
        {
            return await _driverRepo.GetNumberOfDriversAsync();
        }

        public async Task<short> GetNumberOfDriversByStatusAsync(enDriverStatus status)
        {
            return await _driverRepo.GetNumberOfDriversByStatusAsync(status);
        }

        public async Task<bool> DeleteDriverAsync(string nationalNo)
        {
            return await _driverRepo.DeleteDriverAsync(nationalNo);
        }

        public async Task<bool> DeleteDriverAsync(int driverID)
        {
            return await _driverRepo.DeleteDriverAsync(driverID);
        }

        public async Task<bool> IsDriverExistsAsync(string nationalNo)
        {
            return await _driverRepo.IsDriverExistsAsync(nationalNo);
        }

        public async Task<bool> IsDriverExistsAsync(int driverID)
        {
            return await _driverRepo.IsDriverExistsAsync(driverID);
        }
    }
}
