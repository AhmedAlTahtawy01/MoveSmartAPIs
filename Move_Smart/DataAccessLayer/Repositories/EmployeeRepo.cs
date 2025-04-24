using DataAccessLayer.Util;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DataAccessLayer.EmployeeDTO;

namespace DataAccessLayer
{
    public class EmployeeDTO
    {
        public enum enTransportationSubscriptionStatus : byte
        {
            Valid = 0,
            Expired = 1,
            Unsubscribed = 2
        }

        public int? EmployeeID { get; set; }
        public string NationalNo { get; set; }
        public string Name { get; set; }
        public string JobTitle { get; set; }
        public string Phone { get; set; }
        public enTransportationSubscriptionStatus TransportationSubscriptionStatus { get; set; }

        public EmployeeDTO(int? employeeID, string nationalNo, string name, string jobTitle, string phone,
            enTransportationSubscriptionStatus transportationSubscriptionStatus)
        {
            EmployeeID = employeeID;
            NationalNo = nationalNo;
            Name = name;
            JobTitle = jobTitle;
            Phone = phone;
            TransportationSubscriptionStatus = transportationSubscriptionStatus;
        }
    }

    public class EmployeeRepo
    {
        private readonly ConnectionSettings _connectionSettings;
        private readonly ILogger<EmployeeRepo> _logger;

        public EmployeeRepo(ConnectionSettings connectionSettings, ILogger<EmployeeRepo> logger)
        {
            _connectionSettings = connectionSettings ?? throw new ArgumentNullException(nameof(connectionSettings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<EmployeeDTO>> GetAllEmployeesAsync()
        {
            List<EmployeeDTO> employeesList = new List<EmployeeDTO>();

            string query = @"SELECT * FROM Employees
                            ORDER BY Name ASC;";

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
                                employeesList.Add(new EmployeeDTO(
                                    Convert.ToInt32(reader["EmployeeID"]),
                                    (string)reader["NationalNo"],
                                    (string)reader["Name"],
                                    (string)reader["JobTitle"],
                                    (string)reader["Phone"],
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
            }

            return employeesList;
        }

        public async Task<List<EmployeeDTO>> GetAllEmployeesWhoAreUsingBusAsync(byte busID)
        {
            List<EmployeeDTO> employeesList = new List<EmployeeDTO>();

            string query = @"SELECT * FROM Employees
                            WHERE AssociatedBus = @BusID
                            ORDER BY Name ASC;";

            try
            {
                using (MySqlConnection conn = _connectionSettings.GetConnection())
                {
                    using (MySqlCommand cmd = _connectionSettings.GetCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("BusID", busID);

                        await conn.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                employeesList.Add(new EmployeeDTO(
                                    Convert.ToInt32(reader["EmployeeID"]),
                                    (string)reader["NationalNo"],
                                    (string)reader["Name"],
                                    (string)reader["JobTitle"],
                                    (string)reader["Phone"],
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
            }

            return employeesList;
        }

        public async Task<EmployeeDTO> GetEmployeeByIDAsync(int employeeID)
        {
            string query = @"SELECT * FROM Employees
                            WHERE EmployeeID = @EmployeeID;";
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
                            if (await reader.ReadAsync())
                            {
                                return new EmployeeDTO(
                                    Convert.ToInt32(reader["EmployeeID"]),
                                    (string)reader["NationalNo"],
                                    (string)reader["Name"],
                                    (string)reader["JobTitle"],
                                    (string)reader["Phone"],
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
            }

            return null;
        }

        public async Task<EmployeeDTO> GetEmployeeByNationalNoAsync(string nationalNo)
        {
            string query = @"SELECT * FROM Employees
                            WHERE NationalNo = @NationalNo;";
            try
            {
                using (MySqlConnection conn = _connectionSettings.GetConnection())
                {
                    using (MySqlCommand cmd = _connectionSettings.GetCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("NationalNo", nationalNo);

                        await conn.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new EmployeeDTO(
                                    Convert.ToInt32(reader["EmployeeID"]),
                                    (string)reader["NationalNo"],
                                    (string)reader["Name"],
                                    (string)reader["JobTitle"],
                                    (string)reader["Phone"],
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
            }

            return null;
        }

        public async Task<EmployeeDTO> GetEmployeeByPhoneAsync(string phone)
        {
            string query = @"SELECT * FROM Employees
                            WHERE Phone = @Phone;";
            try
            {
                using (MySqlConnection conn = _connectionSettings.GetConnection())
                {
                    using (MySqlCommand cmd = _connectionSettings.GetCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("Phone", phone);

                        await conn.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new EmployeeDTO(
                                    Convert.ToInt32(reader["EmployeeID"]),
                                    (string)reader["NationalNo"],
                                    (string)reader["Name"],
                                    (string)reader["JobTitle"],
                                    (string)reader["Phone"],
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
            }

            return null;
        }

        public async Task<int?> AddNewEmployeeAsync(EmployeeDTO newEmployee)
        {
            string query = @"INSERT INTO Employees
                            (EmployeeID, NationalNo, Name, JobTitle, Phone, TransportationSubscriptionStatus)
                            VALUES
                            (@EmployeeID, @NationalNo, @Name, @JobTitle, @Phone, @TransportationSubscriptionStatus);
                            SELECT LAST_INSERT_ID();";

            try
            {
                using (MySqlConnection conn = _connectionSettings.GetConnection())
                {
                    using (MySqlCommand cmd = _connectionSettings.GetCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("EmployeeID", newEmployee.EmployeeID);
                        cmd.Parameters.AddWithValue("NationalNo", newEmployee.NationalNo);
                        cmd.Parameters.AddWithValue("Name", newEmployee.Name);
                        cmd.Parameters.AddWithValue("JobTitle", newEmployee.JobTitle);
                        cmd.Parameters.AddWithValue("Phone", newEmployee.Phone);
                        cmd.Parameters.AddWithValue("TransportationSubscriptionStatus", newEmployee.TransportationSubscriptionStatus.ToString());

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

        public async Task<bool> UpdateEmployeeAsync(EmployeeDTO updatedEmployee)
        {
            string query = @"UPDATE Employees SET
                            NationalNo = @NationalNo,
                            Name = @Name,
                            JobTitle = @JobTitle,
                            Phone = @Phone,
                            TransportationSubscriptionStatus = @TransportationSubscriptionStatus
                            WHERE EmployeeID = @EmployeeID;";

            try
            {
                using (MySqlConnection conn = _connectionSettings.GetConnection())
                {
                    using (MySqlCommand cmd = _connectionSettings.GetCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("EmployeeID", updatedEmployee.EmployeeID ?? 0);
                        cmd.Parameters.AddWithValue("NationalNo", updatedEmployee.NationalNo);
                        cmd.Parameters.AddWithValue("Name", updatedEmployee.Name);
                        cmd.Parameters.AddWithValue("JobTitle", updatedEmployee.JobTitle);
                        cmd.Parameters.AddWithValue("Phone", updatedEmployee.Phone);
                        cmd.Parameters.AddWithValue("TransportationSubscriptionStatus", updatedEmployee.TransportationSubscriptionStatus.ToString());

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

        public async Task<bool> DeleteEmployeeAsync(string nationalNo)
        {
            string query = @"DELETE FROM Employees
                            WHERE NationalNo = @NationalNo;";

            try
            {
                using (MySqlConnection conn = _connectionSettings.GetConnection())
                {
                    using (MySqlCommand cmd = _connectionSettings.GetCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("NationalNo", nationalNo);

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

        public async Task<bool> DeleteEmployeeAsync(int employeeID)
        {
            string query = @"DELETE FROM Employees
                            WHERE EmployeeID = @EmployeeID;";

            try
            {
                using (MySqlConnection conn = _connectionSettings.GetConnection())
                {
                    using (MySqlCommand cmd = _connectionSettings.GetCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("EmployeeID", employeeID);
            
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

        public async Task<bool> IsEmployeeTransportationSubscriptionValidAsync(int employeeID)
        {
            string query = @"SELECT Valid = 1 FROM Employees
                            WHERE EmployeeID = @EmployeeID AND TransportationSubscriptionStatus = 'Valid';";

            try
            {
                using (MySqlConnection conn = _connectionSettings.GetConnection())
                {
                    using (MySqlCommand cmd = _connectionSettings.GetCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("EmployeeID", employeeID);

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

        public async Task<bool> IsEmployeeExistsAsync(string nationalNo)
        {
            string query = @"SELECT 1 AS Found FROM Employees
                            WHERE NationalNo = @NationalNo;";

            try
            {
                using (MySqlConnection conn = _connectionSettings.GetConnection())
                {
                    using (MySqlCommand cmd = _connectionSettings.GetCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("NationalNo", nationalNo);

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

        public async Task<bool> IsEmployeeExistsAsync(int employeeID)
        {
            string query = @"SELECT 1 AS Found FROM Employees
                            WHERE EmployeeID = @EmployeeID;";

            try
            {
                using (MySqlConnection conn = _connectionSettings.GetConnection())
                {
                    using (MySqlCommand cmd = _connectionSettings.GetCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("EmployeeID", employeeID);
            
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
