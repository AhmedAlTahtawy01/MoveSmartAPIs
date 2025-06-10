using BusinessLayer.Services;
using BusinessLogicLayer.Hubs;
using DataAccessLayer;
using DataAccessLayer.Repositories;
using DataAccessLayer.SharedFunctions;
using Microsoft.AspNetCore.SignalR;
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

        public MissionsNotesService(MissionsNotesRepo missionsNotesRepo, ApplicationRepo applicationRepo, ILogger<ApplicationService> applicationLogger, ILogger<MissionsNotesService> missionsNotesLogger, SharedFunctions sharedFunctions, IHubContext<NotificationHub> notificationHub)
            : base(applicationRepo, applicationLogger, sharedFunctions,notificationHub)
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
            
            try
            {
                await _ValidateApplicationDTO(dto.Application);
            }
            catch (Exception ex)
            {
                _missionsNotesLogger.LogError(ex, "Validation Failed For ApplicationDTO");
                throw new ArgumentException("ApplicationDTO is invalid.", nameof(dto.Application));
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

            dto.Application.ApplicationId = await CreateApplicationAsync(dto.Application);
            dto.ApplicationID = dto.Application.ApplicationId;

            try
            {
                await GetApplicationByIdAsync(dto.ApplicationID);
            }
            catch (Exception ex)
            {
                _missionsNotesLogger.LogError(ex, $"Error : {ex.Message}");
                return null;
            }

            dto.NoteID = await _missionsNotesRepo.AddNewMissionNoteAsync(dto);
            return dto.NoteID;
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

            if(!await _missionsNotesRepo.IsMissionNoteExistsAsync(dto.NoteID ?? 0))
            {
                _missionsNotesLogger.LogError($"Mission Note with ID {dto.NoteID} doesn't exist.");
                return false;
            }

            if(!await UpdateApplicationAsync(dto.Application))
            {
                _missionsNotesLogger.LogError($"Failed to update Application with ID {dto.Application.ApplicationId}.");
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

        public async Task<bool> IsMissionNoteExistsAsync(int noteID)
        {
            return await _missionsNotesRepo.IsMissionNoteExistsAsync(noteID);
        }

        public async Task<bool> DeleteMissionNoteAsync(int noteID)
        {
            return await _missionsNotesRepo.DeleteMissionNoteAsync(noteID);
        }
    }
}
