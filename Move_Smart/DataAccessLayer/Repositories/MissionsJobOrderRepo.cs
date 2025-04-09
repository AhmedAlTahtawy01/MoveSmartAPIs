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
    public class MissionsJobOrder
    {
        public int OrderId { get; set; }
        public int MissionId { get; set; }
        public int JobOrderId { get; set; }

        public MissionsJobOrder(int orderId, int missionId, int jobOrderId)
        {
            this.OrderId = orderId;
            this.MissionId = missionId;
            this.JobOrderId = jobOrderId;
        }
    }

    public class MissionsJobOrderRepo
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

        private MissionsJobOrder MapMissionsJobOrder(DbDataReader reader)
        {
            return new MissionsJobOrder
            (
                reader.GetInt32("OrderID"),
                reader.GetInt32("MissionID"),
                reader.GetInt32("JobOrderID")
            );
        }

        public async Task<List<MissionsJobOrder>> GetAllMissionsJobOrdersAsync()
        {
            var missionsJobOrders = new List<MissionsJobOrder>();

            await using (var conn = GetConnection())
            {
                var query = @"
                    SELECT OrderID, MissionID, JobOrderID
                    FROM missionsjoborders";

                using (var cmd = GetCommand(query, conn))
                {
                    try
                    {
                        await conn.OpenAsync().ConfigureAwait(false);

                        using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            while (await reader.ReadAsync())
                            {
                                missionsJobOrders.Add(MapMissionsJobOrder(reader));
                            }
                        }
                    }
                    catch (MySqlException ex)
                    {
                        throw new Exception("Database error occurred in GetAllMissionsJobOrdersAsync.", ex);
                    }
                }

                return missionsJobOrders;
            }
        }

        public async Task<MissionsJobOrder?> GetMissionsJobOrderByIdAsync(int orderId)
        {
            await using (var conn = GetConnection())
            {
                var query = @"
                    SELECT OrderID, MissionID, JobOrderID
                    FROM missionsjoborders
                    WHERE OrderID = @orderId";

                using (var cmd = GetCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@orderId", orderId);

                    try
                    {
                        await conn.OpenAsync().ConfigureAwait(false);
                        using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            if (await reader.ReadAsync())
                            {
                                return MapMissionsJobOrder(reader);
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                    catch (MySqlException ex)
                    {
                        throw new Exception("Database error occurred in GetMissionsJobOrderByIdAsync.", ex);
                    }
                }
            }
        }

        public async Task<List<MissionsJobOrder>> GetMissionsJobOrdersByMissionIdAsync(int missionId)
        {
            var missionsJobOrders = new List<MissionsJobOrder>();

            await using (var conn = GetConnection())
            {
                var query = @"
                    SELECT OrderID, MissionID, JobOrderID
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
                                missionsJobOrders.Add(MapMissionsJobOrder(reader));
                            }
                        }
                    }
                    catch (MySqlException ex)
                    {
                        throw new Exception("Database error occurred in GetMissionsJobOrdersByMissionIdAsync.", ex);
                    }
                }
            }

            return missionsJobOrders;
        }

        public async Task<List<MissionsJobOrder>> GetMissionsJobOrdersByJobOrderIdAsync(int jobOrderId)
        {
            var missionsJobOrders = new List<MissionsJobOrder>();

            await using (var conn = GetConnection())
            {
                var query = @"
                    SELECT OrderID, MissionID, JobOrderID
                    FROM missionsjoborders
                    WHERE JobOrderID = @jobOrderId";

                using (var cmd = GetCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@jobOrderId", jobOrderId);

                    try
                    {
                        await conn.OpenAsync().ConfigureAwait(false);
                        using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            while (await reader.ReadAsync())
                            {
                                missionsJobOrders.Add(MapMissionsJobOrder(reader));
                            }
                        }
                    }
                    catch (MySqlException ex)
                    {
                        throw new Exception("Database error occurred in GetMissionsJobOrdersByJobOrderIdAsync.", ex);
                    }
                }
            }

            return missionsJobOrders;
        }

        public async Task<int> CreateMissionsJobOrderAsync(MissionsJobOrder missionsJobOrder)
        {
            await using (var conn = GetConnection())
            {
                var query = @"
                    INSERT INTO missionsjoborders (MissionID, JobOrderID)
                    VALUES (@missionId, @jobOrderId)
                    SELECT LAST_INSERT_ID();";

                using (var cmd = GetCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@missionId", missionsJobOrder.MissionId);
                    cmd.Parameters.AddWithValue("@jobOrderId", missionsJobOrder.JobOrderId);

                    try
                    {
                        await conn.OpenAsync().ConfigureAwait(false);
                        var newMissionsJobOrderId = Convert.ToInt32(await cmd.ExecuteScalarAsync().ConfigureAwait(false));

                        return newMissionsJobOrderId;
                    }
                    catch (MySqlException ex)
                    {
                        throw new Exception("Database error occurred in CreateMissionsJobOrderAsync.", ex);
                    }
                }
            }
        }

        public async Task<bool> UpdateMissionsJobOrderAsync(MissionsJobOrder missionsJobOrder)
        {
            await using (var conn = GetConnection())
            {
                var query = @"
                    UPDATE missionsjoborders
                    SET 
                        MissionID = @missionId,
                        JobOrderID = @jobOrderId
                    WHERE
                        OrderID = @orderId";

                using (var cmd = GetCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@orderId", missionsJobOrder.OrderId);
                    cmd.Parameters.AddWithValue("@missionId", missionsJobOrder.MissionId);
                    cmd.Parameters.AddWithValue("@jobOrderId", missionsJobOrder.JobOrderId);

                    try
                    {
                        await conn.OpenAsync().ConfigureAwait(false);
                        var rowsAffected = await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);

                        return rowsAffected > 0;
                    }
                    catch (MySqlException ex)
                    {
                        throw new Exception("Database error occurred in UpdateMissionsJobOrderAsync.", ex);
                    }
                }
            }
        }

        public async Task<bool> DeleteMissionsJobOrderAsync(int orderId)
        {
            await using (var conn = GetConnection())
            {
                var query = "DELETE FROM missionsjoborders WHERE OrderID = @orderId";
                using (var cmd = GetCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@orderId", orderId);

                    try
                    {
                        await conn.OpenAsync().ConfigureAwait(false);
                        var rowsAffected = await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);

                        return rowsAffected > 0;
                    }
                    catch (MySqlException ex)
                    {
                        throw new Exception("Database error occurred in DeleteMissionsJobOrderAsync.", ex);
                    }
                }
            }
        }
    }
}
