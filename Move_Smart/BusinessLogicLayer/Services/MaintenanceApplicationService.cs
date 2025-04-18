using BusinessLayer.Services;
using DataAccessLayer;
using DataAccessLayer.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class MaintenanceApplicationService : ApplicationService
    {
        protected readonly ILogger<MaintenanceApplicationService> _maintenanceApplicationLogger;
        protected readonly MaintenanceApplicationRepo _maintenanceApplicationRepo;
        protected readonly VehicleRepo _vehicleRepo;

        public MaintenanceApplicationService(MaintenanceApplicationRepo maintenanceApplicationRepo, ApplicationRepo applicationRepo, VehicleRepo vehicleRepo, ILogger<MaintenanceApplicationService> maintenanceApplicationLogger, ILogger<ApplicationService> applicationLogger)
            : base(applicationRepo, applicationLogger)
        {
            _maintenanceApplicationRepo = maintenanceApplicationRepo ?? throw new ArgumentNullException(nameof(maintenanceApplicationRepo), "Data access layer cannot be null.");
            _vehicleRepo = vehicleRepo ?? throw new ArgumentNullException(nameof(vehicleRepo), "Data access layer cannot be null.");
            _maintenanceApplicationLogger = maintenanceApplicationLogger ?? throw new ArgumentNullException(nameof(maintenanceApplicationLogger), "Logger cannot be null.");
        }

        private async void _ValidateMaintenanceApplicationDTO(MaintenanceApplicationDTO dto)
        {
            if (dto == null)
            {
                _maintenanceApplicationLogger.LogError("MaintenanceApplicationDTO cannot be null.");
                throw new ArgumentNullException(nameof(dto), "MaintenanceApplicationDTO cannot be null.");
            }
            if (await _repo.GetApplicationByIdAsync(dto.ApplicationID) == null)
            {
                _maintenanceApplicationLogger.LogError($"Application with ID[{dto.ApplicationID}] Doesn't Exist.");
                throw new ArgumentException(nameof(dto), $"Application with ID[{dto.ApplicationID}] Doesn't Exist.");
            }
            if (!await _vehicleRepo.IsVehicleExistsAsync(dto.VehicleID))
            {
                _maintenanceApplicationLogger.LogError($"Vehicle wih ID[{dto.VehicleID}] Doesn't Exist");
                throw new ArgumentException(nameof(dto), $"Vehicle wih ID[{dto.VehicleID}] Doesn't Exist");
            }
        }

        public async Task<int?> AddNewMaintenanceApplicationAsync(MaintenanceApplicationDTO dto)
        {
            try
            {
                _ValidateMaintenanceApplicationDTO(dto);
            }
            catch (Exception ex)
            {
                _maintenanceApplicationLogger.LogError(ex, "Validation Failed For MaintenanceApplicationDTO");
                return null;
            }

            return await _maintenanceApplicationRepo.AddNewMaintenanceApplicationAsync(dto);
        }

        public async Task<bool> UpdateMaintenanceApplicationAsync(MaintenanceApplicationDTO dto)
        {
            try
            {
                _ValidateMaintenanceApplicationDTO(dto);
            }
            catch (Exception ex)
            {
                _maintenanceApplicationLogger.LogError(ex, "Validation Failed For MaintenanceApplicationDTO");
                return false;
            }

            if (!await _maintenanceApplicationRepo.IsMaintenanceApplicationExistsAsync(dto.MaintenanceApplicationID ?? 0))
            {
                _maintenanceApplicationLogger.LogError($"MaintenanceApplication with ID {dto.MaintenanceApplicationID} doesn't exist.");
                return false;
            }

            return await _maintenanceApplicationRepo.UpdateMaintenanceApplicationAsync(dto);
        }

        public async Task<List<MaintenanceApplicationDTO>> GetAllMaintenanceApplicationsAsync()
        {
            return await _maintenanceApplicationRepo.GetAllMaintenanceApplicationsAsync();
        }

        public async Task<List<MaintenanceApplicationDTO>> GetAllMaintenanceApplicationsForVehicleAsync(short vehicleID)
        {
            return await _maintenanceApplicationRepo.GetAllMaintenanceApplicationsForVehicleAsync(vehicleID);
        }

        public async Task<MaintenanceApplicationDTO?> GetMaintenanceApplicationByMaintenanceApplicationIDAsync(int maintenanceApplicationID)
        {
            return await _maintenanceApplicationRepo.GetMaintenanceApplicationByMaintenanceApplicationIDAsync(maintenanceApplicationID);
        }

        public async Task<bool> DeleteMaintenanceApplicationAsync(int maintenanceApplicationID)
        {
            return await _maintenanceApplicationRepo.DeleteMaintenanceApplicationAsync(maintenanceApplicationID);
        }
    }
}
