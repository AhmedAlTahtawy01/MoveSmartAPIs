using System;
using System.Data;
using System.Data.Common;
using DataAccessLayer.Util;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;


namespace DataAccessLayer.Repositories
{
    public class MaintenanceDTO
    {
        public int MaintenanceId { get; set; }
        public DateTime MaintenanceDate { get; set; }
        public string Description { get; set; }
        public int MaintenanceApplicationId { get; set; }


        [JsonConstructor]
        public MaintenanceDTO(int maintenanceId, DateTime maintenanceDate, string description, int maintenanceApplicationId)
        {
            MaintenanceId = maintenanceId;
            MaintenanceDate = maintenanceDate;
            Description = description;
            MaintenanceApplicationId = maintenanceApplicationId;
        }
    }

    public class MaintenanceRepo
    {
        private readonly ConnectionSettings _ConnectionSettings;
        private readonly ILogger<MaintenanceRepo> _logger;

        public MaintenanceRepo(ConnectionSettings ConnectionSettings, ILogger<MaintenanceRepo> logger)
        {
            _ConnectionSettings = ConnectionSettings ?? throw new ArgumentNullException(nameof(ConnectionSettings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private MaintenanceDTO MapMaintenance(DbDataReader reader)
        {
            return new MaintenanceDTO
            (
                reader.GetInt32("MaintenanceID"),
                reader.GetDateTime("MaintenanceDate"),
                reader.GetString("Description"),
                reader.GetInt32("MaintenanceApplicationID")
            );
        }

        public async Task<List<MaintenanceDTO>> GetAllMaintenancesAsync(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize < 1)
                throw new ArgumentException("Page number and page size must be greater than 0.");

            const string query = @"
                SELECT MaintenanceID, MaintenanceDate, Description, MaintenanceApplicationID
                FROM maintenance";
            int offset = (pageNumber - 1) * pageSize;

            return await _ConnectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                var maintenancesList = new List<MaintenanceDTO>();
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync()) maintenancesList.Add(MapMaintenance(reader));
                return maintenancesList;
            }, new MySqlParameter("@Offset", offset), new MySqlParameter("PageSize", pageSize));
        }

        public async Task<MaintenanceDTO?> GetMaintenanceByIdAsync(int maintenanceId)
        {
            const string query = @"
                SELECT MaintenanceID, MaintenanceDate, Description, MaintenanceApplicationID
                FROM maintenance
                WHERE MaintenanceID = @maintenanceId;";

            return await _ConnectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                using var reader = await cmd.ExecuteReaderAsync();
                return await reader.ReadAsync() ? MapMaintenance(reader) : null;
            }, new MySqlParameter("maintenanceId", maintenanceId));
        }

        public async Task<List<MaintenanceDTO>> GetMaintenancesByVehicleIdAsync(int vehicleId)
        {
            const string query = @"
                SELECT m.MaintenanceID, m.MaintenanceDate, m.Description, m.MaintenanceApplicationID
                FROM maintenance m
                JOIN maintenanceapplications ma ON m.MaintenanceApplicationID = ma.MaintenanceApplicationID
                WHERE ma.VehicleID = @vehicleId;";

            return await _ConnectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                var maintenances = new List<MaintenanceDTO>();
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync()) maintenances.Add(MapMaintenance(reader));
                return maintenances;
            }, new MySqlParameter("vehicleId", vehicleId));
        }

        private async Task<List<MaintenanceDTO>> GetMaintenancesAsync(string filter, params MySqlParameter[] parameters)
        {
            string query = @"
                SELECT MaintenanceID, MaintenanceDate, Description, MaintenanceApplicationID
                FROM maintenance";

            if (!string.IsNullOrEmpty(filter))
                query += " WHERE " + filter;

            return await _ConnectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                var maintenancesList = new List<MaintenanceDTO>();
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync()) maintenancesList.Add(MapMaintenance(reader));
                return maintenancesList;
            }, parameters);
        }

        public async Task<List<MaintenanceDTO>> GetMaintenancesByDateAsync(DateTime maintenanceDate)
        {
            return await GetMaintenancesAsync("DATE(MaintenanceDate) = @maintenanceDate",
                new MySqlParameter("@maintenanceDate", maintenanceDate.Date));
        }

        public async Task<List<MaintenanceDTO>> GetMaintenanceByMaintenanceApplicationIdAsync(int maintenanceApplicationId)
        {
            return await GetMaintenancesAsync("MaintenanceApplicationID = @maintenanceApplicationId",
                new MySqlParameter("@maintenanceApplicationId", maintenanceApplicationId));
        }

        public async Task<int> CreateMaintenanceAsync(MaintenanceDTO maintenance)
        {
            const string query = @"
                INSERT INTO maintenance (MaintenanceDate, Description, MaintenanceApplicationID)
                VALUES (@maintenanceDate, @description, @maintenanceApplicationId);
                SELECT LAST_INSERT_ID();";
            return await _ConnectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                return Convert.ToInt32(await cmd.ExecuteScalarAsync());
            },
            new MySqlParameter("@maintenanceDate", maintenance.MaintenanceDate),
            new MySqlParameter("@description", maintenance.Description),
            new MySqlParameter("@maintenanceApplicationId", maintenance.MaintenanceApplicationId));
        }

        public async Task<bool> UpdateMaintenanceAsync(MaintenanceDTO maintenance)
        {
            const string query = @"
                UPDATE maintenance
                SET MaintenanceDate = @maintenanceDate,
                    Description = @description,
                    MaintenanceApplicationID = @maintenanceApplicationId
                WHERE MaintenanceID = @maintenanceId;";
            return await _ConnectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                return await cmd.ExecuteNonQueryAsync() > 0;
            },
            new MySqlParameter("@maintenanceId", maintenance.MaintenanceId),
            new MySqlParameter("@maintenanceDate", maintenance.MaintenanceDate),
            new MySqlParameter("@description", maintenance.Description),
            new MySqlParameter("@maintenanceApplicationId", maintenance.MaintenanceApplicationId));
        }

        public async Task<bool> DeleteMaintenanceAsync(int maintenanceId)
        {
            const string query = @"
                DELETE FROM maintenance
                WHERE MaintenanceID = @maintenanceId;";
            return await _ConnectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                return await cmd.ExecuteNonQueryAsync() > 0;
            }, new MySqlParameter("@maintenanceId", maintenanceId));
        }

    }
}
