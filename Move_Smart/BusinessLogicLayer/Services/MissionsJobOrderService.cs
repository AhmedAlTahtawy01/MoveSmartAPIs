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
    public class MissionsJobOrderService
    {
        private readonly MissionsJobOrderRepo _repo;
        private readonly ILogger<MissionsJobOrderService> _logger;

        public MissionsJobOrderService(MissionsJobOrderRepo repo, ILogger<MissionsJobOrderService> logger)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo), "Data access layer cannot be null.");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger cannot be null.");
        }

        public async Task<int> CreateMissionJobOrderAsync(MissionsJobOrderDTO dto)
        {
            if (dto.OrderId != 0)
            {
                _logger.LogWarning("Attempted to create a MissionJobOrder with a non-zero ID.");
                throw new InvalidOperationException("MissionJobOrder ID must be 0 for new orders.");
            }

            _ValidateMissionsJobOrder(dto);

            _logger.LogInformation("Creating new missionJobOrder.");
            return await _repo.CreateMissionsJobOrderAsync(dto);
        }

        public async Task<bool> UpdateMissionJobOrderAsync(MissionsJobOrderDTO dto)
        {
            if (dto.OrderId <= 0)
            {
                _logger.LogWarning("Attempted to update a missionJobOrder with invalid ID.");
                throw new InvalidOperationException("MissionJobOrder ID must be greater than 0.");
            }

            _ValidateMissionsJobOrder(dto);

            var existingMissionJobOrder = await _repo.GetMissionsJobOrderByIdAsync(dto.OrderId);
            if (existingMissionJobOrder == null)
            {
                _logger.LogWarning($"No MissionJobOrder found with ID {dto.OrderId}.");
                throw new KeyNotFoundException($"No MissionJobOrder found with ID {dto.OrderId}.");
            }

            _logger.LogInformation($"Updating missionJobOrder with ID {dto.OrderId}.");
            return await _repo.UpdateMissionsJobOrderAsync(dto);
        }

        public async Task<List<MissionsJobOrderDTO>> GetAllMissionsJobOrderAsync(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize < 1)
                throw new ArgumentException("Page number and page size must be greater than 0.");

            try
            {
                return await _repo.GetAllMissionsJobOrdersAsync(pageNumber, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all missionsJobOrder.");
                throw;
            }
        }

        public async Task<MissionsJobOrderDTO> GetMissionsJobOrderByIdAsync(int orderId)
        {
            if (orderId <= 0)
            {
                _logger.LogWarning("Attempted to retrieve a missionJobOrder invalid ID.");
                throw new ArgumentException("MissionJobOrder ID must be greater than 0.");
            }

            var missionJobOrder = await _repo.GetMissionsJobOrderByIdAsync(orderId);
            if (missionJobOrder == null)
            {
                _logger.LogError($"MissionJobOrder with ID {orderId} not found.");
                throw new KeyNotFoundException($"MissionJobOrder with ID {orderId} not found.");
            }

            _logger.LogInformation($"Retrieving MissionJobOrder with ID {orderId}.");
            return missionJobOrder;
        }

        private void _ValidateMissionsJobOrder(MissionsJobOrderDTO dto)
        {
            if (dto == null)
            {
                _logger.LogWarning("Validation Failed: Attempted to work on null MissionsJobOrder DTO.");
                throw new ArgumentException(nameof(dto), "MissionsJobOrder DTO cannot be null.");
            }

            if (dto.JobOrderId <= 0)
            {
                _logger.LogWarning("Validation Failed: JobOrder ID must be greater than 0.");
                throw new InvalidOperationException("JobOrder ID must be greater than 0.");
            }

            if (dto.MissionId <= 0)
            {
                _logger.LogWarning("Validation Failed: Mission ID must be greater than 0.");
                throw new InvalidOperationException("Mission ID must be greater than 0.");
            }
        }
    }
}
