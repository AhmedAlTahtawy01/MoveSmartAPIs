using DataAccessLayer;
using DataAccessLayer.Repositories;
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

        protected void _ValidateVehicleDTO(VehicleDTO dto)
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

            if (dto.PlateNumbers.Length != 6 && dto.PlateNumbers.Length != 7)
            {
                _vehicleLogger.LogError("Plate numbers must be between 6-7 characters long.");
                throw new ArgumentException("Plate numbers must be between 6-7 characters long.");
            }

            if (!Enum.IsDefined(typeof(enVehicleStatus), dto.Status))
            {
                _vehicleLogger.LogError("Validation Failed: Invalid status.");
                throw new InvalidOperationException("Invalid status.");
            }

            if (!Enum.IsDefined(typeof(enVehicleType), dto.VehicleType))
            {
                _vehicleLogger.LogError("Validation Failed: Invalid status.");
                throw new InvalidOperationException("Invalid status.");
            }

            if (!Enum.IsDefined(typeof(enFuelType), dto.FuelType))
            {
                _vehicleLogger.LogError("Validation Failed: Invalid status.");
                throw new InvalidOperationException("Invalid status.");
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

            dto.VehicleID = await _vehicleRepo.AddNewVehicleAsync(dto);
            return dto.VehicleID;
        }

        public async Task<bool> UpdateVehicleAsync(VehicleDTO dto)
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

        public async Task<bool> UpdateVehicleTotalKilometersAsync(int totalKilometers, short vehicleID)
        {
            if (totalKilometers < 0)
            {
                _vehicleLogger.LogError("Total kilometers cannot be negative.");
                return false;
            }

            if (vehicleID <= 0)
            {
                _vehicleLogger.LogError("Vehicle ID must be a positive integer.");
                return false;
            }

            try
            {
                if (!await _vehicleRepo.IsVehicleExistsAsync(vehicleID))
                {
                    _vehicleLogger.LogError($"Vehicle with ID {vehicleID} does not exist.");
                    return false;
                }

                _vehicleLogger.LogInformation($"Updating total kilometers for vehicle ID {vehicleID} by {totalKilometers}.");
                return await _vehicleRepo.UpdateVehicleTotalKilometersAsync(totalKilometers, vehicleID);
            }
            catch (Exception ex)
            {
                _vehicleLogger.LogError(ex, "Failed to update total kilometers for vehicle.");
                return false;
            }
        }

        public async Task<List<VehicleDTO>> GetAllVehiclesAsync()
        {
            return await _vehicleRepo.GetAllVehiclesAsync();
        }

        public async Task<List<VehicleDTO>> GetAllVehiclesOfTypeAsync(enVehicleType vehicleType)
        {
            return await _vehicleRepo.GetVehiclesByVehicleTypeAsync(vehicleType);
        }

        public async Task<List<VehicleDTO>> GetAllVehiclesWithStatusAsync(enVehicleStatus status)
        {
            return await _vehicleRepo.GetVehiclesByStatusAsync(status);
        }

        public async Task<List<VehicleDTO>> GetAllVehiclesUsingFuelOfTypeAsync(enFuelType fuelType)
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

        public async Task<bool> IsVehicleExistsAsync(short vehicleID)
        {
            return await _vehicleRepo.IsVehicleExistsAsync(vehicleID);
        }

        public async Task<bool> IsVehicleExistsAsync(string plateNumbers)
        {
            return await _vehicleRepo.IsVehicleExistsAsync(plateNumbers);
        }

        public async Task<bool> DeleteVehicleAsync(string plateNumbers)
        {
            return await _vehicleRepo.DeleteVehicleAsync(plateNumbers);
        }

        public async Task<bool> DeleteVehicleAsync(short vehicleID)
        {
            return await _vehicleRepo.DeleteVehicleAsync(vehicleID);
        }
    }
}
