using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Repositories;
using Microsoft.Extensions.Logging;

namespace BusinessLayer.Services
{
    public class Application
    {
        // Private properties
        protected readonly ApplicationRepo _dal;
        protected readonly ILogger<Application> _logger;

        // Public properties
        public int ApplicationId { get; set; }
        public DateTime CreationDate { get; set; }
        public enStatus Status { get; set; }
        public int ApplicationType { get; set; }
        public string ApplicationDescription { get; set; }
        public int UserId { get; set; }

        // Database properties
        public enum EnMode
        {
            AddNew = 0,
            Update = 1
        }

        public EnMode Mode { get; private set; } = EnMode.AddNew;
        public ApplicationDTO ApplicationDTO => new ApplicationDTO(ApplicationId, CreationDate, Status, ApplicationType, ApplicationDescription, UserId);

        // Constructor
        public Application(ApplicationDTO applicationDTO, ApplicationRepo dal, ILogger<Application> logger, EnMode mode = EnMode.AddNew)
        {
            ApplicationId = applicationDTO.ApplicationId;
            CreationDate = applicationDTO.CreationDate;
            Status = applicationDTO.Status;
            ApplicationType = applicationDTO.ApplicationType;
            ApplicationDescription = applicationDTO.ApplicationDescription;
            UserId = applicationDTO.UserId;

            _dal = dal;
            _logger = logger;
            Mode = mode;
        }

        // Private methods
        private bool _ApplicationValidations()
        {
            if (Mode == EnMode.AddNew && ApplicationId > 0)
                throw new Exception("Application ID must be 0 for new applications.");

            if (Mode == EnMode.Update && ApplicationId <= 0)
                throw new Exception("Application Id must be greater than 0");

            if (CreationDate == default)
                throw new Exception("Creation Date is required");

            if (string.IsNullOrWhiteSpace(ApplicationDescription))
                throw new Exception("Application Description is required");

            if (ApplicationType <= 0)
                throw new Exception("Application Type must be greater than 0");

            if (UserId <= 0)
                throw new Exception("User Id must be greater than 0");

            return true;
        }

        private async Task <bool> _CreateApplicationAsync()
        {
            if (await _dal.GetApplicationByIdAsync(ApplicationId) != null)
                throw new Exception("Application Id already exists");

            ApplicationId = await _dal.CreateApplicationAsync(ApplicationDTO);
            if (ApplicationId > 0)
            {
                Mode = EnMode.Update;
                return true;
            }

            return false;
        }

        private async Task <bool> _UpdateApplicationAsync()
        {
            var exsitingApplication = await _dal.GetApplicationByIdAsync(ApplicationId);
            if (exsitingApplication == null)
                throw new Exception("Application does not exist");

            CreationDate = exsitingApplication.CreationDate;
            UserId = exsitingApplication.UserId;

            return await _dal.UpdateApplicationAsync(ApplicationDTO);
        }


        // Public methods
        public async Task<bool> SaveAsync()
        {
            if (!_ApplicationValidations())
                throw new Exception("Application validations failed");

            return Mode switch
            {
                EnMode.AddNew => await _CreateApplicationAsync(),
                EnMode.Update => await _UpdateApplicationAsync(),
                _ => throw new Exception("Invalid mode"),
            };
        }

        public async Task<List<Application>> GetApplicationsAsync(int pageNumber = 1, int pageSize = 10)
        {
            var applicationDTOs = await _dal.GetAllApplicationsAsync(pageNumber, pageSize);
            var applicationsList = new List<Application>();

            foreach (var applicationDTO in applicationDTOs)
            {
                applicationsList.Add(new Application(applicationDTO, _dal, _logger, EnMode.Update));
            }

            return applicationsList;
        }

        public async Task<Application> GetApplicationByIdAsync(int applicationId)
        {
            if (applicationId <= 0)
                throw new Exception("Application ID must be greater than 0.");

            var applicationDTO = await _dal.GetApplicationByIdAsync(applicationId);
            return applicationDTO != null ? new Application(applicationDTO, _dal, _logger, EnMode.Update) : null;
        }

        public async Task<List<Application>> GetApplicationsByApplicationTypeAsync(int applicationType)
        {
            if (applicationType <= 0)
                throw new Exception("Application type must be greater than 0.");

            var applicationDTOs = await _dal.GetApplicationsByApplicationTypeAsync(applicationType);
            var applicationsList = new List<Application>();

            foreach (var applicationDTO in applicationDTOs)
            {
                applicationsList.Add(new Application(applicationDTO, _dal, _logger, EnMode.Update));
            }

            return applicationsList;
        }

        public async Task<List<Application>> GetApplicationsByUserIdAsync(int userId)
        {
            if (userId <= 0)
                throw new Exception("User ID must be greater than 0.");

            var applicationDTOs = await _dal.GetApplicationsByUserIdAsync(userId);
            var applicationsList = new List<Application>();

            foreach (var applicationDTO in applicationDTOs)
            {
                applicationsList.Add(new Application(applicationDTO, _dal, _logger, EnMode.Update));
            }

            return applicationsList;
        }

        public async Task<List<Application>> GetApplicationsByStatusAsync(enStatus status)
        {
            var applicationDTOs = await _dal.GetApplicationsByStatusAsync(status);
            var applicationsList = new List<Application>();

            foreach (var applicationDTO in applicationDTOs)
            {
                applicationsList.Add(new Application(applicationDTO, _dal, _logger, EnMode.Update));
            }

            return applicationsList;
        }

        public async Task<int> CountAllApplicationsAsync()
        {
            return await _dal.CountAllApplicationsAsync();
        }

        public async Task<int> CountApplicationsByStatusAsync(enStatus status)
        {
            return await _dal.CountApplicationsByStatusAsync(status);
        }

        public async Task<int> CountApplicationsByTypeAsync(int applicationType)
        {
            if (applicationType <= 0)
                throw new Exception("Application type must be greater than 0.");

            return await _dal.CountApplicationsByTypeAsync(applicationType);
        }

        public async Task<bool> UpdateStatusAsync(int applicationId, enStatus status)
        {
            if (applicationId <= 0)
                throw new Exception("Application ID must be greater than 0.");

            return await _dal.UpdateStatusAsync(applicationId, status);
        }

        public async Task<bool> DeleteApplicationAsync(int applicationId)
        {
            if (applicationId <= 0)
                throw new Exception("Application ID must be greater than 0.");

            return await _dal.DeleteApplicationAsync(applicationId);
        }

    }
}
