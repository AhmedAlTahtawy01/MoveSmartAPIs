using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class Maintenance
    {
        public int MaintenanceId { get; set; }
        public DateTime MaintenanceDate { get; set; }
        public string Description { get; set; }
        public int MaintenanceApplicationId { get; set; }

        public Maintenance(int maintenanceId, DateTime date, string description, int maintenanceApplicationId)
        {
            this.MaintenanceId = maintenanceId;
            this.MaintenanceDate = date;
            this.Description = description;
            this.MaintenanceApplicationId = maintenanceApplicationId;
        }
    }

    public class MaintenanceRepo
    {
        private static readonly string _connectionString = "Server=localhost;Database=move_smart;User Id=root;Password=ahmedroot;";

        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }

        private MySqlCommand GetCommand(string query, MySqlConnection conn)
        {
            var cmd = new MySqlCommand(query, conn);
            cmd.CommandType = CommandType.Text;
            return cmd;
        }

        private Maintenance MapMaintenance(DbDataReader reader)
        {
            return new Maintenance
            (
                reader.GetInt32("MaintenanceID"),
                reader.GetDateTime("MaintenanceDate"),
                reader.GetString("Description"),
                reader.GetInt32("MaintenanceApplicationID")
            );
        }

        public async Task<List<Maintenance>> GetAllMaintenancesAsync()
        {
            var maintenances = new List<Maintenance>();

            await using (var conn = GetConnection())
            {
                var query = @"
                    SELECT MaintenanceID, MaintenanceDate, Description, MaintenanceApplicationID
                    FROM maintenance";

                using (var cmd = GetCommand(query, conn))
                {
                    try
                    {
                        await conn.OpenAsync().ConfigureAwait(false);

                        using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            while (await reader.ReadAsync())
                            {
                                maintenances.Add(MapMaintenance(reader));
                            }
                        }
                    }
                    catch (MySqlException ex)
                    {
                        throw new Exception("Database error occurred in GetAllMaintenancesAsync.", ex);
                    }
                }

                return maintenances;
            }
        }

        public async Task<Maintenance> GetMaintenanceByIdAsync(int maintenanceId)
        {
            await using (var conn = GetConnection())
            {
                var query = @"
                    SELECT MaintenanceID, MaintenanceDate, Description, MaintenanceApplicationID
                    FROM maintenance
                    WHERE MaintenanceID = @maintenanceId;";

                using (var cmd = GetCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@maintenanceId", maintenanceId);

                    try
                    {
                        await conn.OpenAsync().ConfigureAwait(false);
                        using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            if (await reader.ReadAsync())
                            {
                                return MapMaintenance(reader);
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                    catch (MySqlException ex)
                    {
                        throw new Exception("Database error occurred in GetMaintenanceByIdAsync.", ex);
                    }
                }
            }
        }

        public async Task<List<Maintenance>> GetMaintenancesByDateAsync(DateTime maintenanceDate)
        {
            var maintenances = new List<Maintenance>();

            await using (var conn = GetConnection())
            {
                var query = @"
                    SELECT MaintenanceID, MaintenanceDate, Description, MaintenanceApplicationID
                    FROM maintenance
                    WHERE DATE(MaintenanceDate) = @maintenanceDate";

                using (var cmd = GetCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@maintenanceDate", maintenanceDate.Date);

                    try
                    {
                        await conn.OpenAsync().ConfigureAwait(false);
                        using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            while (await reader.ReadAsync())
                            {
                                maintenances.Add(MapMaintenance(reader));
                            }
                        }
                    }
                    catch (MySqlException ex)
                    {
                        throw new Exception("Database error occurred in GetMaintenancesByDateAsync.", ex);
                    }
                }
            }

            return maintenances;
        }

        public async Task<List<Maintenance>> GetMaintenanceByMaintenanceApplicationIdAsync(int maintenanceApplicationId)
        {
            var maintenances = new List<Maintenance>();

            await using (var conn = GetConnection())
            {
                var query = @"
                    SELECT MaintenanceID, MaintenanceDate, Description, MaintenanceApplicationID
                    FROM maintenance
                    WHERE MaintenanceApplicationID = @maintenanceApplicationId";

                using (var cmd = GetCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@maintenanceApplicationId", maintenanceApplicationId);

                    try
                    {
                        await conn.OpenAsync().ConfigureAwait(false);
                        using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            while (await reader.ReadAsync())
                            {
                                maintenances.Add(MapMaintenance(reader));
                            }
                        }
                    }
                    catch (MySqlException ex)
                    {
                        throw new Exception("Database error occurred in GetMaintenanceByMaintenanceApplicationIdAsync.", ex);
                    }
                }
            }

            return maintenances;
        }

    }
}
