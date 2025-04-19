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
    public class MissionsVehicleDTO
    {
        public int MissionVehicleId { get; set; }
        public int MissionId { get; set; }
        public int VehicleId { get; set; }

        public MissionsVehicleDTO(int missionVehicleId, int missiondId, int vehicleId)
        {
            MissionVehicleId = missionVehicleId;
            MissionId = missiondId;
            VehicleId = vehicleId;
        }
    }

    public class MissionsVehicleRepo
    {
        private readonly ConnectionSettings _connectionSettings;
        private readonly ILogger<MissionsVehicleRepo> _logger;

        public MissionsVehicleRepo(ConnectionSettings connectionSettings, ILogger<MissionsVehicleRepo> logger)
        {
            _connectionSettings = connectionSettings ?? throw new ArgumentNullException(nameof(connectionSettings), "Connection settings cannot be null.");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger cannot be null.");
        }

        private MissionsVehicleDTO MapMissionsVehicle(DbDataReader reader)
        {
            return new MissionsVehicleDTO
            (
                reader.GetInt32("MissionVehicleID"),
                reader.GetInt32("MissionID"),
                reader.GetInt32("VehicleID")
            );
        }

        public async Task<List<MissionsVehicleDTO>> GetAllMissionVehiclesAsync(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize < 1)
                throw new ArgumentException("Page number and page size must be greater than 0.");

            const string query = @"
                SELECT MissionVehicleID, MissionID, VehicleID
                FROM missionsvehicles
                LIMIT @Offset, @PageSize";
            int offset = (pageNumber - 1) * pageSize;

            return await _connectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                var missionVehiclesList = new List<MissionsVehicleDTO>();
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync()) missionVehiclesList.Add(MapMissionsVehicle(reader));
                return missionVehiclesList;
            }, new MySqlParameter("@Offset", offset), new MySqlParameter("@PageSize", pageSize));
        }

        public async Task<MissionsVehicleDTO?> GetMissionVehicleByIdAsync(int missionVehicleId)
        {
            const string query = @"
                SELECT MissionVehicleID, MissionID, VehicleID
                FROM missionsjoborders
                WHERE MissionVehicleID = @missionVehicleId";

            return await _connectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                using var reader = await cmd.ExecuteReaderAsync();
                return await reader.ReadAsync() ? MapMissionsVehicle(reader) : null;
            }, new MySqlParameter("@missionVehicleId", missionVehicleId));
        }

        private async Task<List<MissionsVehicleDTO>> GetMissionsVehiclesAsync(string filter, params MySqlParameter[] parameters)
        {
            string query = @"
                SELECT MissionVehicleID, MissionID, VehicleID
                FROM missionsjoborders
                WHERE MissionID = @missionId";

            if (!string.IsNullOrEmpty(filter))
                query += " WHERE " + filter;

            return await _connectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                var missionsVehiclesList = new List<MissionsVehicleDTO>();
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync()) missionsVehiclesList.Add(MapMissionsVehicle(reader));
                return missionsVehiclesList;
            }, parameters);
        }

        public async Task<List<MissionsVehicleDTO>> GetMissionVehiclesByMissionIdAsync(int missionId)
        {
            return await GetMissionsVehiclesAsync("MissionID = @missionId", new MySqlParameter("@missionId", missionId));
        }

        public async Task<List<MissionsVehicleDTO>> GetMissionVehiclesByVehicleIdAsync(int vehicleId)
        {
            return await GetMissionsVehiclesAsync("VehicleID = @vehicleId", new MySqlParameter("@vehicleId", vehicleId));
        }

        public async Task<int> CreateMissionVehicleAsync(MissionsVehicleDTO missionsVehicle)
        {
            const string query = @"
                    INSERT INTO missionsvehicles (MissionID, VehicleID)
                    VALUES (@missionId, @vehicleId)
                    SELECT LAST_INSERT_ID();";

            return await _connectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                return Convert.ToInt32(await cmd.ExecuteScalarAsync());
            }, new MySqlParameter("@missionId", missionId),
                new MySqlParameter("@vehicleId", vehicleId)
            );
        }

        public async Task<bool> UpdateMissionsVehicleAsync(MissionsVehicleDTO missionsVehicle)
        {
            var query = @"
                UPDATE missionsvehicles
                SET 
                    MissionID = @missionId,
                    VehicleID = @vehicleId
                WHERE
                    MissionVehicleID = @missionVehicleId";

            return await _connectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                return await cmd.ExecuteNonQueryAsync() > 0;
            }, new MySqlParameter("@missionVehicleId", missionsVehicle.MissionVehicleId),
                new MySqlParameter("@missionId", missionsVehicle.MissionId),
                new MySqlParameter("@vehicleId", missionsVehicle.VehicleId)
            );
        }

        public async Task<bool> DeleteMissionsVehicleAsync(int missionsVehicleId)
        {
            const string query = "DELETE FROM missionsvehicles WHERE MissionVehicleID = @missionsVehicleId";

            return await _connectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                return await cmd.ExecuteNonQueryAsync() > 0;
            }, new MySqlParameter("@missionsVehicleId", missionsVehicleId));
        }
    }
}
