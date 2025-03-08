using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccessLayer.Repositories;
using Microsoft.Extensions.Logging;

namespace BusinessLayer.Services
{
    public class JobOrder : Application
    {
        // Private properties
        private readonly JobOrderRepo _jobOrderDAL;
        private readonly ILogger<JobOrder> _jobOrderLogger;

        // Public properties
        public int OrderId { get; set; }
        public int VehicleId { get; set; }
        public int DriverId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Destination { get; set; }
        public int OdometerBefore { get; set; }
        public int OdometerAfter { get; set; }

        // Database properties
        public enum EnMode { AddNew = 0, Update = 1 }
        public EnMode Mode { get; private set; } = EnMode.AddNew;
        public JobOrderDTO JobOrderDTO => new JobOrderDTO(
            OrderId,
            ApplicationId,
            VehicleId,
            DriverId,
            StartDate,
            EndDate,
            StartTime,
            EndTime,
            Destination,
            OdometerBefore,
            OdometerAfter
            );

        // Constructor
        public JobOrder(JobOrderDTO jobOrderDTO, JobOrderRepo dal, ILogger<JobOrder> logger,
            ApplicationDTO applicationDTO, ApplicationRepo applicationDal, ILogger<Application> applicationLogger
            , EnMode mode = EnMode.AddNew,
             Application.EnMode applicaitonMode = Application.EnMode.AddNew)
            : base(applicationDTO, applicationDal, applicationLogger, applicaitonMode)
        {
            OrderId = jobOrderDTO.OrderId;
            ApplicationId = jobOrderDTO.ApplicationId;
            VehicleId = jobOrderDTO.VehicleId;
            DriverId = jobOrderDTO.DriverId;
            StartDate = jobOrderDTO.StartDate;
            EndDate = jobOrderDTO.EndDate;
            StartTime = jobOrderDTO.StartTime;
            EndTime = jobOrderDTO.EndTime;
            Destination = jobOrderDTO.Destination;
            OdometerBefore = jobOrderDTO.OdometerBefore;
            OdometerAfter = jobOrderDTO.OdometerAfter;

            _jobOrderDAL = dal;
            _jobOrderLogger = logger;
            Mode = mode;
        }

        // Private methods
        private bool _JobOrderValidations()
        {
            if (OrderId < 0)
                throw new Exception("Order ID must be greater than or equal to 0.");

            if (ApplicationId <= 0)
                throw new Exception("Application ID must be greater than 0.");

            if (VehicleId <= 0)
                throw new Exception("Vehicle ID must be greater than 0.");

            if (DriverId <= 0)
                throw new Exception("Driver ID must be greater than 0.");

            if (StartDate == default)
                throw new Exception("Start date is required.");

            if (EndDate == default)
                throw new Exception("End date is required.");

            if (string.IsNullOrWhiteSpace(Destination))
                throw new Exception("Destination is required.");

            if (OdometerBefore < 0)
                throw new Exception("Odometer before must be greater than or equal to 0.");

            if (OdometerAfter < 0)
                throw new Exception("Odometer after must be greater than or equal to 0.");

            return true;
        }

        private async Task<bool> _CreateJobOrderAsync()
        {
            if (await _jobOrderDAL.GetJobOrderByIdAsync(OrderId) != null)
                throw new Exception($"Job order with ID {OrderId} already exists.");

            OrderId = await _jobOrderDAL.CreateJobOrderAsync(JobOrderDTO);
            if (OrderId > 0)
            {
                Mode = EnMode.Update;
                return true;
            }

            return false;
        }

        private async Task<bool> _UpdateJobOrderAsync()
        {
            var existingJobOrder = await _jobOrderDAL.GetJobOrderByIdAsync(OrderId);
            if (existingJobOrder == null)
                throw new Exception("Job order not found.");

            // Ensure the user can't change certain fields (e.g., ApplicationId, VehicleId, DriverId)
            ApplicationId = existingJobOrder.ApplicationId;
            VehicleId = existingJobOrder.VehicleId;
            DriverId = existingJobOrder.DriverId;

            return await _jobOrderDAL.UpdateJobOrderAsync(JobOrderDTO);
        }

        // Public methods
        public async Task<bool> SaveAsync()
        {
            if (!_JobOrderValidations())
                throw new Exception("Job order validations failed.");

            if (Mode == EnMode.Update)
            {
                return await _UpdateJobOrderAsync();
            }
            else
            {
                return await _CreateJobOrderAsync();
            }
        }

        public async Task<List<JobOrder>> GetAllJobOrdersAsync(int pageNumber = 1, int pageSize = 10)
        {
            
            var jobOrderDTOs = await _jobOrderDAL.GetAllJobOrdersAsync(pageNumber, pageSize);
            var jobOrdersList = new List<JobOrder>();

            foreach (var jobOrderDTO in jobOrderDTOs)
            {
                var applicationDTO = await _dal.GetApplicationByIdAsync(jobOrderDTO.ApplicationId);
                jobOrdersList.Add(new JobOrder(jobOrderDTO, _jobOrderDAL, _jobOrderLogger, applicationDTO, _dal, _logger, EnMode.Update));
            }

            return jobOrdersList;
        }

        //public async Task<JobOrder> GetJobOrderByIdAsync(int orderId)
        //{
        //    if (orderId <= 0)
        //        throw new Exception("Order ID must be greater than 0.");

        //    var jobOrderDTO = await _jobOrderDAL.GetJobOrderByIdAsync(orderId);
        //    return jobOrderDTO != null ? new JobOrder(jobOrderDTO, _jobOrderDAL, _jobOrderLogger, EnMode.Update) : null;
        //}

        //public async Task<List<JobOrder>> GetJobOrdersByApplicationIdAsync(int applicationId)
        //{
        //    if (applicationId <= 0)
        //        throw new Exception("Application ID must be greater than 0.");

        //    var jobOrderDTOs = await _jobOrderDAL.GetJobOrdersByApplicationIdAsync(applicationId);
        //    var jobOrdersList = new List<JobOrder>();

        //    foreach (var jobOrderDTO in jobOrderDTOs)
        //    {
        //        jobOrdersList.Add(new JobOrder(jobOrderDTO, _jobOrderDAL, _jobOrderLogger, EnMode.Update));
        //    }

        //    return jobOrdersList;
        //}

        //public async Task<List<JobOrder>> GetJobOrdersByVehicleIdAsync(int vehicleId)
        //{
        //    if (vehicleId <= 0)
        //        throw new Exception("Vehicle ID must be greater than 0.");

        //    var jobOrderDTOs = await _jobOrderDAL.GetJobOrdersByVehicleIdAsync(vehicleId);
        //    var jobOrdersList = new List<JobOrder>();

        //    foreach (var jobOrderDTO in jobOrderDTOs)
        //    {
        //        jobOrdersList.Add(new JobOrder(jobOrderDTO, _jobOrderDAL, _jobOrderLogger, EnMode.Update));
        //    }

        //    return jobOrdersList;
        //}

        //public async Task<List<JobOrder>> GetJobOrdersByDriverIdAsync(int driverId)
        //{
        //    if (driverId <= 0)
        //        throw new Exception("Driver ID must be greater than 0.");

        //    var jobOrderDTOs = await _jobOrderDAL.GetJobOrdersByDriverIdAsync(driverId);
        //    var jobOrdersList = new List<JobOrder>();

        //    foreach (var jobOrderDTO in jobOrderDTOs)
        //    {
        //        jobOrdersList.Add(new JobOrder(jobOrderDTO, _jobOrderDAL, _jobOrderLogger, EnMode.Update));
        //    }

        //    return jobOrdersList;
        //}

        //public async Task<List<JobOrder>> GetJobOrdersByStartDateAsync(DateTime startDate)
        //{
        //    var jobOrderDTOs = await _jobOrderDAL.GetJobOrdersByStartDateAsync(startDate);
        //    var jobOrdersList = new List<JobOrder>();

        //    foreach (var jobOrderDTO in jobOrderDTOs)
        //    {
        //        jobOrdersList.Add(new JobOrder(jobOrderDTO, _jobOrderDAL, _jobOrderLogger, EnMode.Update));
        //    }

        //    return jobOrdersList;
        //}

        //public async Task<List<JobOrder>> GetJobOrdersByDestinationAsync(string destination)
        //{
        //    if (string.IsNullOrWhiteSpace(destination))
        //        throw new Exception("Destination cannot be null or empty.");

        //    var jobOrderDTOs = await _jobOrderDAL.GetJobOrdersByDestinationAsync(destination);
        //    var jobOrdersList = new List<JobOrder>();

        //    foreach (var jobOrderDTO in jobOrderDTOs)
        //    {
        //        jobOrdersList.Add(new JobOrder(jobOrderDTO, _jobOrderDAL, _jobOrderLogger, EnMode.Update));
        //    }

        //    return jobOrdersList;
        //}

        //public async Task<int> CountAllJobOrdersAsync()
        //{
        //    return await _jobOrderDAL.CountAllJobOrdersAsync();
        //}

        //public async Task<int> CountJobOrdersByStatusAsync(enStatus status)
        //{
        //    return await _jobOrderDAL.CountJobOrdersByStatusAsync(status);
        //}

        //public async Task<int> CountJobOrdersByTypeAsync(int applicationType)
        //{
        //    if (applicationType <= 0)
        //        throw new Exception("Application type must be greater than 0.");

        //    return await _jobOrderDAL.CountJobOrdersByTypeAsync(applicationType);
        //}

        //public async Task<bool> UpdateStatusAsync(int orderId, enStatus status)
        //{
        //    if (orderId <= 0)
        //        throw new Exception("Order ID must be greater than 0.");

        //    return await _jobOrderDAL.UpdateStatusAsync(orderId, status);
        //}

        public async Task<bool> DeleteJobOrderAsync(int orderId)
        {
            if (orderId <= 0)
                throw new Exception("Order ID must be greater than 0.");

            return await _jobOrderDAL.DeleteJobOrderAsync(orderId);
        }
    }
}