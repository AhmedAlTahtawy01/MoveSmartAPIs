using DataAccessLayer.Util;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class JobOrderDTO
    {
        [Range(0, int.MaxValue, ErrorMessage = "OrderId must be a non-negative integer.")]
        public int OrderId { get; set; }

        [Required(ErrorMessage = "Application is required.")]
        public ApplicationDTO Application { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "VehicleId must be greater than 0.")]
        public short VehicleId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "DriverId must be greater than 0.")]
        public int DriverId { get; set; }

        [Required(ErrorMessage = "StartDate is required.")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "StartDate is required.")]
        public DateTime EndDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        [Required(ErrorMessage = "Destination is required.")]
        [StringLength(100, ErrorMessage = "Destination cannot exceed 100 characters.")]
        public string Destination { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "OdometerBefore must be non-negative.")]
        public int OdometerBefore { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "OdometerAfter must be non-negative if provided.")]
        public int? OdometerAfter { get; set; }

        public JobOrderDTO(int orderId, ApplicationDTO application, short vehicleId, int driverId, DateTime startDate, DateTime endDate, TimeSpan startTime, TimeSpan endTime, string destination,
            int odometerBefore, int? odometerAfter)
        {
            OrderId = orderId;
            Application = application;
            VehicleId = vehicleId;
            DriverId = driverId;
            StartDate = startDate;
            EndDate = endDate;
            StartTime = startTime;
            EndTime = endTime;
            Destination = destination;
            OdometerBefore = odometerBefore;
            OdometerAfter = odometerAfter;
        }


    }

    public class JobOrderRepo
    {
        private readonly ConnectionSettings _connectionSettings;
        private readonly ApplicationRepo _repo;
        private readonly ILogger<JobOrderRepo> _logger;

        public JobOrderRepo(ConnectionSettings connectionSettings, ApplicationRepo repo, ILogger<JobOrderRepo> logger)
        {
            _connectionSettings = connectionSettings ?? throw new ArgumentNullException(nameof(connectionSettings));
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private JobOrderDTO MapJobOrder(DbDataReader reader)
        {
            ApplicationDTO application;

            try
            {
                application = _repo.MapApplication(reader);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error mapping application data.");
                throw;
            }

            return new JobOrderDTO
            (
                reader.GetInt32(reader.GetOrdinal("OrderID")),
                application,
                reader.GetInt16(reader.GetOrdinal("VehicleID")),
                reader.GetInt32(reader.GetOrdinal("DriverID")),
                reader.GetDateTime(reader.GetOrdinal("OrderStartDate")),
                reader.GetDateTime(reader.GetOrdinal("OrderEndDate")),
                TimeSpan.TryParse(reader["OrderStartTime"]?.ToString(), out TimeSpan startTime) ? startTime : TimeSpan.Zero,
                TimeSpan.TryParse(reader["OrderEndTime"]?.ToString(), out TimeSpan endTime) ? endTime : TimeSpan.Zero,
                reader.GetString(reader.GetOrdinal("Destination")),
                reader.GetInt32(reader.GetOrdinal("KilometersCounterBeforeOrder")),
                reader.IsDBNull(reader.GetOrdinal("KilometersCounterAfterOrder")) ? null : reader.GetInt32(reader.GetOrdinal("KilometersCounterAfterOrder"))
            );
        }

        public async Task<List<JobOrderDTO>> GetAllJobOrdersAsync(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize < 1)
                throw new ArgumentException("Page number and page size must be greater than 0.");

            const string query = @"
                SELECT j.OrderID, j.VehicleID, j.DriverID, j.OrderStartDate, j.OrderEndDate, j.OrderStartTime, j.OrderEndTime, j.Destination, j.KilometersCounterBeforeOrder, j.KilometersCounterAfterOrder,
                       a.ApplicationID, a.CreationDate, a.Status, a.ApplicationType, a.ApplicationDescription, a.CreatedByUserID
                FROM joborders j
                JOIN applications a ON j.ApplicationID = a.ApplicationID
                LIMIT @Offset, @PageSize";
            int offset = (pageNumber - 1) * pageSize;

            return await _connectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                var jobOrders = new List<JobOrderDTO>();
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync()) jobOrders.Add(MapJobOrder(reader));
                return jobOrders;

            }, new MySqlParameter("@Offset", offset), new MySqlParameter("@PageSize", pageSize));
        }

        public async Task<JobOrderDTO?> GetJobOrderByIdAsync(int orderId)
        {
            const string query = @"
                SELECT j.OrderID, j.VehicleID, j.DriverID, j.OrderStartDate, j.OrderEndDate, j.OrderStartTime, j.OrderEndTime, j.Destination, j.KilometersCounterBeforeOrder, j.KilometersCounterAfterOrder,
                       a.ApplicationID, a.CreationDate, a.Status, a.ApplicationType, a.ApplicationDescription, a.CreatedByUserID
                FROM joborders j
                JOIN applications a ON j.ApplicationID = a.ApplicationID
                WHERE j.OrderID = @orderId";

            return await _connectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                using var reader = await cmd.ExecuteReaderAsync();
                return await reader.ReadAsync() ? MapJobOrder(reader) : null;
            }, new MySqlParameter("@orderId", orderId));
        }
    
        private async Task<List<JobOrderDTO>> GetJobOrdersAsync(string filter, params MySqlParameter[] parameters)
        {
            string query = @"
                SELECT j.OrderID, j.VehicleID, j.DriverID, j.OrderStartDate, j.OrderEndDate, j.OrderStartTime, j.OrderEndTime, j.Destination, j.KilometersCounterBeforeOrder, j.KilometersCounterAfterOrder,
                       a.ApplicationID, a.CreationDate, a.Status, a.ApplicationType, a.ApplicationDescription, a.CreatedByUserID
                FROM joborders j
                JOIN applications a ON j.ApplicationID = a.ApplicationID";

            if (!string.IsNullOrEmpty(filter))
            {
                query += " WHERE " + filter;
            }

            return await _connectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                var jobOrders = new List<JobOrderDTO>();
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync()) jobOrders.Add(MapJobOrder(reader));
                return jobOrders;

            }, parameters);
        }

        public async Task<List<JobOrderDTO>> GetJobOrdersByApplicationIdAsync(int applicationId)
        {
            return await GetJobOrdersAsync("j.ApplicationID = @applicationId", new MySqlParameter("@applicationId", applicationId));
        }

        public async Task<List<JobOrderDTO>> GetJobOrdersByVehicleIdAsync(int vehicleId)
        {
            return await GetJobOrdersAsync("j.VehicleID = @vehicleId", new MySqlParameter("@vehicleId", vehicleId));
        }

        public async Task<List<JobOrderDTO>> GetJobOrdersByDriverIdAsync(int driverId)
        {
            return await GetJobOrdersAsync("j.DriverID = @driverId", new MySqlParameter("@driverId", driverId));
        }

        public async Task<List<JobOrderDTO>> GetJobOrdersByStartDateAsync(DateTime startDate)
        {
            return await GetJobOrdersAsync("j.OrderStartDate = @startDate", new MySqlParameter("@startDate", startDate));
        }

        public async Task<List<JobOrderDTO>> GetJobOrdersByDestinationAsync(string destination)
        {
            return await GetJobOrdersAsync("j.Destination LIKE @destination", new MySqlParameter("@destination", "%" + destination + "%"));
        }

        public async Task<List<JobOrderDTO>> GetJobOrdersByStatusAsync(enStatus status)
        {
            return await GetJobOrdersAsync("a.Status = @status", new MySqlParameter("@status", status.ToString()));
        }

        public async Task<List<JobOrderDTO>> GetJobOrdersByDateRangeAsync(DateTime date1, DateTime date2)
        {
            return await GetJobOrdersAsync("j.OrderStartDate BETWEEN @date1 AND @date2",
                new MySqlParameter("@date1", date1),
                new MySqlParameter("@date2", date2));
        }

        public async Task<bool> ExistsAsync(int orderId)
        {
            const string query = "SELECT COUNT(*) FROM joborders WHERE OrderID = @orderId";
            return await _connectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                return Convert.ToInt32(await cmd.ExecuteScalarAsync()) > 0;
            }, new MySqlParameter("@orderId", orderId));
        }

        public async Task<int> CreateJobOrderAsync(JobOrderDTO jobOrder)
        {
            const string query = @"
                INSERT INTO joborders (ApplicationID, VehicleID, DriverID, OrderStartDate, OrderEndDate, OrderStartTime, OrderEndTime, Destination, KilometersCounterBeforeOrder)
                VALUES (@applicationId, @vehicleId, @driverId, @orderStartDate, @orderEndDate, @orderStartTime, @orderEndTime, @destination, @odometerBefore);
                SELECT LAST_INSERT_ID();";

            return await _connectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                return Convert.ToInt32(await cmd.ExecuteScalarAsync());
            }, new MySqlParameter("@applicationId", jobOrder.Application.ApplicationId),
               new MySqlParameter("@vehicleId", jobOrder.VehicleId),
               new MySqlParameter("@driverId", jobOrder.DriverId),
               new MySqlParameter("@orderStartDate", jobOrder.StartDate),
               new MySqlParameter("@orderEndDate", jobOrder.EndDate),
               new MySqlParameter("@orderStartTime", jobOrder.StartTime),
               new MySqlParameter("@orderEndTime", jobOrder.EndTime),
               new MySqlParameter("@destination", jobOrder.Destination),
               new MySqlParameter("@odometerBefore", jobOrder.OdometerBefore));
        }

        public async Task<bool> UpdateJobOrderAsync(JobOrderDTO jobOrder)
        {
            const string query = @"
                UPDATE joborders
                SET 
                    VehicleID = @vehicleId,
                    DriverID = @driverId,
                    OrderStartDate = @startDate,
                    OrderEndDate = @endDate,
                    OrderStartTime = @orderStartTime,
                    OrderEndTime = @orderEndTime,
                    Destination = @destination,
                    KilometersCounterBeforeOrder = @odometerBefore,
                    KilometersCounterAfterOrder = @odometerAfter
                WHERE
                    OrderID = @orderId";

            return await _connectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                return await cmd.ExecuteNonQueryAsync() > 0;
            }, new MySqlParameter("@orderId", jobOrder.OrderId),
               new MySqlParameter("@vehicleId", jobOrder.VehicleId),
               new MySqlParameter("@driverId", jobOrder.DriverId),
               new MySqlParameter("@startDate", jobOrder.StartDate),
               new MySqlParameter("@endDate", jobOrder.EndDate),
               new MySqlParameter("@orderStartTime", jobOrder.StartTime),
               new MySqlParameter("@orderEndTime", jobOrder.EndTime),
               new MySqlParameter("@destination", jobOrder.Destination),
               new MySqlParameter("@odometerBefore", jobOrder.OdometerBefore),
               new MySqlParameter("@odometerAfter", jobOrder.OdometerAfter.HasValue ? jobOrder.OdometerAfter.Value : (object)DBNull.Value));
        }

        public async Task<bool> DeleteJobOrderAsync(int orderId)
        {
            const string query = "DELETE FROM joborders WHERE OrderID = @orderId";

            return await _connectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                return await cmd.ExecuteNonQueryAsync() > 0;
            }, new MySqlParameter("@orderId", orderId));
        }

    }
}
