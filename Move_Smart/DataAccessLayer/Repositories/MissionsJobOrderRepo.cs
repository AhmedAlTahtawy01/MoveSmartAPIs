using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using DataAccessLayer.Util;
using Microsoft.Extensions.Logging;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace DataAccessLayer.Repositories
{
    public class MissionsJobOrderDTO
    {
        public int OrderId { get; set; }
        public int MissionId { get; set; }
        public int JobOrderId { get; set; }

        public MissionsJobOrderDTO(int orderId, int missionId, int jobOrderId)
        {
            OrderId = orderId;
            MissionId = missionId;
            JobOrderId = jobOrderId;
        }
    }

    public class MissionsJobOrderRepo
    {
        private readonly ConnectionSettings _connectionSettings;
        private readonly ILogger<MissionsJobOrderRepo> _logger;

        public MissionsJobOrderRepo(ConnectionSettings connectionSettings, ILogger<MissionsJobOrderRepo> logger)
        {
            _connectionSettings = connectionSettings ?? throw new ArgumentNullException(nameof(connectionSettings), "Connection settings cannot be null.");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger cannot be null.");
        }

        private MissionsJobOrderDTO MapMissionsJobOrder(DbDataReader reader)
        {
            return new MissionsJobOrderDTO
            (
                reader.GetInt32("OrderID"),
                reader.GetInt32("MissionID"),
                reader.GetInt32("JobOrderID")
            );
        }

        public async Task<List<MissionsJobOrderDTO>> GetAllMissionsJobOrdersAsync(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize < 1)
                throw new ArgumentException("Page number and page size must be greater than 0.");

            const string query = @"
                    SELECT OrderID, MissionID, JobOrderID
                    FROM missionsjoborders
                    LIMIT @Offset, @PageSize";
            int offset = (pageNumber - 1) * pageSize;

            return await _connectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                var missionsJobOrdersList = new List<MissionsJobOrderDTO>();
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync()) missionsJobOrdersList.Add(MapMissionsJobOrder(reader));
                return missionsJobOrdersList;
            }, new MySqlParameter("@Offset", offset), new MySqlParameter("@PageSize", pageSize));
        }

        public async Task<MissionsJobOrderDTO?> GetMissionsJobOrderByIdAsync(int orderId)
        {
            const string query = @"
                SELECT OrderID, MissionID, JobOrderID
                FROM missionsjoborders
                WHERE OrderID = @orderId";

            return await _connectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                using var reader = await cmd.ExecuteReaderAsync();
                return await reader.ReadAsync() ? MapMissionsJobOrder(reader) : null;
            }, new MySqlParameter("@orderId", orderId));
        }

        private async Task<List<MissionsJobOrderDTO>> GetMissionsJobOrdersAsync(string filter, params MySqlParameter[] parameters)
        {
            string query = @"
                SELECT OrderID, MissionID, JobOrderID
                FROM missionsjoborders";

            if (!string.IsNullOrEmpty(filter))
                query += " WHERE " + filter;

            return await _connectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                var missionsJobOrdersList = new List<MissionsJobOrderDTO>();
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync()) missionsJobOrdersList.Add(MapMissionsJobOrder(reader));
                return missionsJobOrdersList;
            }, parameters);
        }

        public async Task<List<MissionsJobOrderDTO>> GetMissionsJobOrdersByMissionIdAsync(int missionId)
        {
            return await GetMissionsJobOrdersAsync("MissionID = @missionId", new MySqlParameter("@missionId", missionId));
        }

        public async Task<List<MissionsJobOrderDTO>> GetMissionsJobOrdersByJobOrderIdAsync(int jobOrderId)
        {
            return await GetMissionsJobOrdersAsync("JobOrderID = @jobOrderId", new MySqlParameter("@jobOrderId", jobOrderId));
        }

        public async Task<int> CreateMissionsJobOrderAsync(MissionsJobOrderDTO missionsJobOrder)
        {
            const string query = @"
                INSERT INTO missionsjoborders (MissionID, JobOrderID)
                VALUES (@missionId, @jobOrderId)
                SELECT LAST_INSERT_ID();";

            return await _connectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                return Convert.ToInt32(await cmd.ExecuteScalarAsync());
            }, new MySqlParameter("@missionId", missionsJobOrder.MissionId),
                new MySqlParameter("@jobOrderId", missionsJobOrder.JobOrderId)
            );
        }

        public async Task<bool> UpdateMissionsJobOrderAsync(MissionsJobOrderDTO missionsJobOrder)
        {
            const string query = @"
                UPDATE missionsjoborders
                SET 
                    MissionID = @missionId,
                    JobOrderID = @jobOrderId
                WHERE
                    OrderID = @orderId";

            return await _connectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                return await cmd.ExecuteNonQueryAsync() > 0;
            }, new MySqlParameter("@orderId", missionsJobOrder.OrderId),
                new MySqlParameter("@missionId", missionsJobOrder.MissionId),
                new MySqlParameter("@jobOrderId", missionsJobOrder.JobOrderId)
            );
        }

        public async Task<bool> DeleteMissionsJobOrderAsync(int orderId)
        {
            const string query = "DELETE FROM missionsjoborders WHERE OrderID = @orderId";
            
            return await _connectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                return await cmd.ExecuteNonQueryAsync() > 0;
            }, new MySqlParameter("@orderId", orderId));
        }
    }
}
