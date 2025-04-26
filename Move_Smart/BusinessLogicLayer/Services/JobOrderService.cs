using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccessLayer.Repositories;
using Microsoft.Extensions.Logging;

namespace BusinessLayer.Services
{
    public class JobOrderService : ApplicationService
    {
       
        private readonly JobOrderRepo _jobOrderRepo;
        private readonly ILogger<JobOrderService> _jobOrderLogger;
        

        public JobOrderService(JobOrderRepo repo, ApplicationRepo appRepo, ILogger<JobOrderService> logger, ILogger<ApplicationService> appLogger)
            : base(appRepo, appLogger)
        {
            _jobOrderRepo = repo ?? throw new ArgumentNullException(nameof(repo), "Data access layer cannot be null.");
            _jobOrderLogger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger cannot be null.");
        }

        public async Task<int> CreateJobOrderAsync(JobOrderDTO dto)
        {
            if (dto.OrderId != 0)
            {
                _jobOrderLogger.LogWarning("Attempted to create a job order with a non-zero ID.");
                throw new InvalidOperationException("Job order ID must be 0 for new job orders.");
            }

            _ValidateJobOrderDTO(dto);

            if (!(await _shared.CheckVehicleExistsAsync(dto.VehicleId)))
            {
                _jobOrderLogger.LogWarning($"Vehicle with Id {dto.VehicleId} not exist.");
                throw new KeyNotFoundException($"Vehicle with Id {dto.VehicleId} not exist.");
            }

            if (!(await _shared.CheckDriverExistsAsync(dto.DriverId)))
            {
                _jobOrderLogger.LogWarning($"Driver with Id {dto.DriverId} not exist.");
                throw new KeyNotFoundException($"Driver with Id {dto.DriverId} not exist.");
            }
            
            try
            {
                dto.Application.ApplicationId = await CreateApplicationAsync(dto.Application);
                _jobOrderLogger.LogInformation($"Created application with ID {dto.Application.ApplicationId} for job order.");

                _jobOrderLogger.LogInformation("Creating new job order.");
                return await _jobOrderRepo.CreateJobOrderAsync(dto);
            }
            catch (Exception ex)
            {
                _jobOrderLogger.LogError($"Failed to create job order: {ex.Message}.");
                throw;
            }
        }

        public async Task<bool> UpdateJobOrderAsync(JobOrderDTO dto)
        {
            if (dto.OrderId <= 0)
            {
                _jobOrderLogger.LogWarning("Attempted to update a job order with invalid ID.");
                throw new InvalidOperationException("Job order ID must be greater than 0 for updates.");
            }

            _ValidateJobOrderDTO(dto);
            
            var existingJobOrder = await _jobOrderRepo.GetJobOrderByIdAsync(dto.OrderId);
            if (existingJobOrder == null)
            {
                _jobOrderLogger.LogWarning($"Job order with ID {dto.OrderId} not found.");
                throw new KeyNotFoundException($"Job order with ID {dto.OrderId} not found.");
            }


            if (dto.Application != null && dto.Application.ApplicationId > 0)
            {
                dto.Application.CreationDate = existingJobOrder.Application.CreationDate;
                dto.Application.CreatedByUserID= existingJobOrder.Application.CreatedByUserID;

                await UpdateApplicationAsync(dto.Application);
                _jobOrderLogger.LogInformation($"Updated application with ID {dto.Application.ApplicationId} for job order.");
            }

            _jobOrderLogger.LogInformation("Updating job order.");
            return await _jobOrderRepo.UpdateJobOrderAsync(dto);
        }

        public async Task<List<JobOrderDTO>> GetAllJobOrdersAsync(int pageNumber, int pageSize)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                _jobOrderLogger.LogWarning("Invalid pagination parameters.");
                throw new ArgumentException("Page number and page size must be greater than 0.");
            }

            _jobOrderLogger.LogInformation("Retrieving all job orders.");
            return await _jobOrderRepo.GetAllJobOrdersAsync(pageNumber, pageSize);
        }

        public async Task<JobOrderDTO> GetJobOrderByIdAsync(int orderId)
        {
            if (orderId <= 0)
            {
                _jobOrderLogger.LogWarning("Attempted to retrieve a job order with invalid ID.");
                throw new ArgumentException("Order ID must be greater than 0.");
            }

            var jobOrder = await _jobOrderRepo.GetJobOrderByIdAsync(orderId);
            if (jobOrder == null)
            {
                _jobOrderLogger.LogError($"Job order with ID {orderId} not found.");
                throw new KeyNotFoundException($"Job order with ID {orderId} not found.");
            }

            _jobOrderLogger.LogInformation($"Retrieving job order with ID {orderId}.");
            return jobOrder;
        }

        public async Task<List<JobOrderDTO>> GetJobOrdersByVehicleIdAsync(int vehicleId)
        {
            if (vehicleId <= 0)
            {
                _jobOrderLogger.LogWarning("Attempted to retrieve job orders with invalid Vehicle ID.");
                throw new ArgumentException("Vehicle ID must be greater than 0.");
            }
            _jobOrderLogger.LogInformation($"Retrieving job orders for Vehicle ID {vehicleId}.");
            return await _jobOrderRepo.GetJobOrdersByVehicleIdAsync(vehicleId);
        }

        public async Task<List<JobOrderDTO>> GetJobOrdersByDriverIdAsync(int driverId)
        {
            if (driverId <= 0)
            {
                _jobOrderLogger.LogWarning("Attempted to retrieve job orders with invalid driver ID.");
                throw new ArgumentException("Driver ID must be greater than 0.");
            }
            _jobOrderLogger.LogInformation($"Retrieving job orders for driver ID {driverId}.");
            return await _jobOrderRepo.GetJobOrdersByDriverIdAsync(driverId);
        }

        public async Task<List<JobOrderDTO>> GetJobOrdersByStartDateAsync(DateTime startDate)
        {
            if (startDate == default)
            {
                _jobOrderLogger.LogWarning("Attempted to retrieve job orders with invalid start date.");
                throw new ArgumentException("Start date must be a valid date.");
            }
            _jobOrderLogger.LogInformation($"Retrieving job orders starting from {startDate}.");
            return await _jobOrderRepo.GetJobOrdersByStartDateAsync(startDate);
        }

        public async Task<List<JobOrderDTO>> GetJobOrdersByDestinationAsync(string destination)
        {
            if (string.IsNullOrWhiteSpace(destination))
            {
                _jobOrderLogger.LogWarning("Attempted to retrieve job orders with invalid destination.");
                throw new ArgumentException("Destination cannot be null or empty.");
            }
            _jobOrderLogger.LogInformation($"Retrieving job orders with destination {destination}.");
            return await _jobOrderRepo.GetJobOrdersByDestinationAsync(destination);
        }

        public async Task<List<JobOrderDTO>> GetJobOrdersByStatusAsync(enStatus status)
        {
            _jobOrderLogger.LogInformation($"Retrieving job orders with status {status}.");
            return await _jobOrderRepo.GetJobOrdersByStatusAsync(status);
        }

        public async Task<List<JobOrderDTO>> GetJobOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate == default || endDate == default)
            {
                _jobOrderLogger.LogWarning("Attempted to retrieve job orders with invalid date range.");
                throw new ArgumentException("Start date and end date must be valid dates.");
            }
            _jobOrderLogger.LogInformation($"Retrieving job orders between {startDate} and {endDate}.");
            return await _jobOrderRepo.GetJobOrdersByDateRangeAsync(startDate, endDate);
        }

        public async Task<bool> ExsitsAsync(int orderId)
        {
            if (orderId <= 0)
            {
                _jobOrderLogger.LogWarning("Attempted to check existence of a job order with invalid ID.");
                throw new ArgumentException("Order ID must be greater than 0.");
            }

            _jobOrderLogger.LogInformation($"Checking existence of job order with ID {orderId}.");
            return await _jobOrderRepo.ExistsAsync(orderId);
        }

        public async Task<bool> DeleteJobOrderAsync(int orderId)
        {
            if (orderId <= 0)
            {
                _jobOrderLogger.LogWarning("Attempted to delete a job order with invalid ID.");
                throw new ArgumentException("Order ID must be greater than 0.");
            }
            var jobOrder = await _jobOrderRepo.GetJobOrderByIdAsync(orderId);
            if (jobOrder == null)
            {
                _jobOrderLogger.LogError($"Job order with ID {orderId} not found.");
                throw new KeyNotFoundException($"Job order with ID {orderId} not found.");
            }

            
            _jobOrderLogger.LogInformation($"Deleting job order with ID {orderId}.");
            bool jobOrderDeleted = await _jobOrderRepo.DeleteJobOrderAsync(orderId);
            
            if (jobOrderDeleted)
            {
                _jobOrderLogger.LogInformation($"Job order with ID {orderId} deleted successfully.");

                try
                {
                    await DeleteApplicationAsync(jobOrder.Application.ApplicationId);
                    _jobOrderLogger.LogInformation($"Application with ID {jobOrder.Application.ApplicationId} deleted successfully.");
                }
                catch (Exception ex)
                {
                    _jobOrderLogger.LogError($"Failed to delete application with ID {jobOrder.Application.ApplicationId}: {ex.Message}");
                    throw new InvalidOperationException($"Failed to delete application with ID {jobOrder.Application.ApplicationId}.", ex);
                }
            }

            return jobOrderDeleted;
        }

        private void _ValidateJobOrderDTO(JobOrderDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "Job order DTO cannot be null.");

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