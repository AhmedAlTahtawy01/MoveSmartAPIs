using BusinessLayer.Services;
using DataAccessLayer.Repositories;
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

        public MissionService(MissionRepo repo, ILogger<MissionService> logger)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo), "Data access layer cannot be null.");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger cannot be null.");
        }

        public async Task<int> CreateMissionAsync(int missionNoteId, MissionDTO dto)
        {
            if (dto.MissionId != 0)
            {
                _logger.LogWarning("Attempted to create a mission with a non-zero ID.");
                throw new InvalidOperationException("Mission ID must be 0 for new missions.");
            }

            _ValidateMissionDTO(dto);

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

        public async Task<List<MissionDTO>> GetMissionsByVehiclesIdAsync(int missionVehicleId)
        {
            if (missionVehicleId <= 0)
            {
                _logger.LogWarning("Attempted to retrieve missions with invalid Vehicle ID.");
                throw new ArgumentException("Mission Vehicle ID must be greater than 0.");
            }

            _logger.LogInformation($"Retrieving missions for Vehicle ID {missionVehicleId}");
            return await _repo.GetMissionsByVehicleIdAsync(missionVehicleId);
        }



        private void _ValidateMissionDTO(MissionDTO mission)
        {
            if (mission == null)
            {
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
