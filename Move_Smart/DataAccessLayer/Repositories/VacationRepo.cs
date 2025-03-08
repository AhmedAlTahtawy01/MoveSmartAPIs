using DataAccessLayer.Util;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class VacationDTO
    {
        public int VacationID { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int VacationOwnerID { get; set; }
        public int SubstituteDriverID { get; set; }

        public VacationDTO(int vacationID, DateOnly startDate, DateOnly endDate, int vacationOwnerID, int substituteDriverID)
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
        public static async Task<List<VacationDTO>> GetAllVacationsAsync()
        {
            List<VacationDTO> vacationsList = new List<VacationDTO>();

            string query = @"SELECT * FROM Vacations
                            ORDER BY StartDate ASC, EndDate DESC;";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnectionSettings._connectionString))
                {
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        await conn.OpenAsync();

                        using (MySqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                vacationsList.Add(new VacationDTO(
                                    Convert.ToInt32(reader["VacationID"]),
                                    (DateOnly)reader["StartDate"],
                                    (DateOnly)reader["EndDate"],
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

        public static async Task<List<VacationDTO>> GetAllVacationsForDriverAsync(int driverID)
        {
            List<VacationDTO> vacationsList = new List<VacationDTO>();

            string query = @"SELECT * FROM Vacations
                            WHERE VacationOwnerID = @DriverID
                            ORDER BY StartDate ASC, EndDate DESC;";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnectionSettings._connectionString))
                {
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("DriverID", driverID);
                        await conn.OpenAsync();

                        using (MySqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                vacationsList.Add(new VacationDTO(
                                    Convert.ToInt32(reader["VacationID"]),
                                    (DateOnly)reader["StartDate"],
                                    (DateOnly)reader["EndDate"],
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

        public static async Task<List<VacationDTO>> GetAllActiveVacationsForDriverAsync(int driverID)
        {
            List<VacationDTO> vacationsList = new List<VacationDTO>();

            string query = @"SELECT * FROM Vacations
                            WHERE VacationOwnerID = @VacationOwnerID AND StartDate > CURDATE() AND EndDate > CURDATE()
                            ORDER BY StartDate ASC, EndDate DESC;";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnectionSettings._connectionString))
                {
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("VacationOwnerID", driverID);

                        await conn.OpenAsync();
                        
                        using (MySqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                vacationsList.Add(new VacationDTO(
                                    Convert.ToInt32(reader["VacationID"]),
                                    (DateOnly)reader["StartDate"],
                                    (DateOnly)reader["EndDate"],
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

        public static async Task<VacationDTO> GetVacationByIDAsync(int vacationID)
        {
            string query = @"SELECT * FROM Vacations
                            WHERE VacationID = @VacationID;";

            try
            {
                using(MySqlConnection conn = new MySqlConnection(ConnectionSettings._connectionString))
                {
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("VacationID", vacationID);

                        await conn.OpenAsync();

                        using (MySqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            if(await reader.ReadAsync())
                            {
                                return new VacationDTO(
                                    Convert.ToInt32(reader["VacationID"]),
                                    (DateOnly)reader["StartDate"],
                                    (DateOnly)reader["EndDate"],
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

        public static async Task<int?> AddNewVacationAsync(VacationDTO newVacation)
        {
            string query = @"INSERT INTO Vacations
                            (StartDate, EndDate, VacationOwnerID, SubstituteDriverID)
                            VALUES
                            (@StartDate, @EndDate, @VacationOwnerID, @SubstituteDriverID);
                            SELECT LAST_INSERT_ID();";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnectionSettings._connectionString))
                {
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("StartDate", newVacation.StartDate);
                        cmd.Parameters.AddWithValue("EndDate", newVacation.EndDate);
                        cmd.Parameters.AddWithValue("VacationOwnerID", newVacation.VacationOwnerID);
                        cmd.Parameters.AddWithValue("SubstituteDriverID", newVacation.SubstituteDriverID);

                        await conn.OpenAsync();

                        object? result = await cmd.ExecuteScalarAsync();
                        if(result != null && int.TryParse(result.ToString(), out int id))
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

        public static async Task<bool> UpdateVacationAsync(VacationDTO updatedVacation)
        {
            string query = @"UPDATE Vacations SET
                            StartDate = @StartDate,
                            EndDate = @EndDate,
                            VacationOwnerID = @VacationOwnerID,
                            SubstituteDriverID = @SubstituteDriverID
                            WHERE VacationID = @VacationID;";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnectionSettings._connectionString))
                {
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("VacationID", updatedVacation.VacationID);
                        cmd.Parameters.AddWithValue("StartDate", updatedVacation.StartDate);
                        cmd.Parameters.AddWithValue("EndDate", updatedVacation.EndDate);
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

        public static async Task<bool> DeleteVacationAsync(int vacationID)
        {
            string query = @"DELETE FROM Vacations
                            WHERE VacationID = @VacationID;";
            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnectionSettings._connectionString))
                {
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
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

        public static async Task<bool> IsDriverinVacationAsync(int driverID)
        {
            string query = @"SELECT InVacation = 1 FROM Vacations
                            WHERE VacationOwnerID = @VacationOwnerID AND StartDate <= CURDATE() AND EndDate >= CURDATE();";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnectionSettings._connectionString))
                {
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
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
    }
}
