using DataAccessLayer.Util;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class PatrolDTO
    {
        public short? PatrolID { get; set; }
        public string Description { get; set; }
        public TimeOnly MovingAt { get; set; }
        public float ApproximatedTime { get; set; }
        public byte BusID { get; set; }

        public PatrolDTO(short? patrolID, string description, TimeOnly movingAt, float approximatedTime,
            byte busID)
        {
            PatrolID = patrolID;
            Description = description;
            MovingAt = movingAt;
            ApproximatedTime = approximatedTime;
            BusID = busID;
        }
    }

    public class PatrolRepo
    {
        private readonly ConnectionSettings _connectionSettings;
        private readonly ILogger<PatrolRepo> _logger;

        public PatrolRepo(ConnectionSettings connectionSettings, ILogger<PatrolRepo> logger)
        {
            _connectionSettings = connectionSettings ?? throw new ArgumentNullException(nameof(connectionSettings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<PatrolDTO>> GetAllPatrolsAsync()
        {
            List<PatrolDTO> patrolsList = new List<PatrolDTO>();

            string query = @"SELECT * FROM Patrols
                            ORDER BY ApproximatedTime DESC";

            try
            {
                using (MySqlConnection conn = _connectionSettings.GetConnection())
                {
                    using (MySqlCommand cmd = _connectionSettings.GetCommand(query, conn))
                    {
                        await conn.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                patrolsList.Add(new PatrolDTO(
                                    Convert.ToInt16(reader["PatrolID"]),
                                    (string)reader["Description"],
                                    TimeOnly.FromTimeSpan((TimeSpan)reader["MovingAt"]),
                                    Convert.ToSingle(reader["ApproximatedTime"]),
                                    Convert.ToByte(reader["BusID"])
                                    ));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return patrolsList;
        }

        public async Task<PatrolDTO> GetPatrolByIDAsync(short patrolID)
        {
            string query = @"SELECT * FROM Patrols
                            WHERE PatrolID = @PatrolID";
            try
            {
                using (MySqlConnection conn = _connectionSettings.GetConnection())
                {
                    using (MySqlCommand cmd = _connectionSettings.GetCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("PatrolID", patrolID);

                        await conn.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new PatrolDTO(
                                    Convert.ToInt16(reader["PatrolID"]),
                                    (string)reader["Description"],
                                    TimeOnly.FromTimeSpan((TimeSpan)reader["MovingAt"]),
                                    Convert.ToSingle(reader["ApproximatedTime"]),
                                    Convert.ToByte(reader["BusID"])
                                    );
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return null;
        }

        public async Task<short?> AddNewPatrolAsync(PatrolDTO newPatrol)
        {
            string query = @"INSERT INTO Patrols
                            (Description, MovingAt, ApproximatedTime, BusID)
                            VALUES
                            (@Description, @MovingAt, @ApproximatedTime, @BusID);
                            SELECT LAST_INSERT_ID();";

            try
            {
                using (MySqlConnection conn = _connectionSettings.GetConnection())
                {
                    using (MySqlCommand cmd = _connectionSettings.GetCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("Description", newPatrol.Description);
                        cmd.Parameters.AddWithValue("MovingAt", newPatrol.MovingAt.ToString("HH:mm:ss"));
                        cmd.Parameters.AddWithValue("ApproximatedTime", newPatrol.ApproximatedTime);
                        cmd.Parameters.AddWithValue("BusID", newPatrol.BusID);

                        await conn.OpenAsync();

                        object? result = await cmd.ExecuteScalarAsync();
                        if (result != null && short.TryParse(result.ToString(), out short id))
                        {
                            return id;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return null;
        }

        public async Task<bool> UpdatePatrolAsync(PatrolDTO updatedPatrol)
        {
            string query = @"UPDATE Patrols SET 
                            Description = @Description,
                            MovingAt = @MovingAt,
                            ApproximatedTime = @ApproximatedTime,
                            BusID = @BusID
                            WHERE PatrolID = @PatrolID";

            try
            {
                using (MySqlConnection conn = _connectionSettings.GetConnection())
                {
                    using (MySqlCommand cmd = _connectionSettings.GetCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("PatrolID", updatedPatrol.PatrolID ?? 0);
                        cmd.Parameters.AddWithValue("Description", updatedPatrol.Description);
                        cmd.Parameters.AddWithValue("MovingAt", updatedPatrol.MovingAt.ToString("HH:mm:ss"));
                        cmd.Parameters.AddWithValue("ApproximatedTime", updatedPatrol.ApproximatedTime);
                        cmd.Parameters.AddWithValue("BusID", updatedPatrol.BusID);

                        await conn.OpenAsync();

                        return Convert.ToByte(await cmd.ExecuteNonQueryAsync()) > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return false;
        }

        public async Task<bool> DeletePatrolAsync(short patrolID)
        {
            string query = @"DELETE FROM Patrols
                            WHERE PatrolID = @PatrolID";

            try
            {
                using (MySqlConnection conn = _connectionSettings.GetConnection())
                {
                    using (MySqlCommand cmd = _connectionSettings.GetCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("PatrolID", patrolID);

                        await conn.OpenAsync();

                        return Convert.ToByte(await cmd.ExecuteNonQueryAsync()) > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return false;
        }

        public async Task<bool> IsPatrolExistsAsync(short patrolID)
        {
            string query = @"SELECT 1 AS Found FROM Patrols
                            WHERE PatrolID = @PatrolID";

            try
            {
                using (MySqlConnection conn = _connectionSettings.GetConnection())
                {
                    using (MySqlCommand cmd = _connectionSettings.GetCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("PatrolID", patrolID);

                        await conn.OpenAsync();
                        
                        return await cmd.ExecuteScalarAsync() != null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return false;
        }
    }
}
