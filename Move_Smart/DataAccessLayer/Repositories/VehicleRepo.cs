using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Util;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using static DataAccessLayer.VehicleDTO;

namespace DataAccessLayer
{
    public class VehicleDTO
    {
        public enum enVehicleStatus : byte
        {
            Available = 0,
            Working = 1,
            BrokenDown = 2
        }
        public enum enVehicleType : byte
        {
            Sedan = 0,
            SingleCab = 1,
            DoubleCab = 2,
            Truck = 3,
            Microbus = 4,
            Minibus = 5,
            Bus = 6,
            Ambulance = 7
        }

        public enum enFuelType : byte
        {
            Gasoline = 0,
            Diesel = 1,
            NaturalGas = 2
        }

        public short? VehicleID { get; set; }
        public string BrandName { get; set; }
        public string ModelName { get; set; }
        public string PlateNumbers { get; set; }
        public enVehicleType VehicleType { get; set; }
        public string AssociatedHospital { get; set; }
        public string AssociatedTask { get; set; }
        public enVehicleStatus Status { get; set; }
        public int TotalKilometersMoved { get; set; }
        public enFuelType FuelType { get; set; }
        public byte FuelConsumptionRate { get; set; }
        public byte OilConsumptionRate { get; set; }

        public VehicleDTO(short? vehicleID, string brandName, string modelName, string plateNumbers,
            enVehicleType vehicleType, string associatedHospital, string associatedTask,
            enVehicleStatus status, int totalKilometersMoved, enFuelType fuelType, byte fuelConsumptionRate,
            byte oilConsumptionRate)
        {
            VehicleID = vehicleID;
            BrandName = brandName;
            ModelName = modelName;
            PlateNumbers = plateNumbers;
            VehicleType = vehicleType;
            AssociatedHospital = associatedHospital;
            AssociatedTask = associatedTask;
            Status = status;
            TotalKilometersMoved = totalKilometersMoved;
            FuelType = fuelType;
            FuelConsumptionRate = fuelConsumptionRate;
            OilConsumptionRate = oilConsumptionRate;
        }
    }

    public class VehicleRepo
    {
        private readonly ConnectionSettings _connectionSettings;
        private readonly ILogger<VehicleRepo> _logger;

        public VehicleRepo(ConnectionSettings connectionSettings, ILogger<VehicleRepo> logger)
        {
            _connectionSettings = connectionSettings ?? throw new ArgumentNullException(nameof(connectionSettings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task<List<VehicleDTO>> GetAllVehiclesAsync()
        {
            List<VehicleDTO> vehiclesList = new List<VehicleDTO>();

            string query = @"SELECT * FROM Vehicles
                            ORDER BY Status ASC;";

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
                                vehiclesList.Add(new VehicleDTO
                                    (
                                        Convert.ToInt16(reader["VehicleID"]),
                                        (string)reader["BrandName"],
                                        (string)reader["ModelName"],
                                        (string)reader["PlateNumbers"],
                                        (enVehicleType)Enum.Parse(typeof(enVehicleType), reader["VehicleType"].ToString() ?? string.Empty),
                                        (string)reader["AssociatedHospital"],
                                        (string)reader["AssociatedTask"],
                                        (enVehicleStatus)Enum.Parse(typeof(enVehicleStatus), reader["Status"].ToString() ?? string.Empty),
                                        Convert.ToInt32(reader["TotalKilometersMoved"]),
                                        (enFuelType)Enum.Parse(typeof(enFuelType), reader["FuelType"].ToString() ?? string.Empty),
                                        Convert.ToByte(reader["FuelConsumptionRate"]),
                                        Convert.ToByte(reader["OilConsumptionRate"])
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

            return vehiclesList;
        }

        public async Task<List<VehicleDTO>> GetVehiclesByVehicleTypeAsync(enVehicleType vehicleType)
        {
            List<VehicleDTO> vehiclesList = new List<VehicleDTO>();

            string query = @"SELECT * FROM Vehicles
                            WHERE VehicleType = @VehicleType
                            ORDER BY Status ASC;";

            try
            {
                using (MySqlConnection conn = _connectionSettings.GetConnection())
                {
                    using (MySqlCommand cmd = _connectionSettings.GetCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("VehicleType", vehicleType.ToString());

                        await conn.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                vehiclesList.Add(new VehicleDTO
                                    (
                                        Convert.ToInt16(reader["VehicleID"]),
                                        (string)reader["BrandName"],
                                        (string)reader["ModelName"],
                                        (string)reader["PlateNumbers"],
                                        (enVehicleType)Enum.Parse(typeof(enVehicleType), reader["VehicleType"].ToString() ?? string.Empty),
                                        (string)reader["AssociatedHospital"],
                                        (string)reader["AssociatedTask"],
                                        (enVehicleStatus)Enum.Parse(typeof(enVehicleStatus), reader["Status"].ToString() ?? string.Empty),
                                        Convert.ToInt32(reader["TotalKilometersMoved"]),
                                        (enFuelType)Enum.Parse(typeof(enFuelType), reader["FuelType"].ToString() ?? string.Empty),
                                        Convert.ToByte(reader["FuelConsumptionRate"]),
                                        Convert.ToByte(reader["OilConsumptionRate"])
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

            return vehiclesList;
        }

        public async Task<List<VehicleDTO>> GetVehiclesByFuelTypeAsync(enFuelType FuelType)
        {
            List<VehicleDTO> vehiclesList = new List<VehicleDTO>();

            string query = @"SELECT * FROM Vehicles
                            WHERE FuelType = @FuelType
                            ORDER BY Status ASC;";

            try
            {
                using (MySqlConnection conn = _connectionSettings.GetConnection())
                {
                    using (MySqlCommand cmd = _connectionSettings.GetCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("FuelType", FuelType.ToString());

                        await conn.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                vehiclesList.Add(new VehicleDTO
                                    (
                                        Convert.ToInt16(reader["VehicleID"]),
                                        (string)reader["BrandName"],
                                        (string)reader["ModelName"],
                                        (string)reader["PlateNumbers"],
                                        (enVehicleType)Enum.Parse(typeof(enVehicleType), reader["VehicleType"].ToString() ?? string.Empty),
                                        (string)reader["AssociatedHospital"],
                                        (string)reader["AssociatedTask"],
                                        (enVehicleStatus)Enum.Parse(typeof(enVehicleStatus), reader["Status"].ToString() ?? string.Empty),
                                        Convert.ToInt32(reader["TotalKilometersMoved"]),
                                        (enFuelType)Enum.Parse(typeof(enFuelType), reader["FuelType"].ToString() ?? string.Empty),
                                        Convert.ToByte(reader["FuelConsumptionRate"]),
                                        Convert.ToByte(reader["OilConsumptionRate"])
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

            return vehiclesList;
        }

        public async Task<List<VehicleDTO>> GetVehiclesByStatusAsync(enVehicleStatus status)
        {
            List<VehicleDTO> vehiclesList = new List<VehicleDTO>();

            string query = @"SELECT * FROM Vehicles
                            WHERE Status = @Status
                            ORDER BY Status ASC;";

            try
            {
                using (MySqlConnection conn = _connectionSettings.GetConnection())
                {
                    using (MySqlCommand cmd = _connectionSettings.GetCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("Status", status.ToString());

                        await conn.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                vehiclesList.Add(new VehicleDTO
                                    (
                                        Convert.ToInt16(reader["VehicleID"]),
                                        (string)reader["BrandName"],
                                        (string)reader["ModelName"],
                                        (string)reader["PlateNumbers"],
                                        (enVehicleType)Enum.Parse(typeof(enVehicleType), reader["VehicleType"].ToString() ?? string.Empty),
                                        (string)reader["AssociatedHospital"],
                                        (string)reader["AssociatedTask"],
                                        (enVehicleStatus)Enum.Parse(typeof(enVehicleStatus), reader["Status"].ToString() ?? string.Empty),
                                        Convert.ToInt32(reader["TotalKilometersMoved"]),
                                        (enFuelType)Enum.Parse(typeof(enFuelType), reader["FuelType"].ToString() ?? string.Empty),
                                        Convert.ToByte(reader["FuelConsumptionRate"]),
                                        Convert.ToByte(reader["OilConsumptionRate"])
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

            return vehiclesList;
        }

        public async Task<short> GetNumberOfVehiclesAsync()
        {
            List<VehicleDTO> vehiclesList = new List<VehicleDTO>();

            string query = @"SELECT Count(*) FROM Vehicles;";

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

        public async Task<short> GetNumberOfVehiclesByStatusAsync(enVehicleStatus status)
        {
            List<VehicleDTO> vehiclesList = new List<VehicleDTO>();

            string query = @"SELECT Count(*) FROM Vehicles
                            WHERE Status = @Status;";

            try
            {
                using (MySqlConnection conn = _connectionSettings.GetConnection())
                {
                    using (MySqlCommand cmd = _connectionSettings.GetCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("Status", status.ToString());

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

        public async Task<VehicleDTO> GetVehicleByIDAsync(short vehicleID)
        {
            string query = @"SELECT * FROM Vehicles
                            WHERE VehicleID = @VehicleID";

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
                            if (await reader.ReadAsync())
                            {
                                return new VehicleDTO
                                    (
                                        Convert.ToInt16(reader["VehicleID"]),
                                        (string)reader["BrandName"],
                                        (string)reader["ModelName"],
                                        (string)reader["PlateNumbers"],
                                        (enVehicleType)Enum.Parse(typeof(enVehicleType), reader["VehicleType"].ToString() ?? string.Empty),
                                        (string)reader["AssociatedHospital"],
                                        (string)reader["AssociatedTask"],
                                        (enVehicleStatus)Enum.Parse(typeof(enVehicleStatus), reader["Status"].ToString() ?? string.Empty),
                                        Convert.ToInt32(reader["TotalKilometersMoved"]),
                                        (enFuelType)Enum.Parse(typeof(enFuelType), reader["FuelType"].ToString() ?? string.Empty),
                                        Convert.ToByte(reader["FuelConsumptionRate"]),
                                        Convert.ToByte(reader["OilConsumptionRate"])
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

        public async Task<bool> IsVehicleExistsAsync(short vehicleID)
        {
            string query = @"SELECT 1 AS Found FROM Vehicles
                            WHERE VehicleID = @VehicleID";

            try
            {
                using (MySqlConnection conn = _connectionSettings.GetConnection())
                {
                    using (MySqlCommand cmd = _connectionSettings.GetCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("VehicleID", vehicleID);

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

        public async Task<bool> IsVehicleExistsAsync(string plateNumbers)
        {
            string query = @"SELECT 1 AS Found FROM Vehicles
                            WHERE PlateNumbers = @PlateNumbers";

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

        public async Task<VehicleDTO> GetVehicleByPlateNumbersAsync(string plateNumbers)
        {
            string query = @"SELECT * FROM Vehicles
                            WHERE PlateNumbers = @PlateNumbers";

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
                                return new VehicleDTO
                                    (
                                        Convert.ToInt16(reader["VehicleID"]),
                                        (string)reader["BrandName"],
                                        (string)reader["ModelName"],
                                        (string)reader["PlateNumbers"],
                                        (enVehicleType)Enum.Parse(typeof(enVehicleType), reader["VehicleType"].ToString() ?? string.Empty),
                                        (string)reader["AssociatedHospital"],
                                        (string)reader["AssociatedTask"],
                                        (enVehicleStatus)Enum.Parse(typeof(enVehicleStatus), reader["Status"].ToString() ?? string.Empty),
                                        Convert.ToInt32(reader["TotalKilometersMoved"]),
                                        (enFuelType)Enum.Parse(typeof(enFuelType), reader["FuelType"].ToString() ?? string.Empty),
                                        Convert.ToByte(reader["FuelConsumptionRate"]),
                                        Convert.ToByte(reader["OilConsumptionRate"])
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

        public async Task<short?> AddNewVehicleAsync(VehicleDTO newVehicle)
        {
            string query = @"INSERT INTO Vehicles
                            (BrandName, ModelName, PlateNumbers, VehicleType, AssociatedHospital, AssociatedTask, Status, TotalKilometersMoved, FuelType, FuelConsumptionRate, OilConsumptionRate)
                            VALUES
                            (@BrandName, @ModelName, @PlateNumbers, @VehicleType, @AssociatedHospital, @AssociatedTask, @Status, @TotalKilometersMoved, @FuelType, @FuelConsumptionRate, @OilConsumptionRate);
                            SELECT LAST_INSERT_ID();";

            try
            {
                using (MySqlConnection conn = _connectionSettings.GetConnection())
                {
                    using (MySqlCommand cmd = _connectionSettings.GetCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("BrandName", newVehicle.BrandName);
                        cmd.Parameters.AddWithValue("ModelName", newVehicle.ModelName);
                        cmd.Parameters.AddWithValue("PlateNumbers", newVehicle.PlateNumbers);
                        cmd.Parameters.AddWithValue("VehicleType", newVehicle.VehicleType.ToString());
                        cmd.Parameters.AddWithValue("AssociatedHospital", newVehicle.AssociatedHospital);
                        cmd.Parameters.AddWithValue("AssociatedTask", newVehicle.AssociatedTask);
                        cmd.Parameters.AddWithValue("Status", newVehicle.Status.ToString());
                        cmd.Parameters.AddWithValue("TotalKilometersMoved", newVehicle.TotalKilometersMoved);
                        cmd.Parameters.AddWithValue("FuelType", newVehicle.FuelType.ToString());
                        cmd.Parameters.AddWithValue("FuelConsumptionRate", newVehicle.FuelConsumptionRate);
                        cmd.Parameters.AddWithValue("OilConsumptionRate", newVehicle.OilConsumptionRate);

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
                _logger.LogError(ex, $"Error: {ex.Message}");
            }

            return null;
        }

        public async Task<bool> UpdateVehicleAsync(VehicleDTO updatedVehicle)
        {
            string query = @"UPDATE Vehicles SET
                            BrandName = @BrandName,
                            ModelName = @ModelName,
                            PlateNumbers = @PlateNumbers,
                            VehicleType = @VehicleType,
                            AssociatedHospital = @AssociatedHospital,
                            AssociatedTask = @AssociatedTask,
                            Status = @Status,
                            TotalKilometersMoved = @TotalKilometersMoved,
                            FuelType = @FuelType,
                            FuelConsumptionRate = @FuelConsumptionRate,
                            OilConsumptionRate = @OilConsumptionRate
                            WHERE VehicleID = @VehicleID;";

            try
            {
                using (MySqlConnection conn = _connectionSettings.GetConnection())
                {
                    using (MySqlCommand cmd = _connectionSettings.GetCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("VehicleID", updatedVehicle.VehicleID ?? 0);
                        cmd.Parameters.AddWithValue("BrandName", updatedVehicle.BrandName);
                        cmd.Parameters.AddWithValue("ModelName", updatedVehicle.ModelName);
                        cmd.Parameters.AddWithValue("PlateNumbers", updatedVehicle.PlateNumbers);
                        cmd.Parameters.AddWithValue("VehicleType", updatedVehicle.VehicleType.ToString());
                        cmd.Parameters.AddWithValue("AssociatedHospital", updatedVehicle.AssociatedHospital);
                        cmd.Parameters.AddWithValue("AssociatedTask", updatedVehicle.AssociatedTask);
                        cmd.Parameters.AddWithValue("Status", updatedVehicle.Status.ToString());
                        cmd.Parameters.AddWithValue("TotalKilometersMoved", updatedVehicle.TotalKilometersMoved);
                        cmd.Parameters.AddWithValue("FuelType", updatedVehicle.FuelType.ToString());
                        cmd.Parameters.AddWithValue("FuelConsumptionRate", updatedVehicle.FuelConsumptionRate);
                        cmd.Parameters.AddWithValue("OilConsumptionRate", updatedVehicle.OilConsumptionRate);

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

        public async Task<bool> DeleteVehicleAsync(string plateNumbers)
        {
            string query = @"DELETE FROM Vehicles
                            WHERE PlateNumbers = @PlateNumbers;";
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

        public async Task<bool> DeleteVehicleAsync(short vehicleID)
        {
            string query = @"DELETE FROM Vehicles
                            WHERE VehicleID = @VehicleID;";

            try
            {
                using (MySqlConnection conn = _connectionSettings.GetConnection())
                {
                    using (MySqlCommand cmd = _connectionSettings.GetCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("VehicleID", vehicleID);
            
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
    }
}
