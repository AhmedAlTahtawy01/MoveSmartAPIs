using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessLayer;
using DataAccessLayer.Repositories;
using DataAccessLayer.SharedFunctions;
using Microsoft.Extensions.Logging;
using static DataAccessLayer.VehicleDTO;

namespace BusinessLogicLayer.Services
{
    public class MaintenanceService
    {
        private readonly MaintenanceRepo _repo;
        private readonly ILogger<MaintenanceService> _logger;
        private readonly MaintenanceApplicationService _maintenanceApplicationService;
        private readonly SharedFunctions _shared;

        public MaintenanceService(MaintenanceRepo repo, ILogger<MaintenanceService> logger, MaintenanceApplicationService maintenanceApplicationService, SharedFunctions sharedFunctions)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo), "Data access layer cannot be null.");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger cannot be null.");
            _maintenanceApplicationService = maintenanceApplicationService ?? throw new ArgumentNullException(nameof(maintenanceApplicationService), "Maintenance Application Service cannot be null.");
            _shared = sharedFunctions ?? throw new ArgumentNullException(nameof(sharedFunctions), "Shared Functions cannot be null.");
        }

        public async Task<int> CreateMaintenanceAsync(MaintenanceDTO dto)
        {
            
            if (dto.MaintenanceId != 0)
            {
                _logger.LogWarning("Attempted to create a maintenance with a non-zero ID.");
                throw new InvalidOperationException("Maintenance ID must be 0 for new maintenances.");
            }
            
            _ValidateMaintenanceDTO(dto);

            if (!(await _shared.CheckMaintenanceApplicationExistsAsync(dto.MaintenanceApplicationId)))
            {
                _logger.LogWarning($"Maintenance Application with Id {dto.MaintenanceApplicationId} not found.");
                throw new KeyNotFoundException($"Maintenance Application with Id {dto.MaintenanceApplicationId} not found.");
            }

            try
            {
                _logger.LogInformation($"Validating maintenance application with ID {dto.MaintenanceApplicationId}.");
                var maintenanceApplication = await _maintenanceApplicationService.GetMaintenanceApplicationByMaintenanceApplicationIDAsync(dto.MaintenanceApplicationId);

                if (maintenanceApplication == null)
                {
                    _logger.LogWarning($"Maintenance Application with ID {dto.MaintenanceApplicationId} not found.");
                    throw new KeyNotFoundException($"Maintenance Application with ID {dto.MaintenanceApplicationId} not found.");
                }

                _logger.LogInformation($"Validating maintenance application with ID {maintenanceApplication.MaintenanceApplicationID}.");
                await _shared.UpdateVehicleStatusAsync(maintenanceApplication.VehicleID, enVehicleStatus.Available);
                _logger.LogInformation($"Vehicle with Id {maintenanceApplication.VehicleID} updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating maintenance application.");
                throw new InvalidOperationException("Failed to validate maintenance application.", ex);
            }

            

            _logger.LogInformation("Creating new maintenance.");
            return await _repo.CreateMaintenanceAsync(dto);
        }

        public async Task<bool> UpdateMaintenanceAsync(MaintenanceDTO dto)
        {
            if (dto.MaintenanceId <= 0)
            {
                _logger.LogWarning("Attempted to update a maintenance with invalid ID.");
                throw new InvalidOperationException("Maintenance ID must be greater than 0 for updates.");
            }

            _ValidateMaintenanceDTO(dto);

            var existingMaintenance = await _repo.GetMaintenanceByIdAsync(dto.MaintenanceId);
            if (existingMaintenance == null)
            {
                _logger.LogWarning($"No maintenance found with ID {dto.MaintenanceId} for update.");
                throw new KeyNotFoundException($"No maintenance found with ID {dto.MaintenanceId}.");
            }

            _logger.LogInformation($"Updating maintenance with ID {dto.MaintenanceId}.");
            return await _repo.UpdateMaintenanceAsync(dto);
        }

        public async Task<List<MaintenanceDTO>> GetAllMaintenancesAsync(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize < 1)
                throw new ArgumentException("Page number and page size must be greater than 0.");
            try
            {
                return await _repo.GetAllMaintenancesAsync(pageNumber, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all maintenances.");
                throw;
            }
        }

        public async Task<MaintenanceDTO> GetMaintenanceByIdAsync(int maintenanceId)
        {
            if (maintenanceId <= 0)
            {
                _logger.LogWarning("Attempted to retrieve a maintenane with invalid ID.");
                throw new ArgumentException("Maintenenace ID must be greater than 0.");
            }

            var maintenance = await _repo.GetMaintenanceByIdAsync(maintenanceId);

            if (maintenance == null)
            {
                _logger.LogError($"Maintenance with ID {maintenanceId} not found.");
                throw new KeyNotFoundException($"Maintenance with ID {maintenanceId} not found.");
            }

            _logger.LogInformation($"Retrieving maintenance with ID {maintenanceId}.");
            return maintenance;
        }

        public async Task<List<MaintenanceDTO>> GetMaintenancesByDateAsync(DateTime date)
        {
            _logger.LogInformation($"Retrieving maintenances with date {date}.");
            return await _repo.GetMaintenancesByDateAsync(date);
        }

        public async Task<List<MaintenanceDTO>> GetMaintenanceByMaintenanceApplicationIdAsync(int applicationId)
        {
            if (applicationId <= 0)
            {
                _logger.LogWarning("Attempted to retrieve maintenance with invalid applicaion ID.");
                throw new ArgumentException("Application ID must be greater than 0.");
            }

            _logger.LogInformation($"Retriving maintenance with application ID {applicationId}.");
            return await _repo.GetMaintenanceByMaintenanceApplicationIdAsync(applicationId);
        }

        public async Task<bool> DeleteMaintenanceAsync(int maintenanceId)
        {
            if (maintenanceId <= 0)
            {
                _logger.LogWarning("Attempted to delete a maintenance with invalid ID.");
                throw new ArgumentException("Maintenance ID must be greater than 0.");
            }

            _logger.LogWarning($"Deleting maintenance with ID {maintenanceId}.");
            return await _repo.DeleteMaintenanceAsync(maintenanceId);
        }

        private void _ValidateMaintenanceDTO(MaintenanceDTO dto)
        {
            if (dto == null)
            {
                _logger.LogWarning("Attempted to work on null maintenance DTO.");
                throw new ArgumentNullException(nameof(dto), "Maintenance DTO cannot be null.");
            }

            if (dto.MaintenanceDate == default)
            {
                _logger.LogWarning("Validation Failed: Date is required.");
                throw new InvalidOperationException("Date is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.Description))
            {
                _logger.LogWarning("Validation Failed: Description is required.");
                throw new InvalidOperationException("Description is required.");
            }

            if (dto.MaintenanceApplicationId <= 0)
            {
                _logger.LogWarning("Validation Failed: Maintenance Application Id must be greater than 0.");
                throw new InvalidOperationException("Maintenance Application Id must be greater than 0.");
            }
        }
    }

}
