using DataAccessLayer.Util;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{

    public enum enStatus
    {
        Confirmed = 1,
        Rejected = 2,
        Pending = 3,
        Cancelled = 4
    }

    public enum enApplicationType
    {
        JobOrder = 1,
        MissionNote = 2,
        SparePartWithdrawApplication = 3,
        SparePartPurchaseOrder = 4,
        ConsumableWithdrawApplication = 5,
        ConsumablePurchaseOrder = 6,
        MaintenanceApplication = 7
    }
    [Table("applications")]
    public class ApplicationDTO
    {
        [Key]
        public int ApplicationId { get; set; }
        public DateTime CreationDate { get; set; }
        public enStatus Status { get; set; }
        public enApplicationType ApplicationType { get; set; }
        public string ApplicationDescription { get; set; }
        public int CreatedByUserID { get; set; }
     
        public ApplicationDTO(int applicationId, DateTime creationDate, enStatus status, enApplicationType applicationType, string applicationDescription, int createdByUserID)
        {
            ApplicationId = applicationId;
            CreationDate = creationDate;
            Status = status;
            ApplicationType = applicationType;
            ApplicationDescription = applicationDescription;
            CreatedByUserID = createdByUserID;
        }
    }

    public class ApplicationRepo
    {

        private readonly ConnectionSettings _connectionSettings;
        private readonly ILogger<ApplicationRepo> _logger;

        public ApplicationRepo(ConnectionSettings connectionSettings, ILogger<ApplicationRepo> logger)
        {
            _connectionSettings = connectionSettings ?? throw new ArgumentNullException(nameof(connectionSettings), "Connection settings cannot be null.");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger cannot be null.");
        }

        public ApplicationDTO MapApplication(DbDataReader reader)
        {
            if (!Enum.TryParse(reader.GetString(reader.GetOrdinal("Status")), true, out enStatus status))
            {
                _logger.LogError($"Invalid Status value in database: {reader["Status"]}");
                throw new InvalidOperationException($"Unable to parse status: {reader["Status"]}");
            }

            if (!Enum.TryParse(reader.GetString(reader.GetOrdinal("ApplicationType")), true, out enApplicationType applicationType))
            {
                _logger.LogError($"Invalid ApplicationType value in database: {reader["ApplicationType"]}");
                throw new InvalidOperationException($"Unable to parse application type: {reader["ApplicationType"]}");
            }

            return new ApplicationDTO
            (
                reader.GetInt32("ApplicationID"),
                reader.GetDateTime("CreationDate"),
                status,
                applicationType,
                reader.GetString("ApplicationDescription"),
                reader.GetInt32("CreatedByUserID")
            );
        }

        public async Task<List<ApplicationDTO>> GetAllApplicationsAsync(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize < 1)
                throw new ArgumentException("Page number and page size must be greater than 0.");

            const string query = @"
                        SELECT ApplicationID, CreationDate, Status, ApplicationType, ApplicationDescription, CreatedByUserID
                        FROM applications
                        LIMIT @Offset, @PageSize";
            int offset = (pageNumber - 1) * pageSize;

            return await _connectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                var applicationsList = new List<ApplicationDTO>();
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync()) applicationsList.Add(MapApplication(reader));
                return applicationsList;

            }, new MySqlParameter("@Offset", offset), new MySqlParameter("@PageSize", pageSize));
        }

        public async Task<ApplicationDTO?> GetApplicationByIdAsync(int applicationId)
        {
            const string query = @"
                        SELECT ApplicationID, CreationDate, Status, ApplicationType, ApplicationDescription, CreatedByUserID
                        FROM applications
                        WHERE ApplicationID = @applicationId";

            return await _connectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                using var reader = await cmd.ExecuteReaderAsync();
                return await reader.ReadAsync() ? MapApplication(reader) : null;
            }, new MySqlParameter("@applicationId", applicationId));
        }

        // Helper method to get applications based on a filter
        private async Task<List<ApplicationDTO>> GetApplicationsAsync(string filter, params MySqlParameter[] parameters)
        {
            string query = @"
                SELECT ApplicationID, CreationDate, Status, ApplicationType, ApplicationDescription, CreatedByUserID
                FROM applications";

            if (!string.IsNullOrEmpty(filter))
                query += " WHERE " + filter;

            return await _connectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                var applicationsList = new List<ApplicationDTO>();
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync()) applicationsList.Add(MapApplication(reader));
                return applicationsList;
            }, parameters);
        }

        public async Task<List<ApplicationDTO>> GetApplicationsByApplicationTypeAsync(enApplicationType applicationType)
        {
            return await GetApplicationsAsync("ApplicationType = @applicationType", new MySqlParameter("@applicationType", applicationType.ToString()));
        }

        public async Task<List<ApplicationDTO>> GetApplicationsByUserIdAsync(int createdByUser)
        {
            return await GetApplicationsAsync("CreatedByUserID = @createdByUser", new MySqlParameter("@createdByUser", createdByUser));
        }

        public async Task<List<ApplicationDTO>> GetApplicationsByStatusAsync(enStatus status)
        {
            return await GetApplicationsAsync("Status = @status", new MySqlParameter("@status", status.ToString()));
        }

        public async Task<int> CountAllApplicationsAsync()
        {
            const string query = "SELECT COUNT(*) FROM applications";

            return await _connectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                return Convert.ToInt32(await cmd.ExecuteScalarAsync());
            });
        }

        // Helper method to count applications based on a filter
        private async Task<int> CountApplicationAsync(string filter, params MySqlParameter[] parameters)
        {
            string query = "SELECT COUNT(*) FROM applications";
            if (!string.IsNullOrEmpty(filter))
            {
                query += " WHERE " + filter;
            }
            return await _connectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                return Convert.ToInt32(await cmd.ExecuteScalarAsync());
            }, parameters);
        }

        public async Task<int> CountApplicationsByStatusAsync(enStatus status)
        {
            return await CountApplicationAsync("Status = @status", new MySqlParameter("@status", status.ToString()));
        }

        public async Task<int> CountApplicationsByTypeAsync(enApplicationType applicationType)
        {
            return await CountApplicationAsync("ApplicationType = @applicationType", new MySqlParameter("@applicationType", applicationType.ToString()));
        }

        public async Task<bool> ExistsAsync(int id)
        {
            const string query = "SELECT COUNT(1) FROM applications WHERE ApplicationID = @applicationId";

            return await _connectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                return Convert.ToInt32(await cmd.ExecuteScalarAsync()) > 0;
            }, new MySqlParameter("@applicationId", id));
        }

        public async Task<bool> UpdateStatusAsync(int applicationId, enStatus status)
        {
            const string query = @"
                UPDATE applications
                SET Status = @status
                WHERE ApplicationID = @applicationId";

            return await _connectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                return await cmd.ExecuteNonQueryAsync() > 0;
            }, new MySqlParameter("@applicationId", applicationId),
                new MySqlParameter("@status", status.ToString()));
        }

        public async Task<int> CreateApplicationAsync(ApplicationDTO application)
        {
            const string query = @"
                INSERT INTO applications (CreationDate, Status, ApplicationType, ApplicationDescription, CreatedByUserID)
                VALUES (@creationDate, @status, @applicationType, @applicationDescription, @createdByUser);
                SELECT LAST_INSERT_ID()";

            return await _connectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                return Convert.ToInt32(await cmd.ExecuteScalarAsync());
            }, new MySqlParameter("@creationDate", application.CreationDate),
                new MySqlParameter("@status", enStatus.Pending.ToString()),
                new MySqlParameter("@applicationType", application.ApplicationType.ToString()),
                new MySqlParameter("@applicationDescription", application.ApplicationDescription),
                new MySqlParameter("@createdByUser", application.CreatedByUserID)
            );
        }

        public async Task<bool> UpdateApplicationAsync(ApplicationDTO application)
        {
            const string query = @"
                UPDATE applications
                SET Status = @status,
                    ApplicationType = @applicationType,
                    ApplicationDescription = @applicationDescription,
                    CreatedByUserID = @createdByUser
                WHERE ApplicationID = @applicationId";

            return await _connectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                return await cmd.ExecuteNonQueryAsync() > 0;
            }, new MySqlParameter("@applicationId", application.ApplicationId),
                new MySqlParameter("@status", application.Status.ToString()),
                new MySqlParameter("@applicationType", application.ApplicationType.ToString()),
                new MySqlParameter("@applicationDescription", application.ApplicationDescription),
                new MySqlParameter("@createdByUser", application.CreatedByUserID)
            );
        }
        
        public async Task<bool> DeleteApplicationAsync(int applicationId)
        {
            const string query = "DELETE FROM applications WHERE ApplicationID = @applicationId";

            return await _connectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                return await cmd.ExecuteNonQueryAsync() > 0;
            }, new MySqlParameter("@applicationId", applicationId));
        }
    }
}