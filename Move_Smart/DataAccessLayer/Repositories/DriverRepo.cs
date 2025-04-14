using DataAccessLayer.Util;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DataAccessLayer.DriverDTO;

namespace DataAccessLayer
{
    public class DriverDTO
    {
        public enum enDriverStatus { Available, Absent, Working };
        public int? DriverID { get; set; }
        public string NationalNo { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public enDriverStatus Status { get; set; }
        public short VehicleID { get; set; }

        public DriverDTO(int? driverID, string nationalNo, string name, string phone,
            enDriverStatus status, short vehicleID)
        {
            DriverID = driverID;
            NationalNo = nationalNo;
            Name = name;
            Phone = phone;
            Status = status;
            VehicleID = vehicleID;
        }
    }

    public class DriverRepo
    {
        private readonly ConnectionSettings _connectionSettings;
        private readonly ILogger<DriverRepo> _logger;

        public DriverRepo(ConnectionSettings connectionSettings, ILogger<DriverRepo> logger)
        {
            _connectionSettings = connectionSettings ?? throw new ArgumentNullException(nameof(connectionSettings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<DriverDTO>> GetAllDriversAsync()
        {
            List<DriverDTO> driversList = new List<DriverDTO>();

            string query = @"SELECT * FROM Drivers
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
                                driversList.Add(new DriverDTO(
                                    Convert.ToInt32(reader["DriverID"]),
                                    (string)reader["NationalNo"],
                                    (string)reader["Name"],
                                    (string)reader["Phone"],
                                    (enDriverStatus)Enum.Parse(typeof(enDriverStatus), reader["Status"].ToString() ?? string.Empty),
                                    Convert.ToInt16(reader["VehicleID"])
                                ));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return driversList;
        }

        public async Task<List<DriverDTO>> GetDriversByVehicleIDAsync(short vehicleID)
        {
            List<DriverDTO> driversList = new List<DriverDTO>();

            string query = @"SELECT * FROM Drivers
                            WHERE VehicleID = @VehicleID
                            ORDER BY Name ASC;";

            try
            {
                using (MySqlConnection conn = _connectionSettings.GetConnection())
                {
                    using (MySqlCommand cmd = _connectionSettings.GetCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("VehicleID", vehicleID);

                        await conn.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                driversList.Add(new DriverDTO(
                                    Convert.ToInt32(reader["DriverID"]),
                                    (string)reader["NationalNo"],
                                    (string)reader["Name"],
                                    (string)reader["Phone"],
                                    (enDriverStatus)Enum.Parse(typeof(enDriverStatus), reader["Status"].ToString() ?? string.Empty),
                                    Convert.ToInt16(reader["VehicleID"])
                                ));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return driversList;
        }

        public async Task<List<DriverDTO>> GetDriversByVehiclePlateNumbersAsync(string plateNumbers)
        {
            List<DriverDTO> driversList = new List<DriverDTO>();

            string query = @"SELECT Drivers.* FROM
                            Drivers JOIN Vehicles ON Drivers.VehicleID = Vehicles.VehicleID
                            WHERE PlateNumbers = @PlateNumbers
                            ORDER BY Name ASC;";

            try
            {
                using (MySqlConnection conn = _connectionSettings.GetConnection())
                {
                    using (MySqlCommand cmd = _connectionSettings.GetCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("PlateNumbers", plateNumbers);

                        await conn.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                driversList.Add(new DriverDTO(
                                    Convert.ToInt32(reader["DriverID"]),
                                    (string)reader["NationalNo"],
                                    (string)reader["Name"],
                                    (string)reader["Phone"],
                                    (enDriverStatus)Enum.Parse(typeof(enDriverStatus), reader["Status"].ToString() ?? string.Empty),
                                    Convert.ToInt16(reader["VehicleID"])
                                ));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return driversList;
        }

        public async Task<List<DriverDTO>> GetDriversByStatusAsync(enDriverStatus status)
        {
            List<DriverDTO> driversList = new List<DriverDTO>();

            string query = @"SELECT * FROM Drivers
                            WHERE Status = @Status;";

            try
            {
                using (MySqlConnection conn = _connectionSettings.GetConnection())
                {
                    using (MySqlCommand cmd = _connectionSettings.GetCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("Status", status);

                        await conn.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                driversList.Add(new DriverDTO(
                                    Convert.ToInt32(reader["DriverID"]),
                                    (string)reader["NationalNo"],
                                    (string)reader["Name"],
                                    (string)reader["Phone"],
                                    (enDriverStatus)Enum.Parse(typeof(enDriverStatus), reader["Status"].ToString() ?? string.Empty),
                                    Convert.ToInt16(reader["VehicleID"])
                                ));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return driversList;
        }

        public async Task<DriverDTO> GetDriverByIDAsync(int driverID)
        {
            string query = @"SELECT * FROM Drivers
                            WHERE DriverID = @DriverID;";

            try
            {
                using (MySqlConnection conn = _connectionSettings.GetConnection())
                {
                    using (MySqlCommand cmd = _connectionSettings.GetCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("DriverID", driverID);

                        await conn.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new DriverDTO(
                                    Convert.ToInt32(reader["DriverID"]),
                                    (string)reader["NationalNo"],
                                    (string)reader["Name"],
                                    (string)reader["Phone"],
                                    (enDriverStatus)Enum.Parse(typeof(enDriverStatus), reader["Status"].ToString() ?? string.Empty),
                                    Convert.ToInt16(reader["VehicleID"])
                                );
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return null;
        }

        public async Task<DriverDTO> GetDriverByNationalNoAsync(string nationalNo)
        {
            string query = @"SELECT * FROM Drivers
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
                                return new DriverDTO(
                                    Convert.ToInt32(reader["DriverID"]),
                                    (string)reader["NationalNo"],
                                    (string)reader["Name"],
                                    (string)reader["Phone"],
                                    (enDriverStatus)Enum.Parse(typeof(enDriverStatus), reader["Status"].ToString() ?? string.Empty),
                                    Convert.ToInt16(reader["VehicleID"])
                                );
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return null;
        }

        public async Task<DriverDTO> GetDriverByPhoneAsync(string phone)
        {
            string query = @"SELECT * FROM Drivers
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
                                return new DriverDTO(
                                    Convert.ToInt32(reader["DriverID"]),
                                    (string)reader["NationalNo"],
                                    (string)reader["Name"],
                                    (string)reader["Phone"],
                                    (enDriverStatus)Enum.Parse(typeof(enDriverStatus), reader["Status"].ToString() ?? string.Empty),
                                    Convert.ToInt16(reader["VehicleID"])
                                );
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return null;
        }

        public async Task<int?> AddNewDriverAsync(DriverDTO newDriver)
        {
            string query = @"INSERT INTO Drivers
                            (NationalNo, Name, Phone, Status, VehicleID)
                            VALUES
                            (@NationalNo, @Name, @Phone, @Status, @VehicleID);
                            SELECT LAST_INSERT_ID();";

            try
            {
                using (MySqlConnection conn = _connectionSettings.GetConnection())
                {
                    using (MySqlCommand cmd = _connectionSettings.GetCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("NationalNo", newDriver.NationalNo);
                        cmd.Parameters.AddWithValue("Name", newDriver.Name);
                        cmd.Parameters.AddWithValue("Phone", newDriver.Phone);
                        cmd.Parameters.AddWithValue("Status", newDriver.Status.ToString());
                        cmd.Parameters.AddWithValue("VehicleID", newDriver.VehicleID);

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
                Console.WriteLine($"Error: {ex.Message}");
            }

            return null;
        }

        public async Task<bool> UpdateDriverAsync(DriverDTO updatedDriver)
        {
            string query = @"UPDATE Drivers SET
                            NationalNo = @NationalNo,
                            Name = @Name,
                            Phone = @Phone,
                            Status = @Status,
                            VehicleID = @VehicleID
                            WHERE DriverID = @DriverID;";

            try
            {
                using (MySqlConnection conn = _connectionSettings.GetConnection())
                {
                    using (MySqlCommand cmd = _connectionSettings.GetCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("DriverID", updatedDriver.DriverID ?? 0);
                        cmd.Parameters.AddWithValue("NationalNo", updatedDriver.NationalNo);
                        cmd.Parameters.AddWithValue("Name", updatedDriver.Name);
                        cmd.Parameters.AddWithValue("Phone", updatedDriver.Phone);
                        cmd.Parameters.AddWithValue("Status", updatedDriver.Status.ToString());
                        cmd.Parameters.AddWithValue("VehicleID", updatedDriver.VehicleID);

                        await conn.OpenAsync();

                        return Convert.ToByte(await cmd.ExecuteNonQueryAsync()) > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return false;
        }

        public async Task<bool> DeleteDriverAsync(string nationalNo)
        {
            string query = @"DELETE FROM Drivers
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
                Console.WriteLine($"Error: {ex.Message}");
            }

            return false;
        }

        public async Task<short> GetNumberOfDriversAsync()
        {
            List<DriverDTO> driversList = new List<DriverDTO>();

            string query = @"SELECT Count(*) FROM Drivers;";

            try
            {
                using (MySqlConnection conn = _connectionSettings.GetConnection())
                {
                    using (MySqlCommand cmd = _connectionSettings.GetCommand(query, conn))
                    {
                        await conn.OpenAsync();

                        return Convert.ToInt16(await cmd.ExecuteScalarAsync());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return 0;
        }

        public async Task<short> GetNumberOfDriversByStatusAsync(enDriverStatus status)
        {
            List<DriverDTO> driversList = new List<DriverDTO>();

            string query = @"SELECT Count(*) FROM Drivers
                            WHERE Status = @Status;";

            try
            {
                using (MySqlConnection conn = _connectionSettings.GetConnection())
                {
                    using (MySqlCommand cmd = _connectionSettings.GetCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("Status", status);

                        await conn.OpenAsync();

                        return Convert.ToInt16(await cmd.ExecuteScalarAsync());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return 0;
        }

        public async Task<bool> IsDriverExistsAsync(string nationalNo)
        {
            string query = @"SELECT Found=1 FROM Drivers
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
                Console.WriteLine($"Error: {ex.Message}");
            }

            return false;
        }

        public async Task<bool> IsDriverExistsAsync(int driverID)
        {
            string query = @"SELECT Found=1 FROM Drivers
                            WHERE DriverID = @DriverID;";

            try
            {
                using (MySqlConnection conn = _connectionSettings.GetConnection())
                {
                    using (MySqlCommand cmd = _connectionSettings.GetCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("DriverID", driverID);

                        await conn.OpenAsync();

                        return await cmd.ExecuteScalarAsync() != null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return false;
        }
    }
}
