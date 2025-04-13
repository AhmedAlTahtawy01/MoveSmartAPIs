using DataAccessLayer;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DataAccessLayer.VehicleDTO;

namespace BusinessLayer
{
    public class VehicleService
    {
        protected readonly VehicleRepo _vehicleRepo;
        protected readonly ILogger<VehicleService> _vehicleLogger;

        public VehicleService(VehicleRepo vehicleRepo, ILogger<VehicleService> vehicleLogger)
        {
            _vehicleRepo = vehicleRepo ?? throw new ArgumentNullException(nameof(vehicleRepo), "Data access layer cannot be null.");
            _vehicleLogger = vehicleLogger ?? throw new ArgumentNullException(nameof(vehicleLogger), "Logger cannot be null.");
        }

        private void _ValidateVehicleDTO(VehicleDTO dto)
        {
            if(dto == null)
            {
                _vehicleLogger.LogError("VehicleDTO cannot be null.");
                throw new ArgumentNullException(nameof(dto), "VehicleDTO cannot be null.");
            }

            if(string.IsNullOrWhiteSpace(dto.BrandName) || string.IsNullOrWhiteSpace(dto.ModelName) || string.IsNullOrWhiteSpace(dto.PlateNumbers) || string.IsNullOrWhiteSpace(dto.AssociatedHospital) || string.IsNullOrWhiteSpace(dto.AssociatedTask))
            {
                _vehicleLogger.LogError("VehicleDTO properties cannot be null or empty.");
                throw new ArgumentException("VehicleDTO properties cannot be null or empty.");
            }

            if (dto.PlateNumbers.Length != 7)
            {
                _vehicleLogger.LogError("Plate numbers must be exactly 7 characters long.");
                throw new ArgumentException("Plate numbers must be exactly 7 characters long.");
            }
        }

        public async Task<short?> AddNewVehicleAsync(VehicleDTO dto)
        {
            try
            {
                _ValidateVehicleDTO(dto);
            }
            catch (Exception ex)
            {
                _vehicleLogger.LogError(ex, "Validation failed for VehicleDTO.");
                return null;
            }

            if(await _vehicleRepo.IsVehicleExistsAsync(dto.VehicleID ?? 0))
            {
                _vehicleLogger.LogError($"Vehicle with PlateNumbers {dto.PlateNumbers} already exists.");
                return null;
            }

            return await _vehicleRepo.AddNewVehicleAsync(dto);
        }

        public async Task<bool> UpdateAsync(VehicleDTO dto)
        {
            try
            {
                _ValidateVehicleDTO(dto);
            }
            catch (Exception ex)
            {
                _vehicleLogger.LogError(ex, "Validation failed for VehicleDTO.");
                return false;
            }

            if(await _vehicleRepo.IsVehicleExistsAsync(dto.VehicleID ?? 0))
            {
                return await _vehicleRepo.UpdateVehicleAsync(dto);
            }

            _vehicleLogger.LogError($"Vehicle with ID {dto.VehicleID} does not exist.");
            return false;
        }

        public async Task<List<VehicleDTO>> GetAllVehiclesOfTypeAsync(enVehicleType vehicleType)
        {
            return await _vehicleRepo.GetVehiclesByVehicleTypeAsync(vehicleType);
        }

        public async Task<List<VehicleDTO>> GetAllVehiclesWithStatusAsync(enVehicleStatus status)
        {
            return await _vehicleRepo.GetVehiclesByStatusAsync(status);
        }

        public async Task<List<VehicleDTO>> GetAllVehiclesUsingFuelTypeAsync(enFuelType fuelType)
        {
            return await _vehicleRepo.GetVehiclesByFuelTypeAsync(fuelType);
        }

        public async Task<VehicleDTO?> GetVehicleByIDAsync(short vehicleID)
        {
            return await _vehicleRepo.GetVehicleByIDAsync(vehicleID);
        }

        public async Task<short> GetNumberOfVehiclesAsync()
        {
            return await _vehicleRepo.GetNumberOfVehiclesAsync();
        }

        public async Task<short> GetNumberOfVehiclesWithStatusAsync(enVehicleStatus status)
        {
            return await _vehicleRepo.GetNumberOfVehiclesByStatusAsync(status);
        }

        public async Task<bool> DeleteVehicleAsync(string plateNumbers)
        {
            return await _vehicleRepo.DeleteVehicleAsync(plateNumbers);
        }
    }
}
