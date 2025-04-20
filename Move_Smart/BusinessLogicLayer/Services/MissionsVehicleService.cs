//using BusinessLayer.Services;
//using DataAccessLayer.Repositories;
//using Microsoft.Extensions.Logging;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace BusinessLogicLayer.Services
//{
//    public class MissionsVehicleService
//    {
//        private readonly MissionsVehicleRepo _repo;
//        private readonly ILogger<MissionsVehicleService> _logger;

//        public MissionsVehicleService(MissionsVehicleRepo repo, ILogger<MissionsVehicleService> logger)
//        {
//            _repo = repo ?? throw new ArgumentNullException(nameof(repo), "Data access layer cannot be null.");
//            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger cannot be null.");
//        }

//        private void _ValidateMissionsVehicleDTO(MissionsVehicleDTO dto)
//        {
//            if (dto == null)
//            {
//                _logger.LogWarning("MissionsVehicleDTO is null.");
//                throw new ArgumentNullException(nameof(dto), "MissionsVehicleDTO cannot be null.");
//            }

//            if (dto.MissionId <= 0)
//            {
//                _logger.LogWarning("MissionId is invalid.");
//                throw new ArgumentException("MissionId must be greater than 0.", nameof(dto.MissionId));
//            }

//            if (dto.VehicleId <= 0)
//            {
//                _logger.LogWarning("VehicleId is invalid.");
//                throw new ArgumentException("VehicleId must be greater than 0.", nameof(dto.VehicleId));
//            }
//        }

//        public async Task<int> CreateMissionsVehicleAsync(int missionId, int vehicleId, MissionsVehicleDTO dto)
//        {
//            if (dto.MissionVehicleId != 0)
//            {
//                _logger.LogWarning("Attempted to create a MissionsVehicle with a non-zero ID.");
//                throw new InvalidOperationException("MissionsVehicle ID must be 0 for new missions vehicles.");
//            }

//            dto.MissionId = missionId;
//            dto.VehicleId = vehicleId;

//            _ValidateMissionsVehicleDTO(dto);

//            _logger.LogInformation("Creating new MissionsVehicle.");
//            return await _repo.CreateMissionVehicleAsync(dto);
//        }

//        public async Task<bool> UpdateMissionsVehicleAsync(MissionsVehicleDTO dto)
//        {
//            if (dto.MissionVehicleId <= 0)
//            {
//                _logger.LogWarning("Attempted to update a MissionsVehicle with invalid ID.");
//                throw new InvalidOperationException("MissionsVehicle ID must be greater than 0 for updates.");
//            }

//            _ValidateMissionsVehicleDTO(dto);

//            var existingMissionsVehicle = await _repo.GetMissionVehicleByIdAsync(dto.MissionVehicleId);
//            if (existingMissionsVehicle == null)
//            {
//                _logger.LogWarning($"No MissionsVehicle found with ID {dto.MissionVehicleId} for update.");
//                throw new KeyNotFoundException($"No MissionsVehicle found with ID {dto.MissionVehicleId}.");
//            }

//            _logger.LogInformation($"Updating MissionsVehicle with ID {dto.MissionVehicleId}.");
//            return await _repo.UpdateMissionsVehicleAsync(dto);
//        }

//        public async Task<List<MissionsVehicleDTO>> GetAllMissionsVehiclesAsync(int pageNumber = 1, int pageSize = 10)
//        {
//            if (pageNumber < 1 || pageSize < 1)
//            {
//                _logger.LogWarning("Invalid pagination parameters.");
//                throw new ArgumentException("Page number and page size must be greater than 0.");
//            }

//            _logger.LogInformation("Retrieving all MissionsVehicles.");
//            return await _repo.GetAllMissionVehiclesAsync(pageNumber, pageSize);
//        }

//        public async Task<MissionsVehicleDTO> GetMissionsVehicleByIdAsync(int missionVehicleId)
//        {
//            if (missionVehicleId <= 0)
//            {
//                _logger.LogWarning("Invalid MissionsVehicle ID.");
//                throw new ArgumentException("MissionsVehicle ID must be greater than 0.", nameof(missionVehicleId));
//            }

//            var missionsVehicle = await _repo.GetMissionVehicleByIdAsync(missionVehicleId);
//            if (missionsVehicle == null)
//            {
//                _logger.LogWarning($"No MissionsVehicle found with ID {missionVehicleId}.");
//                throw new KeyNotFoundException($"No MissionsVehicle found with ID {missionVehicleId}.");
//            }

//            _logger.LogInformation($"Retrieving MissionsVehicle with ID {missionVehicleId}.");
//            return missionsVehicle;
//        }

//        public async Task<List<MissionsVehicleDTO>> GetMissionsVehiclesByMissionIdAsync(int missionId)
//        {
//            if (missionId <= 0)
//            {
//                _logger.LogWarning("Attempted to retrieve MissionsVehicles with invalid MissionId.");
//                throw new ArgumentException("MissionId must be greater than 0.", nameof(missionId));
//            }

//            _logger.LogInformation($"Retrieving MissionsVehicles for MissionId {missionId}.");
//            return await _repo.GetMissionVehiclesByMissionIdAsync(missionId);
//        }

//        public async Task<List<MissionsVehicleDTO>> GetMissionsVehiclesByVehicleIdAsync(int vehicleId)
//        {
//            if (vehicleId <= 0)
//            {
//                _logger.LogWarning("Attempted to retrieve MissionsVehicles with invalid VehicleId.");
//                throw new ArgumentException("VehicleId must be greater than 0.", nameof(vehicleId));
//            }

//            _logger.LogInformation($"Retrieving MissionsVehicles for VehicleId {vehicleId}.");
//            return await _repo.GetMissionVehiclesByVehicleIdAsync(vehicleId);
//        }

//        public async Task<bool> DeleteMissionsVehicleAsync(int missionVehicleId)
//        {
//            if (missionVehicleId <= 0)
//            {
//                _logger.LogWarning("Attempted to delete a MissionsVehicle with invalid ID.");
//                throw new ArgumentException("MissionsVehicle ID must be greater than 0.", nameof(missionVehicleId));
//            }

//            var existingMissionsVehicle = await _repo.GetMissionVehicleByIdAsync(missionVehicleId);
//            if (existingMissionsVehicle == null)
//            {
//                _logger.LogWarning($"No MissionsVehicle found with ID {missionVehicleId} for deletion.");
//                throw new KeyNotFoundException($"No MissionsVehicle found with ID {missionVehicleId}.");
//            }

//            _logger.LogInformation($"Deleting MissionsVehicle with ID {missionVehicleId}.");
//            return await _repo.DeleteMissionsVehicleAsync(missionVehicleId);
//        }
//    }
//}
