using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccessLayer.Repositories;
using Microsoft.Extensions.Logging;

namespace BusinessLayer.Services
{ 
    public class ApplicationService
    {
        private readonly ApplicationRepo _repo;
        private readonly ILogger<ApplicationService> _logger;

        protected ApplicationService(ApplicationRepo repo, ILogger<ApplicationService> logger)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo), "Data access layer cannot be null.");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger cannot be null.");
        }

        protected async Task<int> CreateApplicationAsync(ApplicationDTO dto)
        {
            if (dto == null)
            {
                _logger.LogWarning("Attempted to create a null application DTO.");
                throw new ArgumentNullException(nameof(dto), "Application DTO cannot be null.");
            }
            
            if (dto.ApplicationId != 0)
            {
                _logger.LogWarning("Attempted to create an application with a non-zero ID.");
                throw new InvalidOperationException("Application ID must be 0 for new applications.");
            }

            try
            {
                _ValidateApplicationDTO(dto);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning($"Validation failed: {ex.Message}");
                throw;
            }


            _logger.LogInformation("Creating new application.");
            return await _repo.CreateApplicationAsync(dto);
        }

        protected async Task<bool> UpdateApplicationAsync(ApplicationDTO dto)
        {
            if (dto == null)
            {
                _logger.LogWarning("Attempted to update a null application DTO.");
                throw new ArgumentNullException(nameof(dto), "Application DTO cannot be null.");
            }

            if (dto.ApplicationId <= 0)
            {
                _logger.LogWarning("Attempted to update an application with invalid ID.");
                throw new InvalidOperationException("Application ID must be greater than 0 for updates.");
            }

            try
            {
                _ValidateApplicationDTO(dto);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning($"Validation failed: {ex.Message}");
                throw;
            }

            var existingApplication = await _repo.GetApplicationByIdAsync(dto.ApplicationId);
            if (existingApplication == null)
            {
                _logger.LogWarning($"Attempted to update a non-existing application with ID {dto.ApplicationId}.");
                throw new KeyNotFoundException($"Application with ID {dto.ApplicationId} does not exist.");
            }

            dto.CreationDate = existingApplication.CreationDate;
            dto.UserId = existingApplication.UserId;

            _logger.LogInformation($"Updating application with ID {dto.ApplicationId}.");
            return await _repo.UpdateApplicationAsync(dto);
        }

        protected async Task<List<ApplicationDTO>> GetAllApplicationsAsync(int pageNumber = 1, int pageSize = 10)
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

        protected async Task<List<ApplicationDTO>> GetApplicationsByApplicationTypeAsync(enApplicationType applicationType)
        {
            _logger.LogInformation($"Retrieving applications with type {applicationType}.");
            return await _repo.GetApplicationsByApplicationTypeAsync(applicationType);
        }

        protected async Task<List<ApplicationDTO>> GetApplicationsByUserIdAsync(int userId)
        {
            if (userId <= 0)
            {
                _logger.LogWarning("Attempted to retrieve applications with invalid user ID.");
                throw new ArgumentException("User ID must be greater than 0.");
            }

            _logger.LogInformation($"Retrieving applications for user with ID {userId}.");
            return await _repo.GetApplicationsByUserIdAsync(userId);
        }

        protected async Task<List<ApplicationDTO>> GetApplicationsByStatusAsync(enStatus status)
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

        protected async Task<bool> UpdateStatusAsync(int applicationId, enStatus status)
        {
            if (applicationId <= 0)
            {
                _logger.LogWarning("Attempted to update status of an application with invalid ID.");
                throw new ArgumentException("Application ID must be greater than 0.");
            }

            _logger.LogInformation($"Updating status of application with ID {applicationId} to {status}.");
            return await _repo.UpdateStatusAsync(applicationId, status);
        }

        protected async Task<bool> DeleteApplicationAsync(int applicationId)
        {
            if (applicationId <= 0)
            {
                _logger.LogWarning("Attempted to delete an application with invalid ID.");
                throw new ArgumentException("Application ID must be greater than 0.");
            }

            _logger.LogInformation($"Deleting application with ID {applicationId}.");
            return await _repo.DeleteApplicationAsync(applicationId);
        }

        protected void _ValidateApplicationDTO(ApplicationDTO dto)
        {
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

            if (dto.UserId <= 0)
            {
                _logger.LogWarning("Validation Failed: User Id must be greater than 0.");
                throw new InvalidOperationException("User Id must be greater than 0.");
            }
        }
    }
}
