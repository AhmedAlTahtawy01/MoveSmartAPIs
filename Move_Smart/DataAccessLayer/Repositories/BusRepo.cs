using DataAccessLayer.Repositories;
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

namespace DataAccessLayer
{
    public class BusDTO
    {
        public byte? BusID { get; set; }
        public byte Capacity { get; set; }
        public byte AvailableSpace { get; set; }
        public short VehicleID { get; set; }
        public VehicleDTO Vehicle { get; set; }

        public BusDTO(byte? busID, byte capacity, byte availableSpace, short vehicleID, VehicleDTO vehicle)
        {
            BusID = busID;
            Capacity = capacity;
            AvailableSpace = availableSpace;
            VehicleID = vehicleID;
            Vehicle = vehicle;
        }
    }

    public class BusRepo
    {
        private readonly ConnectionSettings _connectionSettings;
        private readonly ILogger<BusRepo> _logger;
        private readonly VehicleRepo _vehicleRepo;

        public BusRepo(ConnectionSettings connectionSettings, VehicleRepo vehicleRepo, ILogger<BusRepo> logger)
        {
            _connectionSettings = connectionSettings ?? throw new ArgumentNullException(nameof(connectionSettings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _vehicleRepo = vehicleRepo ?? throw new ArgumentNullException(nameof(vehicleRepo));
        }

        public async Task<List<BusDTO>> GetAllBusesAsync()
        {
            List<BusDTO> busesList = new List<BusDTO>();

            string query = @"SELECT * FROM Buses
                            ORDER BY Capacity DESC;";

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
                                busesList.Add(new BusDTO(
                                    Convert.ToByte(reader["BusID"]),
                                    Convert.ToByte(reader["Capacity"]),
                                    Convert.ToByte(reader["AvailableSpace"]),
                                    Convert.ToInt16(reader["VehicleID"]),
                                    await _vehicleRepo.GetVehicleByIDAsync(Convert.ToInt16(reader["VehicleID"]))
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

            return busesList;
        }

        public async Task<BusDTO> GetBusByIDAsync(byte busID)
        {
            string query = @"SELECT * FROM Buses 
                            WHERE BusID = @BusID";

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
                            if (await reader.ReadAsync())
                            {
                                return new BusDTO(
                                    Convert.ToByte(reader["BusID"]),
                                    Convert.ToByte(reader["Capacity"]),
                                    Convert.ToByte(reader["AvailableSpace"]),
                                    Convert.ToInt16(reader["VehicleID"]),
                                    await _vehicleRepo.GetVehicleByIDAsync(Convert.ToInt16(reader["VehicleID"]))
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

        public async Task<BusDTO> GetBusByPlateNumbersAsync(string plateNumbers)
        {
            string query = @"SELECT * FROM Buses 
                            WHERE VehicleID = (SELECT VehicleID FROM Vehicles WHERE PlateNumbers = @PlateNumbers)";

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
                            if (await reader.ReadAsync())
                            {
                                return new BusDTO(
                                    Convert.ToByte(reader["BusID"]),
                                    Convert.ToByte(reader["Capacity"]),
                                    Convert.ToByte(reader["AvailableSpace"]),
                                    Convert.ToInt16(reader["VehicleID"]),
                                    await _vehicleRepo.GetVehicleByIDAsync(Convert.ToInt16(reader["VehicleID"]))
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

        public async Task<List<BusDTO>> GetBusesByCapacityAsync(byte capacity)
        {
            List<BusDTO> busesList = new List<BusDTO>();

            string query = @"SELECT * FROM Buses 
                            WHERE Capacity >= @Capacity";

            try
            {
                using (MySqlConnection conn = _connectionSettings.GetConnection())
                {
                    using (MySqlCommand cmd = _connectionSettings.GetCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("Capacity", capacity);

                        await conn.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                busesList.Add(new BusDTO(
                                    Convert.ToByte(reader["BusID"]),
                                    Convert.ToByte(reader["Capacity"]),
                                    Convert.ToByte(reader["AvailableSpace"]),
                                    Convert.ToInt16(reader["VehicleID"]),
                                    await _vehicleRepo.GetVehicleByIDAsync(Convert.ToInt16(reader["VehicleID"]))
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

            return busesList;
        }

        public async Task<List<BusDTO>> GetBusesByAvailableSpaceAsync(byte availableSpace)
        {
            List<BusDTO> busesList = new List<BusDTO>();

            string query = @"SELECT * FROM Buses 
                            WHERE AvailableSpace >= @AvailableSpace";

            try
            {
                using (MySqlConnection conn = _connectionSettings.GetConnection())
                {
                    using (MySqlCommand cmd = _connectionSettings.GetCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("AvailableSpace", availableSpace);

                        await conn.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                busesList.Add(new BusDTO(
                                    Convert.ToByte(reader["BusID"]),
                                    Convert.ToByte(reader["Capacity"]),
                                    Convert.ToByte(reader["AvailableSpace"]),
                                    Convert.ToInt16(reader["VehicleID"]),
                                    await _vehicleRepo.GetVehicleByIDAsync(Convert.ToInt16(reader["VehicleID"]))
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

            return busesList;
        }

        public async Task<byte?> AddNewBusAsync(BusDTO newBus)
        {
            string query = @"INSERT INTO Buses
                            (Capacity, AvailableSpace, VehicleID)
                            VALUES
                            (@Capacity, @AvailableSpace, @VehicleID);
                            SELECT LAST_INSERT_ID();";
            try
            {
                using (MySqlConnection conn = _connectionSettings.GetConnection())
                {
                    using (MySqlCommand cmd = _connectionSettings.GetCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("Capacity", newBus.Capacity);
                        cmd.Parameters.AddWithValue("AvailableSpace", newBus.AvailableSpace);
                        cmd.Parameters.AddWithValue("VehicleID", newBus.VehicleID);

                        await conn.OpenAsync();

                        object? result = await cmd.ExecuteScalarAsync();
                        if (result != null && byte.TryParse(result.ToString(), out byte id))
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

        public async Task<bool> UpdateBusAsync(BusDTO updatedBus)
        {
            string query = @"UPDATE Buses SET
                            Capacity = @Capacity,
                            AvailableSpace = @AvailableSpace,
                            VehicleID = @VehicleID
                            WHERE BusID = @BusID";

            try
            {
                using (MySqlConnection conn = _connectionSettings.GetConnection())
                {
                    using (MySqlCommand cmd = _connectionSettings.GetCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("BusID", updatedBus.BusID);
                        cmd.Parameters.AddWithValue("Capacity", updatedBus.Capacity);
                        cmd.Parameters.AddWithValue("AvailableSpace", updatedBus.AvailableSpace);
                        cmd.Parameters.AddWithValue("VehicleID", updatedBus.VehicleID);

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

        public async Task<bool> DeleteBusAsync(byte busID)
        {
            string query = @"DELETE FROM Buses
                            WHERE BusID = @BusID";

            try
            {
                using (MySqlConnection conn = _connectionSettings.GetConnection())
                {
                    using (MySqlCommand cmd = _connectionSettings.GetCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("BusID", busID);

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

        public async Task<bool> DeleteBusAsync(string plateNumbers)
        {
            string query = @"DELETE FROM Buses
                            WHERE VehicleID = (SELECT VehicleID FROM Vehicles WHERE PlateNumbers = @PlateNumbers)";
            try
            {
                using (MySqlConnection conn = _connectionSettings.GetConnection())
                {
                    using (MySqlCommand cmd = _connectionSettings.GetCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("PlateNumbers", plateNumbers);
                        
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

        public async Task<bool> IsBusExistsAsync(byte busID)
        {
            string query = @"SELECT 1 AS Found FROM Buses
                            WHERE BusID = @BusID";

            try
            {
                using (MySqlConnection conn = _connectionSettings.GetConnection())
                {
                    using (MySqlCommand cmd = _connectionSettings.GetCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("BusID", busID);

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

        public async Task<bool> IsBusExistsAsync(string plateNumbers)
        {
            string query = @"SELECT 1 AS Found FROM Buses
                            WHERE VehicleID = (SELECT VehicleID FROM Vehicles WHERE PlateNumbers = @PlateNumbers)";

            try
            {
                using (MySqlConnection conn = _connectionSettings.GetConnection())
                {
                    using (MySqlCommand cmd = _connectionSettings.GetCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("PlateNumbers", plateNumbers);
                        
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