using DataAccessLayer.Util;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{

    public enum enStatus
    {
        Confirmed,
        Rejected,
        Pending,
        Canceled
    }

    public class ApplicationDTO
    {

        public int ApplicationId { get; set; }
        public DateTime CreationDate { get; set; }
        public enStatus Status { get; set; }
        public int ApplicationType { get; set; }
        public string ApplicationDescription { get; set; }
        public int UserId { get; set; }

        public ApplicationDTO(int applicationId, DateTime creationDate, enStatus status, int applicationType, string applicationDescription, int createdByUser)
        {
            this.ApplicationId = applicationId;
            this.CreationDate = creationDate;
            this.Status = status;
            this.ApplicationType = applicationType;
            this.ApplicationDescription = applicationDescription;
            this.UserId = createdByUser;
        }
    }

    public class ApplicationRepo
    {

        private readonly ConnectionSettings _connectionSettings;
        private readonly ILogger<ApplicationRepo> _logger;

        public ApplicationRepo(ConnectionSettings connectionSettings, ILogger<ApplicationRepo> logger)
        {
            _connectionSettings = connectionSettings ?? throw new ArgumentNullException(nameof(connectionSettings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private ApplicationDTO MapApplication(DbDataReader reader)
        {
            if (!Enum.TryParse(reader.GetString(reader.GetOrdinal("Status")), out enStatus status))
            {
                _logger.LogWarning($"Invalid Status value: {reader["Status"]}. Defaulting to Pending.");
                status = enStatus.Pending;
            }

            return new ApplicationDTO
            (
                reader.GetInt32("ApplicationID"),
                reader.GetDateTime("CreationDate"),
                status,
                reader.GetInt32("ApplicationType"),
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
                using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);

                while (await reader.ReadAsync()) applicationsList.Add(MapApplication(reader));
                return applicationsList;

            }, new MySqlParameter("@Offset", offset), new MySqlParameter("@PageSize", pageSize));
        }
        
        public async Task<ApplicationDTO> GetApplicationByIdAsync(int applicationId)
        {
            const string query = @"
                        SELECT ApplicationID, CreationDate, Status, ApplicationType, ApplicationDescription, CreatedByUserID
                        FROM applications
                        WHERE ApplicationID = @applicationId";

            return await _connectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
                return await reader.ReadAsync() ? MapApplication(reader) : null;
            }, new MySqlParameter("@applicationId", applicationId));
        }

        public async Task<List<ApplicationDTO>> GetApplicationsByApplicationTypeAsync(int applicationType)
        {
            const string query = @"
                SELECT ApplicationID, CreationDate, Status, ApplicationType, ApplicationDescription, CreatedByUserID
                FROM applications
                WHERE ApplicationType = @applicationType";

            return await _connectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                var applicationsList = new List<ApplicationDTO>();
                using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
                while (await reader.ReadAsync()) applicationsList.Add(MapApplication(reader));
                return applicationsList;
            }, new MySqlParameter("@applicationType", applicationType));
        }

        public async Task<List<ApplicationDTO>> GetApplicationsByUserIdAsync(int createdByUser)
        {
            const string query = @"
                SELECT ApplicationID, CreationDate, Status, ApplicationType, ApplicationDescription, CreatedByUserID
                FROM applications
                WHERE CreatedByUserID = @createdByUser";

            return await _connectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                var applicationsList = new List<ApplicationDTO>();
                using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
                while (await reader.ReadAsync()) applicationsList.Add(MapApplication(reader));
                return applicationsList;
            }, new MySqlParameter("@createdByUser", createdByUser));
        }

        public async Task<List<ApplicationDTO>> GetApplicationsByStatusAsync(enStatus status)
        {
            const string query = @"
                SELECT ApplicationID, CreationDate, Status, ApplicationType, ApplicationDescription, CreatedByUserID
                FROM applications
                WHERE Status = @status";

            return await _connectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                var applicationsList = new List<ApplicationDTO>();
                using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
                while (await reader.ReadAsync()) applicationsList.Add(MapApplication(reader));
                return applicationsList;
            }, new MySqlParameter("@status", status.ToString()));
        }

        public async Task<int> CountAllApplicationsAsync()
        {
            const string query = "SELECT COUNT(*) FROM applications";

            return await _connectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                return Convert.ToInt32(await cmd.ExecuteScalarAsync().ConfigureAwait(false));
            });
        }

        public async Task<int> CountApplicationsByStatusAsync(enStatus status)
        {
            const string query = "SELECT COUNT(*) FROM applications WHERE Status = @status";

            return await _connectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                return Convert.ToInt32(await cmd.ExecuteScalarAsync().ConfigureAwait(false));
            }, new MySqlParameter("@status", status.ToString()));
        }

        public async Task<int> CountApplicationsByTypeAsync(int applicationType)
        {
            const string query = "SELECT COUNT(*) FROM applications WHERE ApplicationType = @applicationType";

            return await _connectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                return Convert.ToInt32(await cmd.ExecuteScalarAsync().ConfigureAwait(false));
            }, new MySqlParameter("@applicationType", applicationType));
        }

        public async Task<bool> UpdateStatusAsync(int applicationId, enStatus status)
        {
            const string query = @"
                UPDATE applications
                SET Status = @status
                WHERE ApplicationID = @applicationId";

            return await _connectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                return await cmd.ExecuteNonQueryAsync().ConfigureAwait(false) > 0;
            }, new MySqlParameter("@applicationId", applicationId), new MySqlParameter("@status", status.ToString()));
        }

        public async Task<int> CreateApplicationAsync(ApplicationDTO application)
        {
            const string query = @"
                INSERT INTO applications (CreationDate, Status, ApplicationType, ApplicationDescription, CreatedByUserID)
                VALUES (@creationDate, @status, @applicationType, @applicationDescription, @createdByUser);
                SELECT LAST_INSERT_ID()";

            return await _connectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                return Convert.ToInt32(await cmd.ExecuteScalarAsync().ConfigureAwait(false));
            }, new MySqlParameter("@creationDate", application.CreationDate),
               new MySqlParameter("@status", enStatus.Pending.ToString()),
               new MySqlParameter("@applicationType", application.ApplicationType),
               new MySqlParameter("@applicationDescription", application.ApplicationDescription),
               new MySqlParameter("@createdByUser", application.UserId));
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
                return await cmd.ExecuteNonQueryAsync().ConfigureAwait(false) > 0;
            }, new MySqlParameter("@status", application.Status.ToString()),
               new MySqlParameter("@applicationType", application.ApplicationType),
               new MySqlParameter("@applicationDescription", application.ApplicationDescription),
               new MySqlParameter("@createdByUser", application.UserId),
               new MySqlParameter("@applicationId", application.ApplicationId));
        }
        
        public async Task<bool> DeleteApplicationAsync(int applicationId)
        {
            const string query = "DELETE FROM applications WHERE ApplicationID = @applicationId";

            return await _connectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                return await cmd.ExecuteNonQueryAsync().ConfigureAwait(false) > 0;
            }, new MySqlParameter("@applicationId", applicationId));
        }

    }
}