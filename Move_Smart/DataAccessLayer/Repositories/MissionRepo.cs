using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class Mission
    {
        public int MissionId { get; set; }
        public int MissionNoteId { get; set; }
        public int MissionVehiclesId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Destination { get; set; }
        public int UserId { get; set; }

        public Mission(int missionId, int missionNoteId, int missionVechiclesId, DateTime startDate, DateTime endDate, string destination, int createdByUser)
        {
            this.MissionId = missionId;
            this.MissionNoteId = missionNoteId;
            this.MissionVehiclesId = missionVechiclesId;
            this.StartDate = startDate;
            this.EndDate = endDate;
            this.Destination = destination;
            this.UserId = createdByUser;
        }
    }

    public class MissionRepo
    {
        private static readonly string _connectionString = "Server=localhost;Database=MoveSmart;User Id=root;Password=ahmedroot;";

        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }

        private MySqlCommand GetCommand(string query, MySqlConnection conn)
        {
            var cmd = new MySqlCommand(query, conn);
            cmd.CommandType = CommandType.Text;
            return cmd;
        }

        private Mission MapMission(DbDataReader reader)
        {
            return new Mission
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

        public async Task<List<Mission>> GetAllMissionsAsync()
        {
            var missions = new List<Mission>();

            await using (var conn = GetConnection())
            {
                var query = @"
                    SELECT MissionID, MissionNoteID, MissionVehiclesID, MissionStartDate, MissionEndDate, DIstination, CreatedByUser
                    FROM missions";

                using (var cmd = GetCommand(query, conn))
                {
                    try
                    {
                        await conn.OpenAsync().ConfigureAwait(false);

                        using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            while (await reader.ReadAsync())
                            {
                                missions.Add(MapMission(reader));
                            }
                        }
                    }
                    catch (MySqlException ex)
                    {
                        throw new Exception("Database error occurred in GetAllMissionsAsync.", ex);
                    }
                }

                return missions;
            }
        }

        public async Task<Mission> GetMissionByIdAsync(int missionId)
        {
            await using (var conn = GetConnection())
            {
                var query = @"
                    SELECT MissionID, MissionNoteID, MissionVehiclesID, MissionStartDate, MissionEndDate, DIstination, CreatedByUser
                    FROM missions
                    WHERE MissionID = @missionId";

                using (var cmd = GetCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@missionId", missionId);

                    try
                    {
                        await conn.OpenAsync().ConfigureAwait(false);
                        using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            if (await reader.ReadAsync())
                            {
                                return MapMission(reader);
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                    catch (MySqlException ex)
                    {
                        throw new Exception("Database error occurred in GetMissionByIdAsync.", ex);
                    }
                }
            }
        }

        public async Task<List<Mission>> GetMissionsByNoteIdAsync(int missionNoteId)
        {
            var missions = new List<Mission>();

            await using (var conn = GetConnection())
            {
                var query = @"
                    SELECT MissionID, MissionNoteID, MissionVehiclesID, MissionStartDate, MissionEndDate, DIstination, CreatedByUser
                    FROM missions
                    WHERE MissionNoteID = @missionNoteId";

                using (var cmd = GetCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@missionNoteId", missionNoteId);

                    try
                    {
                        await conn.OpenAsync().ConfigureAwait(false);
                        using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            while (await reader.ReadAsync())
                            {
                                missions.Add(MapMission(reader));
                            }
                        }
                    }
                    catch (MySqlException ex)
                    {
                        throw new Exception("Database error occurred in GetMissionsByNoteIdAsync.", ex);
                    }
                }
            }

            return missions;
        }

        public async Task<List<Mission>> GetMissionsByVehiclesIdAsync(int missionVehiclesId)
        {
            var missions = new List<Mission>();

            await using (var conn = GetConnection())
            {
                var query = @"
                    SELECT MissionID, MissionNoteID, MissionVehiclesID, MissionStartDate, MissionEndDate, DIstination, CreatedByUser
                    FROM missions
                    WHERE MissionVehiclesID = @missionVehiclesId";

                using (var cmd = GetCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@missionVehiclesId", missionVehiclesId);

                    try
                    {
                        await conn.OpenAsync().ConfigureAwait(false);
                        using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            while (await reader.ReadAsync())
                            {
                                missions.Add(MapMission(reader));
                            }
                        }
                    }
                    catch (MySqlException ex)
                    {
                        throw new Exception("Database error occurred in GetMissionsByVehiclesIdAsync.", ex);
                    }
                }
            }

            return missions;
        }

        public async Task<List<Mission>> GetMissionsByStartDateAsync(DateTime startDate)
        {
            var missions = new List<Mission>();

            await using (var conn = GetConnection())
            {
                var query = @"
                    SELECT MissionID, MissionNoteID, MissionVehiclesID, MissionStartDate, MissionEndDate, DIstination, CreatedByUser
                    FROM missions
                    WHERE MissionStartDate = @startDate";

                using (var cmd = GetCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@startDate", startDate);

                    try
                    {
                        await conn.OpenAsync().ConfigureAwait(false);
                        using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            while (await reader.ReadAsync())
                            {
                                missions.Add(MapMission(reader));
                            }
                        }
                    }
                    catch (MySqlException ex)
                    {
                        throw new Exception("Database error occurred in GetMissionsByStartDateAsync.", ex);
                    }
                }
            }

            return missions;
        }

        public async Task<List<Mission>> GetMissionsByDestinationAsync(string destination)
        {
            var missions = new List<Mission>();

            await using (var conn = GetConnection())
            {
                var query = @"
                    SELECT MissionID, MissionNoteID, MissionVehiclesID, MissionStartDate, MissionEndDate, DIstination, CreatedByUser
                    FROM missions
                    WHERE DIstination = @destination";

                using (var cmd = GetCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@destination", destination);

                    try
                    {
                        await conn.OpenAsync().ConfigureAwait(false);
                        using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            while (await reader.ReadAsync())
                            {
                                missions.Add(MapMission(reader));
                            }
                        }
                    }
                    catch (MySqlException ex)
                    {
                        throw new Exception("Database error occurred in GetMissionsByDestinationAsync.", ex);
                    }
                }
            }

            return missions;
        }

        public async Task<List<Mission>> GetMissionsByUserIdAsync(int userId)
        {
            var missions = new List<Mission>();

            await using (var conn = GetConnection())
            {
                var query = @"
                    SELECT MissionID, MissionNoteID, MissionVehiclesID, MissionStartDate, MissionEndDate, DIstination, CreatedByUser
                    FROM missions
                    WHERE CreatedByUser = @userId";

                using (var cmd = GetCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);

                    try
                    {
                        await conn.OpenAsync().ConfigureAwait(false);
                        using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            while (await reader.ReadAsync())
                            {
                                missions.Add(MapMission(reader));
                            }
                        }
                    }
                    catch (MySqlException ex)
                    {
                        throw new Exception("Database error occurred in GetMissionsByUserIdAsync.", ex);
                    }
                }
            }

            return missions;
        }

        public async Task<int> CreateMissionAsync(Mission mission)
        {
            await using (var conn = GetConnection())
            {
                var query = @"
                    INSERT INTO missions (MissionNoteID, MissionVehiclesID, MissionStartDate, MissionEndDate, DIstination, CreatedByUser)
                    VALUES (@missionNoteId, @missionVehiclesId, @missionStartDate, @missionEndDate, @destination, @createdByUser)
                    SELECT LAST_INSERT_ID();";

                using (var cmd = GetCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@missionNoteId", mission.MissionNoteId);
                    cmd.Parameters.AddWithValue("@missionVehiclesId", mission.MissionVehiclesId);
                    cmd.Parameters.AddWithValue("@missionStartDate", mission.StartDate);
                    cmd.Parameters.AddWithValue("@missionEndDate", mission.EndDate);
                    cmd.Parameters.AddWithValue("@destination", mission.Destination);
                    cmd.Parameters.AddWithValue("@createdByUser", mission.UserId);

                    try
                    {
                        await conn.OpenAsync().ConfigureAwait(false);
                        var newMissionId = Convert.ToInt32(await cmd.ExecuteScalarAsync().ConfigureAwait(false));

                        return newMissionId;
                    }
                    catch (MySqlException ex)
                    {
                        throw new Exception("Database error occurred in CreateMissionAsync.", ex);
                    }
                }
            }
        }

        public async Task<bool> UpdateMissionAsync(Mission mission)
        {
            await using (var conn = GetConnection())
            {
                var query = @"
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

                using (var cmd = GetCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@missionId", mission.MissionId);
                    cmd.Parameters.AddWithValue("@missionNoteId", mission.MissionNoteId);
                    cmd.Parameters.AddWithValue("@missionVehiclesId", mission.MissionVehiclesId);
                    cmd.Parameters.AddWithValue("@startDate", mission.StartDate);
                    cmd.Parameters.AddWithValue("@endDate", mission.EndDate);
                    cmd.Parameters.AddWithValue("@destination", mission.Destination);
                    cmd.Parameters.AddWithValue("@createdByUser", mission.UserId);

                    try
                    {
                        await conn.OpenAsync().ConfigureAwait(false);
                        var rowsAffected = await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);

                        return rowsAffected > 0;
                    }
                    catch (MySqlException ex)
                    {
                        throw new Exception("Database error occurred in UpdateMissionAsync.", ex);
                    }
                }
            }
        }

        public async Task<bool> DeleteMissionAsync(int missionId)
        {
            await using (var conn = GetConnection())
            {
                var query = "DELETE FROM missions WHERE MissionID = @missionId";
                using (var cmd = GetCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@missionId", missionId);

                    try
                    {
                        await conn.OpenAsync().ConfigureAwait(false);
                        var rowsAffected = await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);

                        return rowsAffected > 0;
                    }
                    catch (MySqlException ex)
                    {
                        throw new Exception("Database error occurred in DeleteMissionAsync.", ex);
                    }
                }
            }
        }
    }
}