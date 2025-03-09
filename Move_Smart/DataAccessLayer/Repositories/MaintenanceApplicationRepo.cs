using DataAccessLayer.Util;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class MaintenanceApplicationDTO
    {
        public int? MaintenanceApplicationID { get; set; }
        public int ApplicationID { get; set; }
        public short VehicleID { get; set; }
        public bool ApprovedByGeneralSupervisor { get; set; }
        public bool ApprovedByGeneralManager { get; set; }

        public MaintenanceApplicationDTO(int? maintenanceApplicationID, int applicationID,
            short vehicleID, bool approvedByGeneralSupervisor, bool approvedByGeneralManager)
        {
            MaintenanceApplicationID = maintenanceApplicationID;
            ApplicationID = applicationID;
            VehicleID = vehicleID;
            ApprovedByGeneralSupervisor = approvedByGeneralSupervisor;
            ApprovedByGeneralManager = approvedByGeneralManager;
        }
    }

    public class MaintenanceApplicationRepo
    {
        public static async Task<List<MaintenanceApplicationDTO>> GetAllMaintenanceApplicationsAsync()
        {
            List<MaintenanceApplicationDTO> applicationsList = new List<MaintenanceApplicationDTO>();

            string query = @"SELECT * FROM MaintenanceApplications
                            ORDER BY ApprovedByGeneralSupervisor DESC, ApprovedByGeneralManager DESC;";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnectionSettings.ConnectionString))
                {
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        await conn.OpenAsync();

                        using (MySqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                applicationsList.Add(new MaintenanceApplicationDTO(
                                    Convert.ToInt32(reader["MaintenanceApplicationID"]),
                                    Convert.ToInt32(reader["ApplicationID"]),
                                    Convert.ToInt16(reader["VehicleID"]),
                                    Convert.ToBoolean(reader["ApprovedByGeneralSupervisor"]),
                                    Convert.ToBoolean(reader["ApprovedByGeneralManager"])
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

            return applicationsList;
        }

        public static async Task<List<MaintenanceApplicationDTO>> GetAllMaintenanceApplicationsForVehicleAsync(short vehicleID)
        {
            List<MaintenanceApplicationDTO> applicationsList = new List<MaintenanceApplicationDTO>();

            string query = @"SELECT * FROM MaintenanceApplications
                            WHERE VehicleID = @VehicleID
                            ORDER BY ApprovedByGeneralSupervisor DESC, ApprovedByGeneralManager DESC;";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnectionSettings.ConnectionString))
                {
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("VehicleID", vehicleID);

                        await conn.OpenAsync();

                        using (MySqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                applicationsList.Add(new MaintenanceApplicationDTO(
                                    Convert.ToInt32(reader["MaintenanceApplicationID"]),
                                    Convert.ToInt32(reader["ApplicationID"]),
                                    Convert.ToInt16(reader["VehicleID"]),
                                    Convert.ToBoolean(reader["ApprovedByGeneralSupervisor"]),
                                    Convert.ToBoolean(reader["ApprovedByGeneralManager"])
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

            return applicationsList;
        }

        public static async Task<MaintenanceApplicationDTO> GetMaintenanceApplicationByMaintenanceApplicationIDAsync(int maintenanceApplicationID)
        {
            string query = @"SELECT * FROM MaintenanceApplications
                            WHERE MaintenanceApplicationID = @MaintenanceApplicationID;";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnectionSettings.ConnectionString))
                {
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("MaintenanceApplicationID", maintenanceApplicationID);

                        await conn.OpenAsync();
                        
                        using (MySqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new MaintenanceApplicationDTO(
                                    Convert.ToInt32(reader["MaintenanceApplicationID"]),
                                    Convert.ToInt32(reader["ApplicationID"]),
                                    Convert.ToInt16(reader["VehicleID"]),
                                    Convert.ToBoolean(reader["ApprovedByGeneralSupervisor"]),
                                    Convert.ToBoolean(reader["ApprovedByGeneralManager"])
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

        public static async Task<int?> AddNewMaintenanceApplicationAsync(MaintenanceApplicationDTO newMaintenanceApplication)
        {
            string query = @"INSERT INTO MaintenanceApplications
                            (ApplicationID, VehicleID, ApprovedByGeneralSupervisor, ApprovedByGeneralManager)
                            VALUES
                            (@ApplicationID, @VehicleID, @ApprovedByGeneralSupervisor, @ApprovedByGeneralManager);
                            SELECT LAST_INSERT_ID();";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnectionSettings.ConnectionString))
                {
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("ApplicationID", newMaintenanceApplication.ApplicationID);
                        cmd.Parameters.AddWithValue("VehicleID", newMaintenanceApplication.VehicleID);
                        cmd.Parameters.AddWithValue("ApprovedByGeneralSupervisor", newMaintenanceApplication.ApprovedByGeneralSupervisor);
                        cmd.Parameters.AddWithValue("ApprovedByGeneralManager", newMaintenanceApplication.ApprovedByGeneralManager);

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

        public static async Task<bool> UpdateMaintenanceApplicationAsync(MaintenanceApplicationDTO updatedMaintenanceApplication)
        {
            string query = @"UPDATE MaintenanceApplications SET
                            ApplicationID = @ApplicationID,
                            VehicleID = @VehicleID,
                            ApprovedByGeneralSupervisor = @ApprovedByGeneralSupervisor,
                            ApprovedByGeneralManager = @ApprovedByGeneralManager
                            WHERE MaintenanceApplicationID = @MaintenanceApplicationID;";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnectionSettings.ConnectionString))
                {
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("MaintenanceApplicationID", updatedMaintenanceApplication.MaintenanceApplicationID ?? 0);
                        cmd.Parameters.AddWithValue("ApplicationID", updatedMaintenanceApplication.ApplicationID);
                        cmd.Parameters.AddWithValue("VehicleID", updatedMaintenanceApplication.VehicleID);
                        cmd.Parameters.AddWithValue("ApprovedByGeneralSupervisor", updatedMaintenanceApplication.ApprovedByGeneralSupervisor);
                        cmd.Parameters.AddWithValue("ApprovedByGeneralManager", updatedMaintenanceApplication.ApprovedByGeneralManager);

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

        public static async Task<bool> DeleteMaintenanceApplicationAsync(int maintenanceApplicationID)
        {
            string query = @"DELETE FROM MaintenanceApplications
                            WHERE MaintenanceApplicationID = @MaintenanceApplicationID;";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnectionSettings.ConnectionString))
                {
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("MaintenanceApplicationID", maintenanceApplicationID);
                        
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
