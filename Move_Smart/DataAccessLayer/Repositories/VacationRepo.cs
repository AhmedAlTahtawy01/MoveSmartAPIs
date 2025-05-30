﻿using DataAccessLayer.Util;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class VacationDTO
    {
        public int? VacationID { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int VacationOwnerID { get; set; }
        public int SubstituteDriverID { get; set; }

        public VacationDTO(int? vacationID, DateOnly startDate, DateOnly endDate, int vacationOwnerID, int substituteDriverID)
        {
            VacationID = vacationID;
            StartDate = startDate;
            EndDate = endDate;
            VacationOwnerID = vacationOwnerID;
            SubstituteDriverID = substituteDriverID;
        }
    }

    public class VacationRepo
    {
        private readonly ConnectionSettings _connectionSettings;
        private readonly ILogger<VacationRepo> _logger;
        
        public VacationRepo(ConnectionSettings connectionSettings, ILogger<VacationRepo> logger)
        {
            _connectionSettings = connectionSettings ?? throw new ArgumentNullException(nameof(connectionSettings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<VacationDTO>> GetAllVacationsAsync()
        {
            List<VacationDTO> vacationsList = new List<VacationDTO>();

            string query = @"SELECT * FROM Vacations
                            ORDER BY StartDate ASC, EndDate DESC;";

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
                                vacationsList.Add(new VacationDTO(
                                    Convert.ToInt32(reader["VacationID"]),
                                    DateOnly.FromDateTime((DateTime)reader["StartDate"]),
                                    DateOnly.FromDateTime((DateTime)reader["EndDate"]),
                                    Convert.ToInt32(reader["VacationOwnerID"]),
                                    Convert.ToInt32(reader["SubstituteDriverID"])
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

            return vacationsList;
        }

        public async Task<List<VacationDTO>> GetAllVacationsForDriverAsync(int driverID)
        {
            List<VacationDTO> vacationsList = new List<VacationDTO>();

            string query = @"SELECT * FROM Vacations
                            WHERE VacationOwnerID = @DriverID
                            ORDER BY StartDate ASC, EndDate DESC;";

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
                            while (await reader.ReadAsync())
                            {
                                vacationsList.Add(new VacationDTO(
                                    Convert.ToInt32(reader["VacationID"]),
                                    DateOnly.FromDateTime((DateTime)reader["StartDate"]),
                                    DateOnly.FromDateTime((DateTime)reader["EndDate"]),
                                    Convert.ToInt32(reader["VacationOwnerID"]),
                                    Convert.ToInt32(reader["SubstituteDriverID"])
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

            return vacationsList;
        }

        public async Task<List<VacationDTO>> GetAllValidVacationsForDriverAsync(int driverID)
        {
            List<VacationDTO> vacationsList = new List<VacationDTO>();

            string query = @"SELECT * FROM Vacations
                            WHERE VacationOwnerID = @VacationOwnerID AND EndDate > CURDATE()
                            ORDER BY StartDate ASC, EndDate DESC;";

            try
            {
                using (MySqlConnection conn = _connectionSettings.GetConnection())
                {
                    using (MySqlCommand cmd = _connectionSettings.GetCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("VacationOwnerID", driverID);

                        await conn.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                vacationsList.Add(new VacationDTO(
                                    Convert.ToInt32(reader["VacationID"]),
                                    DateOnly.FromDateTime((DateTime)reader["StartDate"]),
                                    DateOnly.FromDateTime((DateTime)reader["EndDate"]),
                                    Convert.ToInt32(reader["VacationOwnerID"]),
                                    Convert.ToInt32(reader["SubstituteDriverID"])
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
            return vacationsList;
        }

        public async Task<VacationDTO> GetVacationByIDAsync(int vacationID)
        {
            string query = @"SELECT * FROM Vacations
                            WHERE VacationID = @VacationID;";

            try
            {
                using (MySqlConnection conn = _connectionSettings.GetConnection())
                {
                    using (MySqlCommand cmd = _connectionSettings.GetCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("VacationID", vacationID);

                        await conn.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new VacationDTO(
                                    Convert.ToInt32(reader["VacationID"]),
                                    DateOnly.FromDateTime((DateTime)reader["StartDate"]),
                                    DateOnly.FromDateTime((DateTime)reader["EndDate"]),
                                    Convert.ToInt32(reader["VacationOwnerID"]),
                                    Convert.ToInt32(reader["SubstituteDriverID"])
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

        public async Task<int?> AddNewVacationAsync(VacationDTO newVacation)
        {
            string query = @"INSERT INTO Vacations
                            (StartDate, EndDate, VacationOwnerID, SubstituteDriverID)
                            VALUES
                            (@StartDate, @EndDate, @VacationOwnerID, @SubstituteDriverID);
                            SELECT LAST_INSERT_ID();";

            try
            {
                using (MySqlConnection conn = _connectionSettings.GetConnection())
                {
                    using (MySqlCommand cmd = _connectionSettings.GetCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("StartDate", newVacation.StartDate.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("EndDate", newVacation.EndDate.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("VacationOwnerID", newVacation.VacationOwnerID);
                        cmd.Parameters.AddWithValue("SubstituteDriverID", newVacation.SubstituteDriverID);

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

        public async Task<bool> UpdateVacationAsync(VacationDTO updatedVacation)
        {
            string query = @"UPDATE Vacations SET
                            StartDate = @StartDate,
                            EndDate = @EndDate,
                            VacationOwnerID = @VacationOwnerID,
                            SubstituteDriverID = @SubstituteDriverID
                            WHERE VacationID = @VacationID;";

            try
            {
                using (MySqlConnection conn = _connectionSettings.GetConnection())
                {
                    using (MySqlCommand cmd = _connectionSettings.GetCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("VacationID", updatedVacation.VacationID ?? 0);
                        cmd.Parameters.AddWithValue("StartDate", updatedVacation.StartDate.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("EndDate", updatedVacation.EndDate.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("VacationOwnerID", updatedVacation.VacationOwnerID);
                        cmd.Parameters.AddWithValue("SubstituteDriverID", updatedVacation.SubstituteDriverID);

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

        public async Task<bool> DeleteVacationAsync(int vacationID)
        {
            string query = @"DELETE FROM Vacations
                            WHERE VacationID = @VacationID;";
            try
            {
                using (MySqlConnection conn = _connectionSettings.GetConnection())
                {
                    using (MySqlCommand cmd = _connectionSettings.GetCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("VacationID", vacationID);

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

        public async Task<bool> IsDriverinVacationAsync(int driverID)
        {
            string query = @"SELECT 1 As InVacation FROM Vacations
                            WHERE VacationOwnerID = @VacationOwnerID AND StartDate <= CURDATE() AND EndDate >= CURDATE();";

            try
            {
                using (MySqlConnection conn = _connectionSettings.GetConnection())
                {
                    using (MySqlCommand cmd = _connectionSettings.GetCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("VacationOwnerID", driverID);

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

        public async Task<bool> IsVacationExistsAsync(int vacationID)
        {
            string query = @"SELECT 1 AS Found FROM Vacations
                            WHERE VacationID = @VacationID;";

            try
            {
                using (MySqlConnection conn = _connectionSettings.GetConnection())
                {
                    using (MySqlCommand cmd = _connectionSettings.GetCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("VacationID", vacationID);

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
