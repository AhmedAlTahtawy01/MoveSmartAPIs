using DataAccessLayer;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class VacationService
    {
        protected readonly ILogger<VacationService> _vacationLogger;
        protected readonly VacationRepo _vacationRepo;
        protected readonly DriverRepo _driverRepo;

        public VacationService(VacationRepo vacationRepo, DriverRepo driverRepo, ILogger<VacationService> vacationLogger)
        {
            _vacationRepo = vacationRepo ?? throw new ArgumentNullException(nameof(vacationRepo), "Data access layer cannot be null.");
            _driverRepo = driverRepo ?? throw new ArgumentNullException(nameof(driverRepo), "Data access layer cannot be null.");
            _vacationLogger = vacationLogger ?? throw new ArgumentNullException(nameof(vacationLogger), "Logger cannot be null.");
        }

        private async void _ValidateVacationDTO(VacationDTO dto)
        {
            if (dto == null)
            {
                _vacationLogger.LogError("VacationDTO cannot be null.");
                throw new ArgumentNullException(nameof(dto), "VacationDTO cannot be null.");
            }

            if(!await _driverRepo.IsDriverExistsAsync(dto.VacationOwnerID))
            {
                _vacationLogger.LogError($"VacationOwner with ID [{dto.VacationOwnerID}] does not exist.");
                throw new ArgumentException($"VacationOwner with ID [{dto.VacationOwnerID}] does not exist.");
            }

            if (!await _driverRepo.IsDriverExistsAsync(dto.SubstituteDriverID))
            {
                _vacationLogger.LogError($"SubstituteDriver with ID [{dto.SubstituteDriverID}] does not exist.");
                throw new ArgumentException($"SubstituteDriver with ID [{dto.SubstituteDriverID}] does not exist.");
            }

            if(dto.StartDate.CompareTo(dto.EndDate) > 0)
            {
                _vacationLogger.LogError("StartDate cannot be greater than EndDate.");
                throw new ArgumentException("StartDate cannot be greater than EndDate.");
            }
        }

        public async Task<int?> AddNewVacationAsync(VacationDTO dto)
        {
            try
            {
                _ValidateVacationDTO(dto);
            }
            catch (ArgumentException ex)
            {
                _vacationLogger.LogError(ex.Message);
                return null;
            }
            
            if(await _vacationRepo.IsVacationExistsAsync(dto.VacationID ?? 0))
            {
                _vacationLogger.LogError($"Vacation with ID [{dto.VacationID}] already exists.");
                return null;
            }

            dto.VacationID = await _vacationRepo.AddNewVacationAsync(dto);
            return dto.VacationID;
        }

        public async Task<bool> UpdateVacationAsync(VacationDTO dto)
        {
            try
            {
                _ValidateVacationDTO(dto);
            }
            catch (ArgumentException ex)
            {
                _vacationLogger.LogError(ex.Message);
                return false;
            }

            if (!await _vacationRepo.IsVacationExistsAsync(dto.VacationID ?? 0))
            {
                _vacationLogger.LogError($"Vacation with ID [{dto.VacationID}] doesn't exists.");
                return false;
            }

            return await _vacationRepo.UpdateVacationAsync(dto);
        }

        public async Task<List<VacationDTO>> GetAllVacationsAsync()
        {
            return await _vacationRepo.GetAllVacationsAsync();
        }

        public async Task<List<VacationDTO>> GetAllVacationsForDriverAsync(int driverID)
        {
            return await _vacationRepo.GetAllVacationsForDriverAsync(driverID);
        }

        public async Task<List<VacationDTO>> GetAllValidVacationsForDriverAsync(int driverID)
        {
            return await _vacationRepo.GetAllValidVacationsForDriverAsync(driverID);
        }

        public async Task<VacationDTO?> GetVacationByIDAsync(int vacationID)
        {
            return await _vacationRepo.GetVacationByIDAsync(vacationID);
        }

        public async Task<bool> IsDriverInVacationAsync(int driverID)
        {
            return await _vacationRepo.IsDriverinVacationAsync(driverID);
        }

        public async Task<bool> IsVacationExistsAsync(int vacationID)
        {
            return await _vacationRepo.IsVacationExistsAsync(vacationID);
        }

        public async Task<bool> DeleteVacationAsync(int vacationID)
        {
            return await _vacationRepo.DeleteVacationAsync(vacationID);
        }
    }
}
