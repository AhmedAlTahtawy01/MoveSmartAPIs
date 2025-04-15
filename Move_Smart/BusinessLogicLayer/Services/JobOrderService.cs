using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccessLayer.Repositories;
using Microsoft.Extensions.Logging;

namespace BusinessLayer.Services
{
    public class JobOrderService : ApplicationService
    {
        
        private readonly JobOrderRepo _repo;
        private readonly ILogger<JobOrderService> _logger;

        public JobOrderService(JobOrderRepo repo, ApplicationRepo appRepo, ILogger<JobOrderService> logger, ILogger<ApplicationService> appLogger)
            : base(appRepo, appLogger)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo), "Data access layer cannot be null.");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger cannot be null.");
        }

        public async Task<int> CreateJobOrderAsync(JobOrderDTO dto)
        {
            if (dto.OrderId != 0)
            {
                _logger.LogWarning("Attempted to create a job order with a non-zero ID.");
                throw new InvalidOperationException("Job order ID must be 0 for new job orders.");
            }

            _ValidateJobOrderDTO(dto);
            
            try
            {
                dto.Application.ApplicationId = await CreateApplicationAsync(dto.Application);
                _logger.LogInformation($"Created application with ID {dto.Application.ApplicationId} for job order.");

                _logger.LogInformation("Creating new job order.");
                return await _repo.CreateJobOrderAsync(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to create job order: {ex.Message}.");
                throw;
            }
        }

        public async Task<bool> UpdateJobOrderAsync(JobOrderDTO dto)
        {
            if (dto.OrderId <= 0)
            {
                _logger.LogWarning("Attempted to update a job order with invalid ID.");
                throw new InvalidOperationException("Job order ID must be greater than 0 for updates.");
            }

            _ValidateJobOrderDTO(dto);
            
            var existingJobOrder = await _repo.GetJobOrderByIdAsync(dto.OrderId);
            if (existingJobOrder == null)
            {
                _logger.LogWarning($"Job order with ID {dto.OrderId} not found.");
                throw new KeyNotFoundException($"Job order with ID {dto.OrderId} not found.");
            }


            if (dto.Application != null && dto.Application.ApplicationId > 0)
            {
                dto.Application.CreationDate = existingJobOrder.Application.CreationDate;
                dto.Application.CreatedByUserID= existingJobOrder.Application.CreatedByUserID;

                await UpdateApplicationAsync(dto.Application);
                _logger.LogInformation($"Updated application with ID {dto.Application.ApplicationId} for job order.");
            }

            _logger.LogInformation("Updating job order.");
            return await _repo.UpdateJobOrderAsync(dto);
        }

        public async Task<List<JobOrderDTO>> GetAllJobOrdersAsync(int pageNumber, int pageSize)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                _logger.LogWarning("Invalid pagination parameters.");
                throw new ArgumentException("Page number and page size must be greater than 0.");
            }

            _logger.LogInformation("Retrieving all job orders.");
            return await _repo.GetAllJobOrdersAsync(pageNumber, pageSize);
        }

        public async Task<JobOrderDTO> GetJobOrderByIdAsync(int orderId)
        {
            if (orderId <= 0)
            {
                _logger.LogWarning("Attempted to retrieve a job order with invalid ID.");
                throw new ArgumentException("Order ID must be greater than 0.");
            }

            var jobOrder = await _repo.GetJobOrderByIdAsync(orderId);
            if (jobOrder == null)
            {
                _logger.LogError($"Job order with ID {orderId} not found.");
                throw new KeyNotFoundException($"Job order with ID {orderId} not found.");
            }

            _logger.LogInformation($"Retrieving job order with ID {orderId}.");
            return jobOrder;
        }

        public async Task<List<JobOrderDTO>> GetJobOrdersByVehicleIdAsync(int vehicleId)
        {
            if (vehicleId <= 0)
            {
                _logger.LogWarning("Attempted to retrieve job orders with invalid Vehicle ID.");
                throw new ArgumentException("Vehicle ID must be greater than 0.");
            }
            _logger.LogInformation($"Retrieving job orders for Vehicle ID {vehicleId}.");
            return await _repo.GetJobOrdersByVehicleIdAsync(vehicleId);
        }

        public async Task<List<JobOrderDTO>> GetJobOrdersByDriverIdAsync(int driverId)
        {
            if (driverId <= 0)
            {
                _logger.LogWarning("Attempted to retrieve job orders with invalid driver ID.");
                throw new ArgumentException("Driver ID must be greater than 0.");
            }
            _logger.LogInformation($"Retrieving job orders for driver ID {driverId}.");
            return await _repo.GetJobOrdersByDriverIdAsync(driverId);
        }

        public async Task<List<JobOrderDTO>> GetJobOrdersByStartDateAsync(DateTime startDate)
        {
            if (startDate == default)
            {
                _logger.LogWarning("Attempted to retrieve job orders with invalid start date.");
                throw new ArgumentException("Start date must be a valid date.");
            }
            _logger.LogInformation($"Retrieving job orders starting from {startDate}.");
            return await _repo.GetJobOrdersByStartDateAsync(startDate);
        }

        public async Task<List<JobOrderDTO>> GetJobOrdersByDestinationAsync(string destination)
        {
            if (string.IsNullOrWhiteSpace(destination))
            {
                _logger.LogWarning("Attempted to retrieve job orders with invalid destination.");
                throw new ArgumentException("Destination cannot be null or empty.");
            }
            _logger.LogInformation($"Retrieving job orders with destination {destination}.");
            return await _repo.GetJobOrdersByDestinationAsync(destination);
        }

        public async Task<List<JobOrderDTO>> GetJobOrdersByStatusAsync(enStatus status)
        {
            _logger.LogInformation($"Retrieving job orders with status {status}.");
            return await _repo.GetJobOrdersByStatusAsync(status);
        }

        public async Task<List<JobOrderDTO>> GetJobOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate == default || endDate == default)
            {
                _logger.LogWarning("Attempted to retrieve job orders with invalid date range.");
                throw new ArgumentException("Start date and end date must be valid dates.");
            }
            _logger.LogInformation($"Retrieving job orders between {startDate} and {endDate}.");
            return await _repo.GetJobOrdersByDateRangeAsync(startDate, endDate);
        }

        public async Task<bool> DeleteJobOrderAsync(int orderId)
        {
            if (orderId <= 0)
            {
                _logger.LogWarning("Attempted to delete a job order with invalid ID.");
                throw new ArgumentException("Order ID must be greater than 0.");
            }
            var jobOrder = await _repo.GetJobOrderByIdAsync(orderId);
            if (jobOrder == null)
            {
                _logger.LogError($"Job order with ID {orderId} not found.");
                throw new KeyNotFoundException($"Job order with ID {orderId} not found.");
            }

            
            _logger.LogInformation($"Deleting job order with ID {orderId}.");
            bool jobOrderDeleted = await _repo.DeleteJobOrderAsync(orderId);
            
            if (jobOrderDeleted)
            {
                _logger.LogInformation($"Job order with ID {orderId} deleted successfully.");

                try
                {
                    await DeleteApplicationAsync(jobOrder.Application.ApplicationId);
                    _logger.LogInformation($"Application with ID {jobOrder.Application.ApplicationId} deleted successfully.");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Failed to delete application with ID {jobOrder.Application.ApplicationId}: {ex.Message}");
                    throw new InvalidOperationException($"Failed to delete application with ID {jobOrder.Application.ApplicationId}.", ex);
                }
            }

            return jobOrderDeleted;
        }

        private void _ValidateJobOrderDTO(JobOrderDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "Job order DTO cannot be null.");

            _ValidateApplicationDTO(dto.Application);

            if (dto.VehicleId <= 0)
                throw new InvalidOperationException("Vehicle ID must be greater than 0.");

            if (dto.DriverId <= 0)
                throw new InvalidOperationException("Driver ID must be greater than 0.");

            if (dto.StartDate == default)
                throw new InvalidOperationException("Start date is required.");

            if (dto.EndDate == default)
                throw new InvalidOperationException("End date is required.");

            if (string.IsNullOrWhiteSpace(dto.Destination))
                throw new InvalidOperationException("Destination is required.");

            if (dto.OdometerBefore < 0)
                throw new InvalidOperationException("Odometer before must be greater than or equal to 0.");

            if (dto.OdometerAfter.HasValue && dto.OdometerAfter < 0)
                throw new InvalidOperationException("Odometer after must be greater than or equal to 0.");
        }

    }
}