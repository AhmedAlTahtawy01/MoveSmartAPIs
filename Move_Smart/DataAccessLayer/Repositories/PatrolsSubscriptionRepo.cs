using DataAccessLayer.Util;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class PatrolsSubscriptionDTO
    {
        public int SubscriptionID { get; set; }
        public short PatrolID { get; set; }
        public int EmployeeID { get; set; }

        public PatrolsSubscriptionDTO(int subscriptionID, short patrolID, int employeeID)
        {
            SubscriptionID = subscriptionID;
            PatrolID = patrolID;
            EmployeeID = employeeID;
        }
    }

    public class PatrolsSubscriptionRepo
    {
        private readonly ConnectionSettings _connectionSettings;
        private readonly ILogger<PatrolsSubscriptionRepo> _logger;

        public PatrolsSubscriptionRepo(ConnectionSettings connectionSettings, ILogger<PatrolsSubscriptionRepo> logger)
        {
            _connectionSettings = connectionSettings ?? throw new ArgumentNullException(nameof(connectionSettings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<PatrolsSubscriptionDTO>> GetAllSubscriptionsForEmployeeAsync(int employeeID)
        {
            List<PatrolsSubscriptionDTO> subscriptionsList = new List<PatrolsSubscriptionDTO>();

            string query = @"SELECT * FROM PatrolsSubscriptions
                            WHERE EmployeeID = @EmployeeID"
            ;

            try
            {
                using (MySqlConnection conn = _connectionSettings.GetConnection())
                {
                    using (MySqlCommand cmd = _connectionSettings.GetCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("EmployeeID", employeeID);
                        await conn.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                subscriptionsList.Add(new PatrolsSubscriptionDTO(
                                    Convert.ToInt32(reader["SubscriptionID"]),
                                    Convert.ToInt16(reader["PatrolID"]),
                                    Convert.ToInt32(reader["EmployeeID"])
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

            return subscriptionsList;
        }

        public async Task<List<PatrolsSubscriptionDTO>> GetAllSubscriptionsForPatrolAsync(short patrolID)
        {
            List<PatrolsSubscriptionDTO> subscriptionsList = new List<PatrolsSubscriptionDTO>();

            string query = @"SELECT * FROM PatrolsSubscriptions
                            WHERE PatrolID = @PatrolID"
            ;

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
                            while (await reader.ReadAsync())
                            {
                                subscriptionsList.Add(new PatrolsSubscriptionDTO(
                                    Convert.ToInt32(reader["SubscriptionID"]),
                                    Convert.ToInt16(reader["PatrolID"]),
                                    Convert.ToInt32(reader["EmployeeID"])
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

            return subscriptionsList;
        }

        public async Task<PatrolsSubscriptionDTO> GetSubscriptionRecordByIDAsync(int subscriptionID)
        {
            string query = @"SELECT * FROM PatrolsSubscriptions
                            WHERE SubscriptionID = @SubscriptionID"
            ;

            try
            {
                using (MySqlConnection conn = _connectionSettings.GetConnection())
                {
                    using (MySqlCommand cmd = _connectionSettings.GetCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("SubscriptionID", subscriptionID);
                        await conn.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new PatrolsSubscriptionDTO(
                                    Convert.ToInt32(reader["SubscriptionID"]),
                                    Convert.ToInt16(reader["PatrolID"]),
                                    Convert.ToInt32(reader["EmployeeID"])
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

        public async Task<int?> CreateNewSubscriptionRecordAsync(PatrolsSubscriptionDTO newSubscription)
        {
            string query = @"INSERT INTO PatrolsSubscriptions
                            (PatrolID, EmployeeID)
                            VALUES
                            (@PatrolID, @EmployeeID);

                            SELECT SCOPE_IDENTITY();"
            ;

            try
            {
                using (MySqlConnection conn = _connectionSettings.GetConnection())
                {
                    using (MySqlCommand cmd = _connectionSettings.GetCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("PatrolID", newSubscription.PatrolID);
                        cmd.Parameters.AddWithValue("EmployeeID", newSubscription.EmployeeID);

                        await conn.OpenAsync();

                        object? result = await cmd.ExecuteScalarAsync();
                        if (result != null && int.TryParse(result.ToString(), out int id))
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

        public async Task<bool> UpdateSubscriptionRecordAsync(PatrolsSubscriptionDTO updatedSubscription)
        {
            string query = @"UPDATE PatrolsSubscriptions SET
                            PatrolID = @PatrolID,
                            EmployeeID = @EmployeeID
                            WHERE SubscriptionID = @SubscriptionID"
            ;

            try
            {
                using (MySqlConnection conn = _connectionSettings.GetConnection())
                {
                    using (MySqlCommand cmd = _connectionSettings.GetCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("SubscriptionID", updatedSubscription.SubscriptionID);
                        cmd.Parameters.AddWithValue("PatrolID", updatedSubscription.PatrolID);
                        cmd.Parameters.AddWithValue("EmployeeID", updatedSubscription.EmployeeID);

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

        public async Task<bool> DeleteSubscriptionRecordAsync(int subscriptionID)
        {
            string query = @"DELETE FROM PatrolsSubscriptions
                            WHERE SubscriptionID = @SubscriptionID"
            ;

            try
            {
                using (MySqlConnection conn = _connectionSettings.GetConnection())
                {
                    using (MySqlCommand cmd = _connectionSettings.GetCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("SubscriptionID", subscriptionID);

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
    }
}
