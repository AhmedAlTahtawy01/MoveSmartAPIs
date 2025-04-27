using DataAccessLayer;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class PatrolsSubscriptionService
    {
        protected readonly PatrolsSubscriptionRepo _patrolsSubscriptionRepo;
        protected readonly PatrolRepo _patrolsRepo;
        protected readonly EmployeeRepo _employeeRepo;
        protected readonly ILogger<PatrolsSubscriptionService> _patrolsSubscriptionLogger;

        public PatrolsSubscriptionService(PatrolsSubscriptionRepo patrolsSubscriptionRepo, PatrolRepo patrolRepo, EmployeeRepo employeeRepo, ILogger<PatrolsSubscriptionService> patrolsSubscriptionLogger)
        {
            _patrolsSubscriptionRepo = patrolsSubscriptionRepo ?? throw new ArgumentNullException(nameof(patrolsSubscriptionRepo), "Data access layer cannot be null.");
            _patrolsRepo = patrolRepo ?? throw new ArgumentNullException(nameof(patrolRepo), "Data access layer cannot be null.");
            _employeeRepo = employeeRepo ?? throw new ArgumentNullException(nameof(employeeRepo), "Data access layer cannot be null.");
            _patrolsSubscriptionLogger = patrolsSubscriptionLogger ?? throw new ArgumentNullException(nameof(patrolsSubscriptionLogger), "Logger cannot be null.");
        }

        private async void _ValidatePatrolsSubscriptionDTO(PatrolsSubscriptionDTO dto)
        {
            if (dto == null)
            {
                _patrolsSubscriptionLogger.LogError("PatrolsSubscriptionDTO cannot be null.");
                throw new ArgumentNullException(nameof(dto), "PatrolsSubscriptionDTO cannot be null.");
            }
            if (!await _patrolsRepo.IsPatrolExistsAsync(dto.PatrolID))
            {
                _patrolsSubscriptionLogger.LogError($"Patrol with ID {dto.PatrolID} doesn't exist.");
                throw new ArgumentException(nameof(dto), $"Patrol with ID {dto.PatrolID} doesn't exist.");
            }
            if (!await _employeeRepo.IsEmployeeExistsAsync(dto.EmployeeID))
            {
                _patrolsSubscriptionLogger.LogError($"Employee with ID {dto.EmployeeID} doesn't exist.");
                throw new ArgumentException(nameof(dto), $"Employee with ID {dto.EmployeeID} doesn't exist.");
            }
        }

        public async Task<int?> AddNewPatrolSubscriptionAsync(PatrolsSubscriptionDTO dto)
        {
            try
            {
                _ValidatePatrolsSubscriptionDTO(dto);
            }
            catch (Exception ex)
            {
                _patrolsSubscriptionLogger.LogError(ex, ex.Message);
                return null;
            }

            dto.SubscriptionID = await _patrolsSubscriptionRepo.CreateNewSubscriptionRecordAsync(dto);
            return dto.SubscriptionID;
        }

        public async Task<bool> UpdatePatrolSubscriptionAsync(PatrolsSubscriptionDTO dto)
        {
            try
            {
                _ValidatePatrolsSubscriptionDTO(dto);
            }
            catch (Exception ex)
            {
                _patrolsSubscriptionLogger.LogError(ex, ex.Message);
                return false;
            }

            if (!await _patrolsSubscriptionRepo.IsPatrolSubscriptionExistsAsync(dto.SubscriptionID ?? 0))
            {
                _patrolsSubscriptionLogger.LogError($"Patrol Subscription with ID {dto.SubscriptionID} doesn't exist.");
                return false;
            }

            return await _patrolsSubscriptionRepo.UpdateSubscriptionRecordAsync(dto);
        }

        public async Task<List<PatrolsSubscriptionDTO>> GetAllPatrolsSubscriptionsForEmployeeAsync(int employeeID)
        {
            return await _patrolsSubscriptionRepo.GetAllSubscriptionsForEmployeeAsync(employeeID);
        }

        public async Task<List<PatrolsSubscriptionDTO>> GetAllPatrolsSubscriptionsForPatrolAsync(short patrolID)
        {
            return await _patrolsSubscriptionRepo.GetAllSubscriptionsForPatrolAsync(patrolID);
        }

        public async Task<PatrolsSubscriptionDTO?> GetPatrolSubscriptionByIDAsync(int subscriptionID)
        {
            return await _patrolsSubscriptionRepo.GetSubscriptionRecordByIDAsync(subscriptionID);
        }


        public async Task<bool> IsPatrolSubscriptionExists(int subscriptionID)
        {
            return await _patrolsSubscriptionRepo.IsPatrolSubscriptionExistsAsync(subscriptionID);
        }

        public async Task<bool> DeletePatrolSubscriptionAsync(int subscriptionID)
        {
            return await _patrolsSubscriptionRepo.DeleteSubscriptionRecordAsync(subscriptionID);
        }
    }
}
