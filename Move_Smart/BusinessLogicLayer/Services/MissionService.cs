using BusinessLayer.Services;
using DataAccessLayer.Repositories;
using DataAccessLayer.SharedFunctions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class MissionService
    {
        private readonly MissionRepo _repo;
        private readonly ILogger<MissionService> _logger;
        private readonly SharedFunctions _shared;

        public MissionService(MissionRepo repo, ILogger<MissionService> logger, SharedFunctions shared)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo), "Data access layer cannot be null.");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger cannot be null.");
            _shared = shared ?? throw new ArgumentNullException(nameof(shared), "Shared Functions cannot be null.");
        }

        public async Task<int> CreateMissionAsync(int missionNoteId, MissionDTO dto)
        {
            if (dto.MissionId != 0)
            {
                _logger.LogWarning("Attempted to create a mission with a non-zero ID.");
                throw new InvalidOperationException("Mission ID must be 0 for new missions.");
            }

            _ValidateMissionDTO(dto);

            if (!(await _shared.CheckMissionNoteExistsAsync(missionNoteId)))
            {
                _logger.LogWarning($"Mission Note with Id {missionNoteId} not found.");
                throw new KeyNotFoundException($"Mission Note with Id {missionNoteId} not found.");
            }

            if (!(await _shared.CheckUserExistsAsync(dto.UserId)))
            {
                _logger.LogWarning($"User with Id {dto.UserId} not found.");
                throw new KeyNotFoundException($"User with Id {dto.UserId} not found.");
            }

            dto.MissionNoteId = missionNoteId;

            _logger.LogInformation("Creating new mission.");
            return await _repo.CreateMissionAsync(dto);
        }

        public async Task<bool> UpdateMissionAsync(MissionDTO dto)
        {
            if (dto.MissionId <= 0)
            {
                _logger.LogWarning("Attempted to update a mission with invalid ID.");
                throw new InvalidOperationException("Mission ID must be greater than 0 for updates.");
            }

            _ValidateMissionDTO(dto);

            var existingMission = await _repo.GetMissionByIdAsync(dto.MissionId);
            if (existingMission == null)
            {
                _logger.LogWarning($"Mission with ID {dto.MissionId} not found.");
                throw new KeyNotFoundException($"Mission with ID {dto.MissionId} not found.");
            }

            _logger.LogInformation("Updating Mission.");
            return await _repo.UpdateMissionAsync(dto);
        }

        public async Task<List<MissionDTO>> GetAllMissionsAsync(int pageNumber, int pageSize)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                _logger.LogWarning("Invalid pagination parameters.");
                throw new ArgumentException("Page number and page size must be greater than 0.");
            }

            _logger.LogInformation("Retrieving all missions.");
            return await _repo.GetAllMissionsAsync(pageNumber, pageSize);
        }

        public async Task<MissionDTO> GetMissionByIdAsync(int missionId)
        {
            if (missionId <= 0)
            {
                _logger.LogWarning("Attempted to retrieve a mission with invalid ID.");
                throw new ArgumentException("Mission ID must be greater than 0.");
            }

            var mission = await _repo.GetMissionByIdAsync(missionId);
            if (mission == null)
            {
                _logger.LogError($"Mission With ID {missionId} not found.");
                throw new KeyNotFoundException($"Mission with ID {missionId} not found.");
            }

            _logger.LogInformation($"Retrieving mission with ID {missionId}.");
            return mission;
        }

        public async Task<List<MissionDTO>> GetMissionsByNoteIdAsync(int missionNoteId)
        {
            if (missionNoteId <= 0)
            {
                _logger.LogWarning("Attempted to retrieve missions with invalid Note ID.");
                throw new ArgumentException("Mission Note ID must be greater than 0.");
            }

            _logger.LogInformation($"Retrieving missions for Note ID {missionNoteId}.");
            return await _repo.GetMissionsByNoteIdAsync(missionNoteId);
        }

        public async Task<List<MissionDTO>> GetMissionsByVehicleIdAsync(int missionVehicleId)
        {
            if (missionVehicleId <= 0)
            {
                _logger.LogWarning("Attempted to retrieve missions with invalid Vehicle ID.");
                throw new ArgumentException("Mission Vehicle ID must be greater than 0.");
            }

            _logger.LogInformation($"Retrieving missions for Vehicle ID {missionVehicleId}");
            return await _repo.GetMissionsByVehicleIdAsync(missionVehicleId);
        }

        public async Task<List<MissionDTO>> GetMissionsByStartDateAsync(DateTime startDate)
        {
            if (startDate == default)
            {
                _logger.LogWarning("Attempted to retieve missions with invalid Date.");
                throw new ArgumentException("Mission Start Date must has a value.");
            }

            _logger.LogInformation($"Retrieving missions with start date {startDate}");
            return await _repo.GetMissionsByStartDateAsync(startDate);
        }

        public async Task<List<MissionDTO>> GetMissionsByDestinationAsync(string destination)
        {
            if (string.IsNullOrEmpty(destination))
            {
                _logger.LogWarning("Attempted to retrieve missions with invalid destination.");
                throw new ArgumentException("Mission Destination must has a value.");
            }

            _logger.LogInformation($"Retrieving missions with destination {destination}.");
            return await _repo.GetMissionsByDestinationAsync(destination);
        }

        public async Task<bool> DeleteMissionAsync(int missionId)
        {
            if (missionId <= 0)
            {
                _logger.LogWarning("Attempted to delete a mission with invalid ID.");
                throw new ArgumentException("Mission ID must be greater than 0.");
            }

            _logger.LogWarning($"Deleting mission with ID {missionId}.");
            return await _repo.DeleteMissionAsync(missionId);
        }

        private void _ValidateMissionDTO(MissionDTO mission)
        {
            if (mission == null)
            {
                _logger.LogWarning("MissionDTO is null.");
                throw new ArgumentNullException(nameof(mission), "Mission cannot be null.");
            }


            if (string.IsNullOrWhiteSpace(mission.Destination))
            {
                throw new ArgumentException("Destination cannot be empty.", nameof(mission.Destination));
            }
            if (mission.StartDate == default || mission.EndDate == default || mission.StartDate > mission.EndDate)
            {
                throw new ArgumentException("Start date cannot be after end date.", nameof(mission));
            }
        }
    }
}
