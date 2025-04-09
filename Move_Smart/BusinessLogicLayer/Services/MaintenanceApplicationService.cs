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
    public class MaintenanceApplication : ApplicationService
    {
        private static ApplicationRepo _Dal;
        private static ILogger<ApplicationService> _Logger;

        public enum enMode { Add, Update };
        public enMode mode = enMode.Add;
        public int? MaintenanceApplicationID { get; set; }
        public short VehicleID { get; set; }
        public bool ApprovedByGeneralSupervisor { get; set; }
        public bool ApprovedByGeneralManager { get; set; }

        public MaintenanceApplicationDTO MaintenanceApplicationDTO => new MaintenanceApplicationDTO(
            MaintenanceApplicationID,
            ApplicationId,
            VehicleID,
            ApprovedByGeneralSupervisor,
            ApprovedByGeneralManager
            );

        public MaintenanceApplication(MaintenanceApplicationDTO maintenanceApplicationDTO, ApplicationDTO applicationDTO, ApplicationRepo dal, ILogger<ApplicationService> logger, enMode mode = enMode.Add) 
            : base(applicationDTO, dal, logger, EnMode.Update)
        {
            this.MaintenanceApplicationID = maintenanceApplicationDTO.MaintenanceApplicationID;
            this.VehicleID = maintenanceApplicationDTO.VehicleID;
            this.ApprovedByGeneralSupervisor = maintenanceApplicationDTO.ApprovedByGeneralSupervisor;
            this.ApprovedByGeneralManager = maintenanceApplicationDTO.ApprovedByGeneralManager;
            this.mode = mode;
        }

        private async Task<bool> _AddNewAsync()
        {
            this.MaintenanceApplicationID = await MaintenanceApplicationRepo.AddNewMaintenanceApplicationAsync(MaintenanceApplicationDTO);

            return this.MaintenanceApplicationID.HasValue;
        }

        private async Task<bool> _UpdateAsync()
        {
            return await MaintenanceApplicationRepo.UpdateMaintenanceApplicationAsync(MaintenanceApplicationDTO);
        }

        public static async Task<List<MaintenanceApplication>> GetAllMaintenanceApplicationsAsync()
        {
            List<MaintenanceApplicationDTO> maintenanceApplicationDTOs = await MaintenanceApplicationRepo.GetAllMaintenanceApplicationsAsync();

            List<MaintenanceApplication> maintenanceApplications = new List<MaintenanceApplication>();

            foreach(MaintenanceApplicationDTO maintenanceApplicationDTO in maintenanceApplicationDTOs)
            {
                maintenanceApplications.Add(new MaintenanceApplication(maintenanceApplicationDTO, await _Dal.GetApplicationByIdAsync(maintenanceApplicationDTO.ApplicationID), _Dal, _Logger, enMode.Update));
            }

            return maintenanceApplications;
        }

        public static async Task<List<MaintenanceApplication>> GetAllMaintenanceApplicationsForVehicleAsync(short vehicleID)
        {
            List<MaintenanceApplicationDTO> maintenanceApplicationDTOs = await MaintenanceApplicationRepo.GetAllMaintenanceApplicationsForVehicleAsync(vehicleID);

            List<MaintenanceApplication> maintenanceApplications = new List<MaintenanceApplication>();

            foreach (MaintenanceApplicationDTO maintenanceApplicationDTO in maintenanceApplicationDTOs)
            {
                maintenanceApplications.Add(new MaintenanceApplication(maintenanceApplicationDTO, await _Dal.GetApplicationByIdAsync(maintenanceApplicationDTO.ApplicationID), _Dal, _Logger, enMode.Update));
            }

            return maintenanceApplications;
        }

        public static async Task<MaintenanceApplication?> GetMaintenanceApplicationByMaintenanceApplicationIDAsync(int maintenanceApplicationID)
        {
            MaintenanceApplicationDTO maintenanceApplicationDTO = await MaintenanceApplicationRepo.GetMaintenanceApplicationByMaintenanceApplicationIDAsync(maintenanceApplicationID);

            return maintenanceApplicationDTO != null ? new MaintenanceApplication(maintenanceApplicationDTO, await _Dal.GetApplicationByIdAsync(maintenanceApplicationDTO.ApplicationID), _Dal, _Logger, enMode.Update) : null;
        }

        public override async Task<bool> SaveAsync()
        {
            if(! await base.SaveAsync())
                return false;

            switch(mode)
            {
                case enMode.Add:
                    if (await _AddNewAsync())
                    {
                        mode = enMode.Update;
                        return true;
                    }
                    else
                        return false;

                case enMode.Update:
                    return await _UpdateAsync();
            }

            return false;
        }

        public async Task<bool> DeleteAsync()
        {
            if(this.MaintenanceApplicationID.HasValue)
            {
                if (!await base.DeleteApplicationAsync(this.ApplicationId))
                    return false;

                return await MaintenanceApplicationRepo.DeleteMaintenanceApplicationAsync(this.MaintenanceApplicationID.Value);
            }

            return false;
        }
    }
}
