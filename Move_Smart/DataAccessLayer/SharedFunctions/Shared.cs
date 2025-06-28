using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Util;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using MySql.Data.MySqlClient;
using DataAccessLayer.Repositories;
using static DataAccessLayer.DriverDTO;
using static DataAccessLayer.VehicleDTO;

namespace DataAccessLayer.SharedFunctions
{
    public class SharedFunctions
    {
        private readonly ConnectionSettings _connectionSettings;

        public SharedFunctions(IConfiguration configuration, ILogger<ConnectionSettings> logger)
        {
            _connectionSettings = new ConnectionSettings(configuration, logger);
        }

        public async Task<bool> CheckUserExistsAsync(int userId)
        {
            const string query = "SELECT COUNT(*) FROM users WHERE UserID = @UserId";

            return await _connectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                return Convert.ToInt32(await cmd.ExecuteScalarAsync()) > 0;
            },
            new MySqlParameter("@UserId", userId)
            );
        }

        public async Task<bool> CheckVehicleExistsAsync(int vehicleId)
        {
            const string query = "SELECT COUNT(*) FROM vehicles WHERE VehicleID = @VehicleId";

            return await _connectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                return Convert.ToInt32(await cmd.ExecuteScalarAsync()) > 0;
            },
            new MySqlParameter("@VehicleId", vehicleId)
            );
        }

        public async Task<bool> CheckDriverExistsAsync(int driverId)
        {
            const string query = "SELECT COUNT(*) FROM drivers WHERE DriverID = @DriverId";

            return await _connectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                return Convert.ToInt32(await cmd.ExecuteScalarAsync()) > 0;
            },
            new MySqlParameter("@DriverId", driverId)
            );
        }

        public async Task<bool> CheckMissionNoteExistsAsync(int missionNoteId)
        {
            const string query = "SELECT COUNT(*) FROM missionsnotes WHERE NoteID = @MissionNoteId";

            return await _connectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                return Convert.ToInt32(await cmd.ExecuteScalarAsync()) > 0;
            },
            new MySqlParameter("@MissionNoteId", missionNoteId)
            );
        }

        public async Task<bool> CheckMissionExistsAsync(int missionId)
        {
            const string query = "SELECT COUNT(*) FROM missions WHERE MissionID = @missionId";

            return await _connectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                return Convert.ToInt32(await cmd.ExecuteScalarAsync()) > 0;
            },
            new MySqlParameter("@missionId", missionId)
            );
        }

        public async Task<bool> CheckMaintenanceApplicationExistsAsync(int maintenanceApplicationId)
        {
            const string query = "SELECT COUNT(*) FROM maintenanceapplications WHERE MaintenanceApplicationID = @MaintenanceNoteId";
            return await _connectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                return Convert.ToInt32(await cmd.ExecuteScalarAsync()) > 0;
            },
            new MySqlParameter("@MaintenanceNoteId", maintenanceApplicationId)
            );
        }

        public async Task<bool> CheckJobOrderExistsAsync(int jobOrderId)
        {
            const string query = "SELECT COUNT(*) FROM joborders WHERE OrderID = @JobOrderId";
            return await _connectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                return Convert.ToInt32(await cmd.ExecuteScalarAsync()) > 0;
            },
            new MySqlParameter("@JobOrderId", jobOrderId)
            );
        }
    
        public async Task<bool> UpdateDriverStatusAsync(int driverId, enDriverStatus status)
        {
            const string query = "UPDATE drivers SET Status = @Status WHERE DriverID = @DriverId";
            return await _connectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                return Convert.ToInt32(await cmd.ExecuteNonQueryAsync()) > 0;
            },
            new MySqlParameter("@DriverId", driverId),
            new MySqlParameter("@Status", status)
            );
        }

        public async Task<bool> UpdateVehicleStatusAsync(int vehicleId, enVehicleStatus status)
        {
            const string query = "UPDATE vehicles SET Status = @Status WHERE VehicleID = @VehicleId";
            return await _connectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                return Convert.ToInt32(await cmd.ExecuteNonQueryAsync()) > 0;
            },
            new MySqlParameter("@VehicleId", vehicleId),
            new MySqlParameter("@Status", status)
            );
        }
    }
}
