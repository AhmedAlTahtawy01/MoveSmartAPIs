using DataAccessLayer.Util;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
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
        public static async Task<List<PatrolsSubscriptionDTO>> GetAllSubscriptionsForEmployeeAsync(int employeeID)
        {
            List<PatrolsSubscriptionDTO> subscriptionsList = new List<PatrolsSubscriptionDTO>();

            string query = @"SELECT * FROM PatrolsSubscriptions
                            WHERE EmployeeID = @EmployeeID"
            ;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnectionSettings._connectionString))
                {
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("EmployeeID", employeeID);
                        await conn.OpenAsync();

                        using (MySqlDataReader reader = await cmd.ExecuteReaderAsync())
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

        public static async Task<List<PatrolsSubscriptionDTO>> GetAllSubscriptionsForPatrolAsync(short patrolID)
        {
            List<PatrolsSubscriptionDTO> subscriptionsList = new List<PatrolsSubscriptionDTO>();

            string query = @"SELECT * FROM PatrolsSubscriptions
                            WHERE PatrolID = @PatrolID"
            ;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnectionSettings._connectionString))
                {
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("PatrolID", patrolID);
                        await conn.OpenAsync();

                        using (MySqlDataReader reader = await cmd.ExecuteReaderAsync())
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

        public static async Task<PatrolsSubscriptionDTO> GetSubscriptionRecordByIDAsync(int subscriptionID)
        {
            string query = @"SELECT * FROM PatrolsSubscriptions
                            WHERE SubscriptionID = @SubscriptionID"
            ;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnectionSettings._connectionString))
                {
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("SubscriptionID", subscriptionID);
                        await conn.OpenAsync();

                        using (MySqlDataReader reader = await cmd.ExecuteReaderAsync())
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

        public static async Task<int?> CreateNewSubscriptionRecordAsync(PatrolsSubscriptionDTO newSubscription)
        {
            string query = @"INSERT INTO PatrolsSubscriptions
                            (PatrolID, EmployeeID)
                            VALUES
                            (@PatrolID, @EmployeeID);

                            SELECT SCOPE_IDENTITY();"
            ;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnectionSettings._connectionString))
                {
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
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

        public static async Task<bool> UpdateSubscriptionRecordAsync(PatrolsSubscriptionDTO updatedSubscription)
        {
            string query = @"UPDATE PatrolsSubscriptions SET
                            PatrolID = @PatrolID,
                            EmployeeID = @EmployeeID
                            WHERE SubscriptionID = @SubscriptionID"
            ;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnectionSettings._connectionString))
                {
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
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

        public static async Task<bool> DeleteSubscriptionRecordAsync(int subscriptionID)
        {
            string query = @"DELETE FROM PatrolsSubscriptions
                            WHERE SubscriptionID = @SubscriptionID"
            ;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnectionSettings._connectionString))
                {
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
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
