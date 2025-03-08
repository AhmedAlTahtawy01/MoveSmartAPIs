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
    public class MissionsVehicle
    {
        public int MissionVehicleId { get; set; }
        public int MissionId { get; set; }
        public int VehicleId { get; set; }

        public MissionsVehicle(int missionVehicleId, int missiondId, int vehicleId)
        {
            this.MissionVehicleId = missionVehicleId;
            this.MissionId = missiondId;
            this.VehicleId = vehicleId;
        }


    }

    public class MissionsVehicleRepo
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

        private MissionsVehicle MapMissionsVehicle(DbDataReader reader)
        {
            return new MissionsVehicle
            (
                reader.GetInt32("MissionVehicleID"),
                reader.GetInt32("MissionID"),
                reader.GetInt32("VehicleID")
            );
        }

        public async Task<List<MissionsVehicle>> GetAllMissionVehiclesAsync()
        {
            var missionsVehicles = new List<MissionsVehicle>();

            await using (var conn = GetConnection())
            {
                var query = @"
                    SELECT MissionVehicleID, MissionID, VehicleID
                    FROM missionsvehicles";

                using (var cmd = GetCommand(query, conn))
                {
                    try
                    {
                        await conn.OpenAsync().ConfigureAwait(false);

                        using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            while (await reader.ReadAsync())
                            {
                                missionsVehicles.Add(MapMissionsVehicle(reader));
                            }
                        }
                    }
                    catch (MySqlException ex)
                    {
                        throw new Exception("Database error occurred in GetAllMissionVehiclesAsync.", ex);
                    }
                }

                return missionsVehicles;
            }
        }

        public async Task<MissionsVehicle> GetMissionVehicleByIdAsync(int missionVehicleId)
        {
            await using (var conn = GetConnection())
            {
                var query = @"
                    SELECT MissionVehicleID, MissionID, VehicleID
                    FROM missionsjoborders
                    WHERE MissionVehicleID = @missionVehicleId";

                using (var cmd = GetCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@missionVehicleId", missionVehicleId);

                    try
                    {
                        await conn.OpenAsync().ConfigureAwait(false);
                        using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            if (await reader.ReadAsync())
                            {
                                return MapMissionsVehicle(reader);
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                    catch (MySqlException ex)
                    {
                        throw new Exception("Database error occurred in GetMissionVehicleByIdAsync.", ex);
                    }
                }
            }
        }

        public async Task<List<MissionsVehicle>> GetMissionVehiclesByMissionIdAsync(int missionId)
        {
            var missionVehicles = new List<MissionsVehicle>();

            await using (var conn = GetConnection())
            {
                var query = @"
                    SELECT MissionVehicleID, MissionID, VehicleID
                    FROM missionsjoborders
                    WHERE MissionID = @missionId";

                using (var cmd = GetCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@missionId", missionId);

                    try
                    {
                        await conn.OpenAsync().ConfigureAwait(false);
                        using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            while (await reader.ReadAsync())
                            {
                                missionVehicles.Add(MapMissionsVehicle(reader));
                            }
                        }
                    }
                    catch (MySqlException ex)
                    {
                        throw new Exception("Database error occurred in GetMissionVehiclesByMissionIdAsync.", ex);
                    }
                }
            }

            return missionVehicles;
        }

        public async Task<List<MissionsVehicle>> GetMissionVehiclesByVehicleIdAsync(int vehicleId)
        {
            var missionVehicles = new List<MissionsVehicle>();

            await using (var conn = GetConnection())
            {
                var query = @"
                    SELECT MissionVehicleID, MissionID, VehicleID
                    FROM missionsvehicles
                    WHERE VehicleID = @vehicleId";

                using (var cmd = GetCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@vehicleId", vehicleId);

                    try
                    {
                        await conn.OpenAsync().ConfigureAwait(false);
                        using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            while (await reader.ReadAsync())
                            {
                                missionVehicles.Add(MapMissionsVehicle(reader));
                            }
                        }
                    }
                    catch (MySqlException ex)
                    {
                        throw new Exception("Database error occurred in GetMissionVehiclesByVehicleIdAsync.", ex);
                    }
                }
            }

            return missionVehicles;
        }

        public async Task<int> CreateMissionVehicleAsync(MissionsVehicle missionsVehicle)
        {
            await using (var conn = GetConnection())
            {
                var query = @"
                    INSERT INTO missionsvehicles (MissionID, VehicleID)
                    VALUES (@missionId, @vehicleId)
                    SELECT LAST_INSERT_ID();";

                using (var cmd = GetCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@missionId", missionsVehicle.MissionId);
                    cmd.Parameters.AddWithValue("@vehicleId", missionsVehicle.VehicleId);

                    try
                    {
                        await conn.OpenAsync().ConfigureAwait(false);
                        var newMissionsVehicleId = Convert.ToInt32(await cmd.ExecuteScalarAsync().ConfigureAwait(false));

                        return newMissionsVehicleId;
                    }
                    catch (MySqlException ex)
                    {
                        throw new Exception("Database error occurred in CreateMissionVehicleAsync.", ex);
                    }
                }
            }
        }

        public async Task<bool> UpdateMissionsVehicleAsync(MissionsVehicle missionsVehicle)
        {
            await using (var conn = GetConnection())
            {
                var query = @"
                    UPDATE missionsvehicles
                    SET 
                        MissionID = @missionId,
                        VehicleID = @vehicleId
                    WHERE
                        MissionVehicleID = @missionVehicleId";

                using (var cmd = GetCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@missionVehicleId", missionsVehicle.MissionVehicleId);
                    cmd.Parameters.AddWithValue("@missionId", missionsVehicle.MissionId);
                    cmd.Parameters.AddWithValue("@vehicleId", missionsVehicle.VehicleId);

                    try
                    {
                        await conn.OpenAsync().ConfigureAwait(false);
                        var rowsAffected = await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);

                        return rowsAffected > 0;
                    }
                    catch (MySqlException ex)
                    {
                        throw new Exception("Database error occurred in UpdateMissionsVehicleAsync.", ex);
                    }
                }
            }
        }

        public async Task<bool> DeleteMissionsVehicleAsync(int missionsVehicleId)
        {
            await using (var conn = GetConnection())
            {
                var query = "DELETE FROM missionsvehicles WHERE MissionVehicleID = @missionsVehicleId";
                using (var cmd = GetCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@missionsVehicleId", missionsVehicleId);

                    try
                    {
                        await conn.OpenAsync().ConfigureAwait(false);
                        var rowsAffected = await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);

                        return rowsAffected > 0;
                    }
                    catch (MySqlException ex)
                    {
                        throw new Exception("Database error occurred in DeleteMissionsVehicleAsync.", ex);
                    }
                }
            }
        }

    }
}
