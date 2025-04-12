using DataAccessLayer.Util;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace DataAccessLayer.Repositories
{
    public class MissionDTO
    {
        public int MissionId { get; set; }
        public int MissionNoteId { get; set; }
        public int MissionVehiclesId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Destination { get; set; }
        public int UserId { get; set; }

        public MissionDTO(int missionId, int missionNoteId, int missionVechiclesId, DateTime startDate, DateTime endDate, string destination, int createdByUser)
        {
            MissionId = missionId;
            MissionNoteId = missionNoteId;
            MissionVehiclesId = missionVechiclesId;
            StartDate = startDate;
            EndDate = endDate;
            Destination = destination;
            UserId = createdByUser;
        }
    }

    public class MissionRepo
    {
        private readonly ConnectionSettings _connectionSettings;
        private readonly ILogger<MissionRepo> _logger;

        public MissionRepo(ConnectionSettings connectionSettings, ILogger<MissionRepo> logger)
        {
            _connectionSettings = connectionSettings ?? throw new ArgumentNullException(nameof(connectionSettings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private MissionDTO MapMission(DbDataReader reader)
        {
            return new MissionDTO
            (
                reader.GetInt32("MissionID"),
                reader.GetInt32("MissionNoteID"),
                reader.GetInt32("MissionVehiclesID"),
                reader.GetDateTime("MissionStartDate"),
                reader.GetDateTime("MissionEndDate"),
                reader.GetString("DIstination"),
                reader.GetInt32("CreatedByUser")
            );
        }

        public async Task<List<MissionDTO>> GetAllMissionsAsync(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize < 1)
                throw new ArgumentException("Page number and page size must be greater than 0.");

            const string query = @"
                    SELECT MissionID, MissionNoteID, MissionVehiclesID, MissionStartDate, MissionEndDate, DIstination, CreatedByUser
                    FROM missions
                    LIMIT @Offset, @PageSize";
            int offset = (pageNumber - 1) * pageSize;

            return await _connectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                var missionsList = new List<MissionDTO>();
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync()) missionsList.Add(MapMission(reader));
                return missionsList;

            }, new MySqlParameter("@Offset", offset), new MySqlParameter("@PageSize", pageSize));
        }

        public async Task<MissionDTO?> GetMissionByIdAsync(int missionId)
        {
            const string query = @"
                    SELECT MissionID, MissionNoteID, MissionVehiclesID, MissionStartDate, MissionEndDate, DIstination, CreatedByUser
                    FROM missions
                    WHERE MissionID = @missionId";

            return await _connectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                using var reader = await cmd.ExecuteReaderAsync();
                return await reader.ReadAsync() ? MapMission(reader) : null;
            }, new MySqlParameter("@missionId", missionId));
        }

        private async Task<List<MissionDTO>> GetMissionsAsync(string filter, params MySqlParameter[] parameters)
        {
            string query = @"
                    SELECT MissionID, MissionNoteID, MissionVehiclesID, MissionStartDate, MissionEndDate, DIstination, CreatedByUser
                    FROM missions";

            if (!string.IsNullOrEmpty(filter))
                query += " WHERE " + filter;

            return await _connectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                var missionsList = new List<MissionDTO>();
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync()) missionsList.Add(MapMission(reader));
                return missionsList;
            }, parameters);
        }

        public async Task<List<MissionDTO>> GetMissionsByNoteIdAsync(int missionNoteId)
        {
            return await GetMissionsAsync("MissionNoteID = @missionNoteId",
                new MySqlParameter("@missionNoteId", missionNoteId));
        }

        public async Task<List<MissionDTO>> GetMissionsByVehiclesIdAsync(int missionVehiclesId)
        {
            return await GetMissionsAsync("MissionVehiclesID = @missionVehiclesId",
                new MySqlParameter("@missionVehicleId", missionVehiclesId));
        }

        public async Task<List<MissionDTO>> GetMissionsByStartDateAsync(DateTime startDate)
        {
            return await GetMissionsAsync("MissionStartDate = @startDate",
                new MySqlParameter("@startDate", startDate));
        }

        public async Task<List<MissionDTO>> GetMissionsByDestinationAsync(string destination)
        {
            return await GetMissionsAsync("DIstination = @destination",
                new MySqlParameter("@destination", destination));
        }

        public async Task<List<MissionDTO>> GetMissionsByUserIdAsync(int userId)
        {
            return await GetMissionsAsync("CreatedByUser = @userId",
                new MySqlParameter("@userId", userId));
        }

        public async Task<int> CreateMissionAsync(MissionDTO mission)
        {
            const string query = @"
                INSERT INTO missions (MissionNoteID, MissionVehiclesID, MissionStartDate, MissionEndDate, DIstination, CreatedByUser)
                VALUES (@missionNoteId, @missionVehiclesId, @missionStartDate, @missionEndDate, @destination, @createdByUser)
                SELECT LAST_INSERT_ID();";

            return await _connectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                return Convert.ToInt32(await cmd.ExecuteScalarAsync());
            }, new MySqlParameter("@missionNoteId", mission.MissionNoteId),
            new MySqlParameter("@missionVehiclesId", mission.MissionVehiclesId),
            new MySqlParameter("@missionStartDate", mission.StartDate),
            new MySqlParameter("@missionEndDate", mission.EndDate),
            new MySqlParameter("@destination", mission.Destination),
            new MySqlParameter("@createdByUser", mission.UserId));
        }

        public async Task<bool> UpdateMissionAsync(MissionDTO mission)
        {
            const string query = @"
                UPDATE missions
                SET 
                    MissionNoteID = @missionNoteId,
                    MissionVehiclesID = @missionVehiclesId,
                    MissionStartDate = @startDate,
                    MissionEndDate = @endDate,
                    DIstination = @destination,
                    CreatedByUser = @createdByUser
                WHERE
                    MissionID = @missionId";

            return await _connectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                return await cmd.ExecuteNonQueryAsync() > 0;
            }, new MySqlParameter("@missionId", mission.MissionId),
            new MySqlParameter("@missionNoteId", mission.MissionNoteId),
            new MySqlParameter("@missionVehiclesId", mission.MissionVehiclesId),
            new MySqlParameter("@missionStartDate", mission.StartDate),
            new MySqlParameter("@missionEndDate", mission.EndDate),
            new MySqlParameter("@destination", mission.Destination),
            new MySqlParameter("@createdByUser", mission.UserId));
        }

        public async Task<bool> DeleteMissionAsync(int missionId)
        {
            const string query = "DELETE FROM missions WHERE MissionID = @missionId";

            return await _connectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                return await cmd.ExecuteNonQueryAsync() > 0;
            }, new MySqlParameter("missionId", missionId));
        }
    }
}