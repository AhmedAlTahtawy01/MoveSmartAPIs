using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessLogicLayer.Hubs;
using DataAccessLayer.Repositories;
using DataAccessLayer.SharedFunctions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace BusinessLayer.Services
{ 
    public class ApplicationService
    {
        protected readonly ApplicationRepo _repo;
        protected readonly ILogger<ApplicationService> _logger;
        protected readonly SharedFunctions _shared;
        private readonly IHubContext<NotificationHub> _hubContext;


        public ApplicationService(ApplicationRepo repo, ILogger<ApplicationService> logger, SharedFunctions sharedFunctions, IHubContext<NotificationHub> hubContext)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _shared = sharedFunctions ?? throw new ArgumentNullException(nameof(sharedFunctions));
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        }

        public async Task<int> CreateApplicationAsync(ApplicationDTO dto)
        {
            await _ValidateApplicationDTO(dto);

            _logger.LogInformation("Creating new application.");
            int newAppId = await _repo.CreateApplicationAsync(dto);

            string message = $"New application created with ID {newAppId}, Type: {dto.ApplicationType}";

            // Example: send to a specific role group (e.g., "Managers")
            await _hubContext.Clients.Group("Managers").SendAsync("ReceiveNotification", message);
            return newAppId;
        }

        public async Task<bool> UpdateApplicationAsync(ApplicationDTO dto)
        {
            if (dto.ApplicationId <= 0)
            {
                _logger.LogWarning("Attempted to update an application with invalid ID.");
                throw new InvalidOperationException("Application ID must be greater than 0 for updates.");
            }

            await _ValidateApplicationDTO(dto);
            

            var existingApplication = await _repo.GetApplicationByIdAsync(dto.ApplicationId);
            if (existingApplication == null)
            {
                _logger.LogWarning($"Attempted to update a non-existing application with ID {dto.ApplicationId}.");
                throw new KeyNotFoundException($"Application with ID {dto.ApplicationId} does not exist.");
            }

            dto.CreationDate = existingApplication.CreationDate;
            dto.CreatedByUserID = existingApplication.CreatedByUserID;

            _logger.LogInformation($"Updating application with ID {dto.ApplicationId}.");
            return await _repo.UpdateApplicationAsync(dto);
        }

        public async Task<List<ApplicationDTO>> GetAllApplicationsAsync(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                _logger.LogWarning("Invalid pagination parameters.");
                throw new ArgumentException("Page number and page size must be greater than 0.");
            }

            _logger.LogInformation($"Retrieving all applications (Page {pageNumber}, Size {pageSize}).");
            return await _repo.GetAllApplicationsAsync(pageNumber, pageSize);
        }

        protected async Task<ApplicationDTO> GetApplicationByIdAsync(int applicationId)
        {
            if (applicationId <= 0)
            {
                _logger.LogWarning("Attempted to retrieve an application with invalid ID.");
                throw new ArgumentException("Application ID must be greater than 0.");
            }

            var application = await _repo.GetApplicationByIdAsync(applicationId);

            if (application == null)
            {
                _logger.LogError($"Application with ID {applicationId} not found.");
                throw new KeyNotFoundException($"Application with ID {applicationId} not found.");
            }

            _logger.LogInformation($"Retrieving application with ID {applicationId}.");
            return application;
        }

        public async Task<List<ApplicationDTO>> GetApplicationsByApplicationTypeAsync(enApplicationType applicationType)
        {
            _logger.LogInformation($"Retrieving applications with type {applicationType}.");
            return await _repo.GetApplicationsByApplicationTypeAsync(applicationType);
        }

        public async Task<List<ApplicationDTO>> GetApplicationsByUserIdAsync(int userId)
        {
            if (userId <= 0)
            {
                _logger.LogWarning("Attempted to retrieve applications with invalid user ID.");
                throw new ArgumentException("User ID must be greater than 0.");
            }

            if (!(await _shared.CheckUserExistsAsync(userId)))
            {
                _logger.LogWarning($"User with ID: {userId} does not exist.");
                throw new KeyNotFoundException($"User with ID {userId} does not exist.");
            }

            _logger.LogInformation($"Retrieving applications for user with ID {userId}.");
            return await _repo.GetApplicationsByUserIdAsync(userId);
        }

        public async Task<List<ApplicationDTO>> GetApplicationsByStatusAsync(enStatus status)
        {
            _logger.LogInformation($"Retrieving applications with status {status}.");
            return await _repo.GetApplicationsByStatusAsync(status);
        }

        public async Task<int> CountAllApplicationsAsync()
        {
            _logger.LogInformation("Counting all applications.");
            return await _repo.CountAllApplicationsAsync();
        }

        public async Task<int> CountApplicationsByStatusAsync(enStatus status)
        {
            _logger.LogInformation($"Counting applications with status {status}.");
            return await _repo.CountApplicationsByStatusAsync(status);
        }

        public async Task<int> CountApplicationsByTypeAsync(enApplicationType applicationType)
        {
            _logger.LogInformation($"Counting applications with type {applicationType}.");
            return await _repo.CountApplicationsByTypeAsync(applicationType);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Attempted to check existence of an application with invalid ID.");
                throw new ArgumentException("Application ID must be greater than 0.");
            }

            _logger.LogInformation($"Checking existence of application with ID {id}.");
            return await _repo.ExistsAsync(id);
        }

        private async Task<bool> UpdateStatusAsync(int applicationId, enStatus status)
        {
            if (applicationId <= 0)
            {
                _logger.LogWarning("Attempted to update status of an application with invalid ID.");
                throw new ArgumentException("Application ID must be greater than 0.");
            }

            _logger.LogInformation($"Updating status of application with ID {applicationId} to {status}.");
            return await _repo.UpdateStatusAsync(applicationId, status);
        }

        public async Task<bool> ApproveApplicationAsync(int applicationId)
        {
            return await UpdateStatusAsync(applicationId, enStatus.Confirmed);
        }

        public async Task<bool> CancelApplicationAsync(int applicationId)
        {
            return await UpdateStatusAsync(applicationId, enStatus.Canceled);
        }

        public async Task<bool> RejectApplicationAsync(int applicationId)
        {
            return await UpdateStatusAsync(applicationId, enStatus.Rejected);
        }

        public async Task<bool> DeleteApplicationAsync(int applicationId)
        {
            if (applicationId <= 0)
            {
                _logger.LogWarning("Attempted to delete an application with invalid ID.");
                throw new ArgumentException("Application ID must be greater than 0.");
            }

            _logger.LogInformation($"Deleting application with ID {applicationId}.");
            return await _repo.DeleteApplicationAsync(applicationId);
        }

        protected async Task _ValidateApplicationDTO(ApplicationDTO dto)
        {
            if (dto == null)
            {
                _logger.LogWarning("Attempted to process a null application DTO.");
                throw new ArgumentNullException(nameof(dto), "Application DTO cannot be null.");
            }

            if (dto.CreationDate == default)
            {
                _logger.LogWarning("Validation Failed: Creation Date is required.");
                throw new InvalidOperationException("Creation Date is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.ApplicationDescription))
            {
                _logger.LogWarning("Validation Failed: Application Description is required.");
                throw new InvalidOperationException("Application Description is required.");
            }

            if (dto.CreatedByUserID <= 0)
            {
                _logger.LogWarning("Validation Failed: User Id must be greater than 0.");
                throw new InvalidOperationException("User Id must be greater than 0.");
            }

            if (!(await _shared.CheckUserExistsAsync(dto.CreatedByUserID)))
            {
                _logger.LogWarning($"User with ID {dto.CreatedByUserID} does not exist.");
                throw new KeyNotFoundException($"User with ID {dto.CreatedByUserID} does not exist.");
            }
        }
    }
}
