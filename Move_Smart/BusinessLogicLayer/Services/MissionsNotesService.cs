using BusinessLayer.Services;
using DataAccessLayer;
using DataAccessLayer.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class MissionsNotesService : ApplicationService
    {
        protected readonly MissionsNotesRepo _missionsNotesRepo;
        protected readonly ILogger<MissionsNotesService> _missionsNotesLogger;

        public MissionsNotesService(MissionsNotesRepo missionsNotesRepo, ApplicationRepo applicationRepo, ILogger<ApplicationService> applicationLogger, ILogger<MissionsNotesService> missionsNotesLogger)
            : base(applicationRepo, applicationLogger)
        {
            _missionsNotesRepo = missionsNotesRepo ?? throw new ArgumentNullException(nameof(missionsNotesRepo), "Data access layer cannot be null.");
            _missionsNotesLogger = missionsNotesLogger ?? throw new ArgumentNullException(nameof(missionsNotesLogger), "Logger cannot be null.");
        }

        private async void _ValidateMissionsNotesDTO(MissionsNotesDTO dto)
        {
            if (dto == null)
            {
                _missionsNotesLogger.LogError("MissionsNotesDTO cannot be null.");
                throw new ArgumentNullException(nameof(dto), "MissionsNotesDTO cannot be null.");
            }
            if (await _repo.GetApplicationByIdAsync(dto.ApplicationID) == null)
            {
                _missionsNotesLogger.LogError($"Application with ID[{dto.ApplicationID}] Doesn't Existes.");
                throw new ArgumentException(nameof(dto), $"Application with ID[{dto.ApplicationID}] Doesn't Existes.");
            }
        }

        public async Task<int?> AddNewMissionNoteAsync(MissionsNotesDTO dto)
        {
            try
            {
                _ValidateMissionsNotesDTO(dto);
            }
            catch (Exception ex)
            {
                _missionsNotesLogger.LogError(ex, "Validation Failed For MissionsNotesDTO");
            }

            return await _missionsNotesRepo.AddNewMissionNoteAsync(dto);
        }

        public async Task<bool> UpdateMissionNoteAsync(MissionsNotesDTO dto)
        {
            try
            {
                _ValidateMissionsNotesDTO(dto);
            }
            catch (Exception ex)
            {
                _missionsNotesLogger.LogError(ex, "Validation Failed For MissionsNotesDTO");
            }

            if(!await _missionsNotesRepo.IsMissionNoteExistsAsync(dto.NoteID))
            {
                _missionsNotesLogger.LogError($"Mission Note with ID {dto.NoteID} doesn't exist.");
                return false;
            }

            return await _missionsNotesRepo.UpdateMissionNoteAsync(dto);
        }

        public async Task<List<MissionsNotesDTO>> GetAllMissionsNotesAsync()
        {
            return await _missionsNotesRepo.GetAllMissionsNotesAsync();
        }

        public async Task<MissionsNotesDTO> GetMissionNoteByNoteIDAsync(int noteID)
        {
            return await _missionsNotesRepo.GetMissionNoteByNoteIDAsync(noteID);
        }

        public async Task<MissionsNotesDTO> GetMissionNoteByApplicationIDAsync(int applicationID)
        {
            return await _missionsNotesRepo.GetMissionNoteByApplicationIDAsync(applicationID);
        }

        public async Task<bool> DeleteMissionNoteAsync(int noteID)
        {
            return await _missionsNotesRepo.DeleteMissionNoteAsync(noteID);
        }
    }
}
