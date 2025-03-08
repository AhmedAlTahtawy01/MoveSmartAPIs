using DataAccessLayer.Util;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
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
        public int OrderId { get; set; }
        public int ApplicationId { get; set; }
        public int VehicleId { get; set; }
        public int DriverId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Destination { get; set; }
        public int OdometerBefore { get; set; }
        public int OdometerAfter { get; set; }

        public JobOrderDTO(int orderId, int applicationId, int vehicleId, int driverId, DateTime startDate, DateTime endDate, TimeSpan startTime, TimeSpan endTime, string destination,
            int odometerBefore, int odometerAfter)
        {
            OrderId = orderId;
            ApplicationId = applicationId;
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
        private readonly ILogger<JobOrderRepo> _logger;

        public JobOrderRepo(ILogger<JobOrderRepo> logger)
        {
            _logger = logger;
        }

        private JobOrderDTO MapJobOrder(DbDataReader reader)
        {
            if (!Enum.TryParse(reader.GetString(reader.GetOrdinal("Status")).Trim(), true, out enStatus status))
            {
                _logger.LogWarning($"Invalid status value: {reader["Status"]}. Defaulting to Pending.");
                status = enStatus.Pending;
            }

            return new JobOrderDTO
            (
                reader.GetInt32(reader.GetOrdinal("OrderID")),
                reader.GetInt32(reader.GetOrdinal("ApplicationID")),
                reader.GetInt32(reader.GetOrdinal("VehicleID")),
                reader.GetInt32(reader.GetOrdinal("DriverID")),
                reader.GetDateTime(reader.GetOrdinal("OrderStartDate")),
                reader.GetDateTime(reader.GetOrdinal("OrderEndDate")),
                TimeSpan.TryParse(reader["OrderStartTime"]?.ToString(), out TimeSpan startTime) ? startTime : TimeSpan.Zero,
                TimeSpan.TryParse(reader["OrderEndTime"]?.ToString(), out TimeSpan endTime) ? endTime : TimeSpan.Zero,
                reader.GetString(reader.GetOrdinal("Destination")),
                reader.GetInt32(reader.GetOrdinal("KilometersCounterBeforeOrder")),
                reader.IsDBNull(reader.GetOrdinal("KilometersCounterAfterOrder")) ? 0 : reader.GetInt32(reader.GetOrdinal("KilometersCounterAfterOrder"))
            );
        }

        public async Task<List<JobOrderDTO>> GetAllJobOrdersAsync(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize < 1)
                throw new ArgumentException("Page number and page size must be greater than 0.");

            const string query = @"
                SELECT j.OrderID, j.VehicleID, j.DriverID, j.OrderStartDate, j.OrderEndDate, j.OrderStartTime, j.OrderEndTime, j.Destination, j.KilometersCounterBeforeOrder, j.KilometersCounterAfterOrder,
                       a.ApplicationID, a.CreationDate, a.Status, a.ApplicationType, a.ApplicationDescription, a.CreatedByUser
                FROM joborders j
                JOIN applications a ON j.ApplicationID = a.ApplicationID
                LIMIT @Offset, @PageSize";
            int offset = (pageNumber - 1) * pageSize;

            return await ConnectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                var jobOrders = new List<JobOrderDTO>();
                using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
                while (await reader.ReadAsync()) jobOrders.Add(MapJobOrder(reader));
                return jobOrders;
            }, new MySqlParameter("@Offset", offset), new MySqlParameter("@PageSize", pageSize));
        }

        public async Task<JobOrderDTO> GetJobOrderByIdAsync(int orderId)
        {
            const string query = @"
                SELECT j.OrderID, j.VehicleID, j.DriverID, j.OrderStartDate, j.OrderEndDate, j.OrderStartTime, j.OrderEndTime, j.Destination, j.KilometersCounterBeforeOrder, j.KilometersCounterAfterOrder,
                       a.ApplicationID, a.CreationDate, a.Status, a.ApplicationType, a.ApplicationDescription, a.CreatedByUser
                FROM joborders j
                JOIN applications a ON j.ApplicationID = a.ApplicationID
                WHERE j.OrderID = @orderId";

            return await ConnectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
                return await reader.ReadAsync() ? MapJobOrder(reader) : null;
            }, new MySqlParameter("@orderId", orderId));
        }
    
        public async Task<List<JobOrderDTO>> GetJobOrdersByApplicationIdAsync(int applicationId)
        {
            const string query = @"
                SELECT j.OrderID, j.VehicleID, j.DriverID, j.OrderStartDate, j.OrderEndDate, j.OrderStartTime, j.OrderEndTime, j.Destination, j.KilometersCounterBeforeOrder, j.KilometersCounterAfterOrder,
                       a.ApplicationID, a.CreationDate, a.Status, a.ApplicationType, a.ApplicationDescription, a.CreatedByUser
                FROM joborders j
                JOIN applications a ON j.ApplicationID = a.ApplicationID
                WHERE j.ApplicationID = @applicationId";

            return await ConnectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                var jobOrders = new List<JobOrderDTO>();
                using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
                while (await reader.ReadAsync()) jobOrders.Add(MapJobOrder(reader));
                return jobOrders;
            }, new MySqlParameter("@applicationId", applicationId));
        }

        public async Task<List<JobOrderDTO>> GetJobOrdersByVehicleIdAsync(int vehicleId)
        {
            const string query = @"
                SELECT j.OrderID, j.VehicleID, j.DriverID, j.OrderStartDate, j.OrderEndDate, j.OrderStartTime, j.OrderEndTime, j.Destination, j.KilometersCounterBeforeOrder, j.KilometersCounterAfterOrder,
                       a.ApplicationID, a.CreationDate, a.Status, a.ApplicationType, a.ApplicationDescription, a.CreatedByUser
                FROM joborders j
                JOIN applications a ON j.ApplicationID = a.ApplicationID
                WHERE j.VehicleID = @vehicleId";

            return await ConnectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                var jobOrders = new List<JobOrderDTO>();
                using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
                while (await reader.ReadAsync()) jobOrders.Add(MapJobOrder(reader));
                return jobOrders;
            }, new MySqlParameter("@vehicleId", vehicleId));
        }

        public async Task<List<JobOrderDTO>> GetJobOrdersByDriverIdAsync(int driverId)
        {
            const string query = @"
                SELECT j.OrderID, j.VehicleID, j.DriverID, j.OrderStartDate, j.OrderEndDate, j.OrderStartTime, j.OrderEndTime, j.Destination, j.KilometersCounterBeforeOrder, j.KilometersCounterAfterOrder,
                       a.ApplicationID, a.CreationDate, a.Status, a.ApplicationType, a.ApplicationDescription, a.CreatedByUser
                FROM joborders j
                JOIN applications a ON j.ApplicationID = a.ApplicationID
                WHERE j.DriverID = @driverId";

            return await ConnectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                var jobOrders = new List<JobOrderDTO>();
                using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
                while (await reader.ReadAsync()) jobOrders.Add(MapJobOrder(reader));
                return jobOrders;
            }, new MySqlParameter("@driverId", driverId));
        }

        public async Task<List<JobOrderDTO>> GetJobOrdersByStartDateAsync(DateTime startDate)
        {
            const string query = @"
                SELECT j.OrderID, j.VehicleID, j.DriverID, j.OrderStartDate, j.OrderEndDate, j.OrderStartTime, j.OrderEndTime, j.Destination, j.KilometersCounterBeforeOrder, j.KilometersCounterAfterOrder,
                       a.ApplicationID, a.CreationDate, a.Status, a.ApplicationType, a.ApplicationDescription, a.CreatedByUser
                FROM joborders j
                JOIN applications a ON j.ApplicationID = a.ApplicationID
                WHERE j.OrderStartDate = @startDate";

            return await ConnectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                var jobOrders = new List<JobOrderDTO>();
                using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
                while (await reader.ReadAsync()) jobOrders.Add(MapJobOrder(reader));
                return jobOrders;
            }, new MySqlParameter("@startDate", startDate));
        }

        public async Task<List<JobOrderDTO>> GetJobOrdersByDestinationIdAsync(string destination)
        {
            const string query = @"
                SELECT j.OrderID, j.VehicleID, j.DriverID, j.OrderStartDate, j.OrderEndDate, j.OrderStartTime, j.OrderEndTime, j.Destination, j.KilometersCounterBeforeOrder, j.KilometersCounterAfterOrder,
                       a.ApplicationID, a.CreationDate, a.Status, a.ApplicationType, a.ApplicationDescription, a.CreatedByUser
                FROM joborders j
                JOIN applications a ON j.ApplicationID = a.ApplicationID
                WHERE j.Destination = @destination";

            return await ConnectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                var jobOrders = new List<JobOrderDTO>();
                using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
                while (await reader.ReadAsync()) jobOrders.Add(MapJobOrder(reader));
                return jobOrders;
            }, new MySqlParameter("@destination", destination));
        }

        // public async Task<list<JobOrder>> GetJobOrdersByStatus(enStatus status);

        // public async Task<list<JobOrder>> GetJobOrdersByDateRange(DateTime Date1, DateTime Date2);
        public async Task<int> CreateJobOrderAsync(JobOrderDTO jobOrder)
        {
            const string query = @"
                INSERT INTO joborders (ApplicationID, VehicleID, DriverID, OrderStartDate, OrderEndDate, OrderStartTime, OrderEndTime, Destination, KilometersCounterBeforeOrder)
                VALUES (@applicationId, @vehicleId, @driverId, @orderStartDate, @orderEndDate, @orderStartTime, @orderEndTime, @destination, @odometerBefore);
                SELECT LAST_INSERT_ID();";

            return await ConnectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                return Convert.ToInt32(await cmd.ExecuteScalarAsync().ConfigureAwait(false));
            }, new MySqlParameter("@applicationId", jobOrder.ApplicationId),
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
                    ApplicationID = @applicationId,
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

            return await ConnectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                return await cmd.ExecuteNonQueryAsync().ConfigureAwait(false) > 0;
            }, new MySqlParameter("@orderId", jobOrder.OrderId),
               new MySqlParameter("@applicationId", jobOrder.ApplicationId),
               new MySqlParameter("@vehicleId", jobOrder.VehicleId),
               new MySqlParameter("@driverId", jobOrder.DriverId),
               new MySqlParameter("@startDate", jobOrder.StartDate),
               new MySqlParameter("@endDate", jobOrder.EndDate),
               new MySqlParameter("@orderStartTime", jobOrder.StartTime),
               new MySqlParameter("@orderEndTime", jobOrder.EndTime),
               new MySqlParameter("@destination", jobOrder.Destination),
               new MySqlParameter("@odometerBefore", jobOrder.OdometerBefore),
               new MySqlParameter("@odometerAfter", jobOrder.OdometerAfter));
        }

        public async Task<bool> DeleteJobOrderAsync(int orderId)
        {
            const string query = "DELETE FROM joborders WHERE OrderID = @orderId";

            return await ConnectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                return await cmd.ExecuteNonQueryAsync().ConfigureAwait(false) > 0;
            }, new MySqlParameter("@orderId", orderId));
        }

    }
}
