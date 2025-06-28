using DataAccessLayer;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class PatrolService
    {
        protected readonly ILogger<PatrolService> _patrolLogger;
        protected readonly PatrolRepo _patrolRepo;
        protected readonly BusRepo _busRepo;

        public PatrolService(PatrolRepo patrolRepo, BusRepo busRepo, ILogger<PatrolService> patrolLogger)
        {
            _patrolRepo = patrolRepo ?? throw new ArgumentNullException(nameof(patrolRepo), "Data access layer cannot be null.");
            _busRepo = busRepo ?? throw new ArgumentNullException(nameof(busRepo), "Data access layer cannot be null.");
            _patrolLogger = patrolLogger ?? throw new ArgumentNullException(nameof(patrolLogger), "Logger cannot be null.");
        }

        private async void _ValidatePatrolDTO(PatrolDTO dto)
        {
            if (dto == null)
            {
                _patrolLogger.LogError("PatrolDTO cannot be null.");
                throw new ArgumentNullException(nameof(dto), "PatrolDTO cannot be null.");
            }
            if (string.IsNullOrWhiteSpace(dto.Description))
            {
                _patrolLogger.LogError("Description cannot be empty.");
                throw new ArgumentException("Description cannot be empty.");
            }
            if (dto.ApproximatedTime <= 0)
            {
                _patrolLogger.LogError("ApproximatedTime must be greater than 0.");
                throw new ArgumentException("ApproximatedTime must be greater than 0.");
            }
            if (!await _busRepo.IsBusExistsAsync(dto.BusID))
            {
                _patrolLogger.LogError($"Bus with ID [{dto.BusID}] does not exist.");
                throw new ArgumentException($"Bus with ID [{dto.BusID}] does not exist.");
            }
        }

        public async Task<short?> AddNewPatrolAsync(PatrolDTO dto)
        {
            try
            {
                _ValidatePatrolDTO(dto);
            }
            catch (Exception ex)
            {
                _patrolLogger.LogError(ex.Message);
                return null;
            }

            if (await _patrolRepo.IsPatrolExistsAsync(dto.PatrolID ?? 0))
            {
                _patrolLogger.LogError($"Patrol with ID [{dto.PatrolID}] already exists.");
                return null;
            }

            dto.PatrolID = await _patrolRepo.AddNewPatrolAsync(dto);
            return dto.PatrolID;
        }

        public async Task<bool> UpdatePatrolAsync(PatrolDTO dto)
        {
            try
            {
                _ValidatePatrolDTO(dto);
            }
            catch (Exception ex)
            {
                _patrolLogger.LogError(ex.Message);
                return false;
            }

            if (!await _patrolRepo.IsPatrolExistsAsync(dto.PatrolID ?? 0))
            {
                _patrolLogger.LogError($"Patrol with ID [{dto.PatrolID}] does not exist.");
                return false;
            }

            return await _patrolRepo.UpdatePatrolAsync(dto);
        }

        public async Task<List<PatrolDTO>> GetAllPatrolsAsync()
        {
            return await _patrolRepo.GetAllPatrolsAsync();
        }

        public async Task<PatrolDTO?> GetPatrolByIDAsync(short patrolID)
        {
            return await _patrolRepo.GetPatrolByIDAsync(patrolID);
        }

        public async Task<bool> IsPatrolExistsAsync(short patrolID)
        {
            return await _patrolRepo.IsPatrolExistsAsync(patrolID);
        }

        public async Task<bool> DeletePatrolAsync(short patrolID)
        {
            return await _patrolRepo.DeletePatrolAsync(patrolID);
        }

        public async Task<int> GetNumberOfPatrols()
        {
            return await _patrolRepo.GetNumberOfPatrolsAsync();
        }
    }
}
