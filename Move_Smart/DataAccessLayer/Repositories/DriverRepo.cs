using DataAccessLayer.Util;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class DriverDTO
    {
        public int DriverID { get; set; }
        public string NationalNo { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public bool IsAbsent { get; set; }
        public bool IsAvailable { get; set; }
        public short VehicleID { get; set; }

        public DriverDTO(int driverID, string nationalNo, string name, string phone, bool isAbsent,
            bool isAvailable, short vehicleID)
        {
            DriverID = driverID;
            NationalNo = nationalNo;
            Name = name;
            Phone = phone;
            IsAbsent = isAbsent;
            IsAvailable = isAvailable;
            VehicleID = vehicleID;
        }
    }

    public class DriverRepo
    {
        public static async Task<List<DriverDTO>> GetAllDriversAsync()
        {
            List<DriverDTO> driversList = new List<DriverDTO>();

            string query = @"SELECT * FROM Drivers
                            ORDER BY Name ASC;";

            try
            {
                using(MySqlConnection conn = new MySqlConnection(ConnectionSettings._connectionString))
                {
                    using(MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        await conn.OpenAsync();

                        using(MySqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while(await reader.ReadAsync())
                            {
                                driversList.Add(new DriverDTO(
                                    Convert.ToInt32(reader["DriverID"]),
                                    (string)reader["NationalNo"],
                                    (string)reader["Name"]  ,
                                    (string)reader["Phone"],
                                    Convert.ToBoolean(reader["IsAbsent"]),
                                    Convert.ToBoolean(reader["IsAvailable"]),
                                    Convert.ToInt16(reader["VehicleID"])
                                ));
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return driversList;
        }

        public static async Task<List<DriverDTO>> GetDriversByVehicleIDAsync(short vehicleID)
        {
            List<DriverDTO> driversList = new List<DriverDTO>();

            string query = @"SELECT * FROM Drivers
                            WHERE VehicleID = @VehicleID
                            ORDER BY Name ASC;";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnectionSettings._connectionString))
                {
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("VehicleID", vehicleID);

                        await conn.OpenAsync();

                        using (MySqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while(await reader.ReadAsync())
                            {
                                driversList.Add(new DriverDTO(
                                    Convert.ToInt32(reader["DriverID"]),
                                    (string)reader["NationalNo"],
                                    (string)reader["Name"],
                                    (string)reader["Phone"],
                                    Convert.ToBoolean(reader["IsAbsent"]),
                                    Convert.ToBoolean(reader["IsAvailable"]),
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

        public static async Task<List<DriverDTO>> GetDriversByVehiclePlateNumbersAsync(string plateNumbers)
        {
            List<DriverDTO> driversList = new List<DriverDTO>();

            string query = @"SELECT Drivers.* FROM
                            Drivers JOIN Vehicles ON Drivers.VehicleID = Vehicles.VehicleID
                            WHERE PlateNumbers = @PlateNumbers
                            ORDER BY Name ASC;";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnectionSettings._connectionString))
                {
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("PlateNumbers", plateNumbers);
                        
                        await conn.OpenAsync();

                        using (MySqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while(await reader.ReadAsync())
                            {
                                driversList.Add(new DriverDTO(
                                    Convert.ToInt32(reader["DriverID"]),
                                    (string)reader["NationalNo"],
                                    (string)reader["Name"],
                                    (string)reader["Phone"],
                                    Convert.ToBoolean(reader["IsAbsent"]),
                                    Convert.ToBoolean(reader["IsAvailable"]),
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

        public static async Task<DriverDTO> GetDriverByIDAsync(int driverID)
        {
            string query = @"SELECT * FROM Drivers
                            WHERE DriverID = @DriverID;";

            try
            {
                using(MySqlConnection conn = new MySqlConnection(ConnectionSettings._connectionString))
                {
                    using(MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("DriverID", driverID);

                        await conn.OpenAsync();

                        using(MySqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            if(await reader.ReadAsync())
                            {
                                return new DriverDTO(
                                    Convert.ToInt32(reader["DriverID"]),
                                    (string)reader["NationalNo"],
                                    (string)reader["Name"],
                                    (string)reader["Phone"],
                                    Convert.ToBoolean(reader["IsAbsent"]),
                                    Convert.ToBoolean(reader["IsAvailable"]),
                                    Convert.ToInt16(reader["VehicleID"])
                                );
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return null;
        }

        public static async Task<DriverDTO> GetDriverByNationalNoAsync(string nationalNo)
        {
            string query = @"SELECT * FROM Drivers
                            WHERE NationalNo = @NationalNo;";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnectionSettings._connectionString))
                {
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("NationalNo", nationalNo);

                        await conn.OpenAsync();

                        using (MySqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new DriverDTO(
                                    Convert.ToInt32(reader["DriverID"]),
                                    (string)reader["NationalNo"],
                                    (string)reader["Name"],
                                    (string)reader["Phone"],
                                    Convert.ToBoolean(reader["IsAbsent"]),
                                    Convert.ToBoolean(reader["IsAvailable"]),
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

        public static async Task<DriverDTO> GetDriverByPhoneAsync(string phone)
        {
            string query = @"SELECT * FROM Drivers
                            WHERE Phone = @Phone;";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnectionSettings._connectionString))
                {
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("Phone", phone);

                        await conn.OpenAsync();

                        using (MySqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new DriverDTO(
                                    Convert.ToInt32(reader["DriverID"]),
                                    (string)reader["NationalNo"],
                                    (string)reader["Name"],
                                    (string)reader["Phone"],
                                    Convert.ToBoolean(reader["IsAbsent"]),
                                    Convert.ToBoolean(reader["IsAvailable"]),
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

        public static async Task<int?> AddNewDriverAsync(DriverDTO newDriver)
        {
            string query = @"INSERT INTO Drivers
                            (NationalNo, Name, Phone, IsAbsent, IsAvailable, VehicleID)
                            VALUES
                            (@NationalNo, @Name, @Phone, @IsAbsent, @IsAvailable, @VehicleID);
                            SELECT LAST_INSERT_ID();";

            try
            {
                using(MySqlConnection conn = new MySqlConnection(ConnectionSettings._connectionString))
                {
                    using(MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("NationalNo", newDriver.NationalNo);
                        cmd.Parameters.AddWithValue("Name", newDriver.Name);
                        cmd.Parameters.AddWithValue("Phone", newDriver.Phone);
                        cmd.Parameters.AddWithValue("IsAbsent", newDriver.IsAbsent);
                        cmd.Parameters.AddWithValue("IsAvailable", newDriver.IsAvailable);
                        cmd.Parameters.AddWithValue("VehicleID", newDriver.VehicleID);

                        await conn.OpenAsync();

                        object? result = await cmd.ExecuteScalarAsync();
                        if(result != null && int.TryParse(result.ToString(), out int id))
                        {
                            return id;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return null;
        }

        public static async Task<bool> UpdateDriverAsync(DriverDTO updatedDriver)
        {
            string query = @"UPDATE Drivers SET
                            NationalNo = @NationalNo,
                            Name = @Name,
                            Phone = @Phone,
                            IsAbsent = @IsAbsent,
                            IsAvailable = @IsAvailable,
                            VehicleID = @VehicleID
                            WHERE DriverID = @DriverID;";

            try
            {
                using(MySqlConnection conn = new MySqlConnection(ConnectionSettings._connectionString))
                {
                    using(MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("DriverID", updatedDriver.DriverID);
                        cmd.Parameters.AddWithValue("NationalNo", updatedDriver.NationalNo);
                        cmd.Parameters.AddWithValue("Name", updatedDriver.Name);
                        cmd.Parameters.AddWithValue("Phone", updatedDriver.Phone);
                        cmd.Parameters.AddWithValue("IsAbsent", updatedDriver.IsAbsent);
                        cmd.Parameters.AddWithValue("IsAvailable", updatedDriver.IsAvailable);
                        cmd.Parameters.AddWithValue("VehicleID", updatedDriver.VehicleID);

                        await conn.OpenAsync();

                        return Convert.ToByte(await cmd.ExecuteNonQueryAsync()) > 0;
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return false;
        }

        public static async Task<bool> DeleteDriverAsync(string nationalNo)
        {
            string query = @"DELETE FROM Drivers
                            WHERE NationalNo = @NationalNo;";

            try
            {
                using(MySqlConnection conn = new MySqlConnection(ConnectionSettings._connectionString))
                {
                    using(MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("NationalNo", nationalNo);

                        await conn.OpenAsync();

                        return Convert.ToByte(await cmd.ExecuteNonQueryAsync()) > 0;
                    }    
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return false;
        }
    }
}
