using DataAccessLayer.Util;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DataAccessLayer.PatrolsSubscriptionDTO;

namespace DataAccessLayer
{
    public class PatrolsSubscriptionDTO
    {
        public enum enTransportationSubscriptionStatus : byte
        {
            Valid = 0,
            Expired = 1,
            Unsubscribed = 2
        }
        public int? SubscriptionID { get; set; }
        public short PatrolID { get; set; }
        public int EmployeeID { get; set; }
        public enTransportationSubscriptionStatus TransportationSubscriptionStatus { get; set; }

        public PatrolsSubscriptionDTO(int? subscriptionID, short patrolID, int employeeID,
            enTransportationSubscriptionStatus transportationSubscriptionStatus)
        {
            SubscriptionID = subscriptionID;
            PatrolID = patrolID;
            EmployeeID = employeeID;
            TransportationSubscriptionStatus = transportationSubscriptionStatus;
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

        public async Task<List<PatrolsSubscriptionDTO>> GetAllSubscriptionsAsync()
        {
            List<PatrolsSubscriptionDTO> subscriptionsList = new List<PatrolsSubscriptionDTO>();
            string query = @"SELECT * FROM patrolsubscriptions";
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
                                subscriptionsList.Add(new PatrolsSubscriptionDTO(
                                    Convert.ToInt32(reader["SubscriptionID"]),
                                    Convert.ToInt16(reader["PatrolID"]),
                                    Convert.ToInt32(reader["EmployeeID"]),
                                    (enTransportationSubscriptionStatus)Enum.Parse(typeof(enTransportationSubscriptionStatus), reader["TransportationSubscriptionStatus"].ToString() ?? string.Empty)
                                ));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception($"Database error occurred in GetAllSubscriptionsAsync.", ex);
            }
            return subscriptionsList;
        }

        public async Task<List<PatrolsSubscriptionDTO>> GetAllSubscriptionsForEmployeeAsync(int employeeID)
        {
            List<PatrolsSubscriptionDTO> subscriptionsList = new List<PatrolsSubscriptionDTO>();

            string query = @"SELECT * FROM patrolsubscriptions
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
                                    Convert.ToInt32(reader["EmployeeID"]),
                                    (enTransportationSubscriptionStatus)Enum.Parse(typeof(enTransportationSubscriptionStatus), reader["TransportationSubscriptionStatus"].ToString() ?? string.Empty)
                                ));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception($"Database error occurred in GetAllSubscriptionsForEmployeeAsync.", ex);
            }

            return subscriptionsList;
        }

        public async Task<List<PatrolsSubscriptionDTO>> GetAllSubscriptionsForPatrolAsync(short patrolID)
        {
            List<PatrolsSubscriptionDTO> subscriptionsList = new List<PatrolsSubscriptionDTO>();

            string query = @"SELECT * FROM patrolsubscriptions
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
                                    Convert.ToInt32(reader["EmployeeID"]),
                                    (enTransportationSubscriptionStatus)Enum.Parse(typeof(enTransportationSubscriptionStatus), reader["TransportationSubscriptionStatus"].ToString() ?? string.Empty)
                                ));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception($"Database error occurred in GetAllSubscriptionsForPatrolAsync.", ex);
            }

            return subscriptionsList;
        }

        public async Task<PatrolsSubscriptionDTO?> GetSubscriptionRecordByIDAsync(int subscriptionID)
        {
            string query = @"SELECT * FROM patrolsubscriptions
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
                                    Convert.ToInt32(reader["EmployeeID"]),
                                    (enTransportationSubscriptionStatus)Enum.Parse(typeof(enTransportationSubscriptionStatus), reader["TransportationSubscriptionStatus"].ToString() ?? string.Empty)
                                );
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception($"Database error occurred in GetSubscriptionRecordByIDAsync.", ex);
            }

            return null;
        }

        public async Task<int?> CreateNewSubscriptionRecordAsync(PatrolsSubscriptionDTO newSubscription)
        {
            string query = @"INSERT INTO patrolsubscriptions
                            (PatrolID, EmployeeID, TransportationSubscriptionStatus)
                            VALUES
                            (@PatrolID, @EmployeeID, TransportationSubscriptionStatus);

                            SELECT LAST_INSERT_ID();"
            ;

            try
            {
                using (MySqlConnection conn = _connectionSettings.GetConnection())
                {
                    using (MySqlCommand cmd = _connectionSettings.GetCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("PatrolID", newSubscription.PatrolID);
                        cmd.Parameters.AddWithValue("EmployeeID", newSubscription.EmployeeID);
                        cmd.Parameters.AddWithValue("TransportationSubscriptionStatus", newSubscription.TransportationSubscriptionStatus.ToString());

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
                throw new Exception($"Database error occurred in CreateNewSubscriptionRecordAsync.", ex);
            }

            return null;
        }

        public async Task<bool> UpdateSubscriptionRecordAsync(PatrolsSubscriptionDTO updatedSubscription)
        {
            string query = @"UPDATE patrolsubscriptions SET
                            PatrolID = @PatrolID,
                            EmployeeID = @EmployeeID,
                            TransportationSubscriptionStatus = @TransportationSubscriptionStatus
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
                        cmd.Parameters.AddWithValue("TransportationSubscriptionStatus", updatedSubscription.TransportationSubscriptionStatus.ToString());

                        await conn.OpenAsync();

                        return Convert.ToByte(await cmd.ExecuteNonQueryAsync()) > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception($"Database error occurred in UpdateSubscriptionRecordAsync.", ex);
            }

            return false;
        }

        public async Task<bool> DeleteSubscriptionRecordAsync(int subscriptionID)
        {
            string query = @"DELETE FROM patrolsubscriptions
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
                throw new Exception($"Database error occurred in DeleteSubscriptionRecordAsync.", ex);
            }

            return false;
        }

        public async Task<bool> IsPatrolSubscriptionExistsAsync(int subscriptionID)
        {
            string query = @"SELECT 1 AS Found FROM patrolsubscriptions
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

                        return await cmd.ExecuteScalarAsync() != null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception($"Database error occurred in IsPatrolSubscriptionExistsAsync.", ex);
            }

            return false;
        }

        public async Task<int> GetNumberOfPatrolsSubscriptionsAsync()
        {
            string query = @"SELECT COUNT(*) FROM patrolsubscriptions";

            try
            {
                using (MySqlConnection conn = _connectionSettings.GetConnection())
                {
                    using (MySqlCommand cmd = _connectionSettings.GetCommand(query, conn))
                    {
                        await conn.OpenAsync();

                        object? result = await cmd.ExecuteScalarAsync();
                        if(result != null && int.TryParse(result.ToString(), out int count))
                        {
                            return count;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception($"Database error occurred in GetNumberOfPatrolsSubscriptionsAsync.", ex);
            }
    
            return 0;
        }
    }
}
