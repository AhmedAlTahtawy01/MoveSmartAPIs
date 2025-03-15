using DataAccessLayer.Util;
using System;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data.Common;
using Microsoft.Extensions.Logging;

namespace DataAccessLayer.Repositories
{
    public enum EnUserRole
    {
        SuperUser,
        HospitalManager,
        GeneralManager,
        GeneralSupervisor,
        PatrolsSupervisor,
        WorkshopSupervisor,
        AdministrativeSupervisor
    }

    public class UserDTO
    {
        public int UserId { get; set; }
        public string NationalNo { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public EnUserRole Role { get; set; }
        public int AccessRight { get; set; }

        public UserDTO(int userId, string nationalNo, string password, string name, EnUserRole role, int accessRight)
        {
            UserId = userId;
            NationalNo = nationalNo;
            Password = password;
            Name = name;
            Role = role;
            AccessRight = accessRight;
        }
    }

    public class UserRepo
    {
        private readonly ConnectionSettings _connectionSettings;
        private readonly ILogger<UserRepo> _logger;

        public UserRepo(ConnectionSettings connectionSettings, ILogger<UserRepo> logger)
        {
            _connectionSettings = connectionSettings ?? throw new ArgumentNullException(nameof(connectionSettings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private UserDTO MapUser(DbDataReader reader)
        {
            if (!Enum.TryParse(reader["Role"].ToString(), out EnUserRole role))
            {
                _logger.LogError($"Invalid Role value in database: {reader["Role"]}");
                throw new InvalidOperationException($"Unable to parse role: {reader["Role"]}");
            }

            return new UserDTO
            (
                reader.GetInt32(reader.GetOrdinal("UserID")),
                reader.GetString(reader.GetOrdinal("NationalNo")),
                reader.GetString(reader.GetOrdinal("Password")),
                reader.GetString(reader.GetOrdinal("Name")),
                role,
                reader.GetInt32(reader.GetOrdinal("AccessRight"))
            );
        }

        public async Task<List<UserDTO>> GetAllUsersAsync(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize < 1)
                throw new ArgumentException("Page number and page size must be greater than 0.");

            const string query = "SELECT UserID, NationalNo, Password, Name, Role, AccessRight FROM users LIMIT @Offset, @PageSize";
            int offset = (pageNumber - 1) * pageSize;

            return await _connectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                var usersList = new List<UserDTO>();
                using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
                while (await reader.ReadAsync()) usersList.Add(MapUser(reader));
                return usersList;
            }, new MySqlParameter("@Offset", offset), new MySqlParameter("@PageSize", pageSize));
        }

        public async Task<UserDTO> GetUserByIdAsync(int userId)
        {
            const string query = "SELECT UserID, NationalNo, Password, Name, Role, AccessRight FROM users WHERE UserID = @userId";

            return await _connectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
                return await reader.ReadAsync() ? MapUser(reader) : null;
            }, new MySqlParameter("@userId", userId));
        }

        public async Task<UserDTO> GetUserByNationalNoAsync(string nationalNo)
        {
            const string query = "SELECT UserID, NationalNo, Password, Name, Role, AccessRight FROM users WHERE NationalNo = @nationalNo";

            return await _connectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
                return await reader.ReadAsync() ? MapUser(reader) : null;
            }, new MySqlParameter("@nationalNo", nationalNo));
        }

        public async Task<bool> NationalNoExistsAsync(string nationalNo, int excludeUserId = 0)
        {
            string query = "SELECT COUNT(*) FROM users WHERE NationalNo = @nationalNo";
            if (excludeUserId > 0)
                query += " AND UserID != @excludeUserId";

            return await _connectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                return Convert.ToInt32(await cmd.ExecuteScalarAsync().ConfigureAwait(false)) > 0;
            }, new MySqlParameter("@nationalNo", nationalNo),
               excludeUserId > 0 ? new MySqlParameter("@excludeUserId", excludeUserId) : null);
        }

        public async Task<int> CreateUserAsync(UserDTO user)
        {
            await using var conn = _connectionSettings.GetConnection();
            const string query = @"
                INSERT INTO users (NationalNo, Password, Name, Role, AccessRight)
                VALUES (@NationalNo, @Password, @Name, @Role, @AccessRight);
                SELECT LAST_INSERT_ID();";

            using var cmd = _connectionSettings.GetCommand(query, conn);
            cmd.Parameters.AddWithValue("@NationalNo", user.NationalNo);
            cmd.Parameters.AddWithValue("@Password", user.Password);
            cmd.Parameters.AddWithValue("@Name", user.Name);
            cmd.Parameters.AddWithValue("@Role", user.Role.ToString());
            cmd.Parameters.AddWithValue("@AccessRight", user.AccessRight);

            try
            {
                await conn.OpenAsync().ConfigureAwait(false);
                return Convert.ToInt32(await cmd.ExecuteScalarAsync().ConfigureAwait(false));
            }
            catch (MySqlException ex)
            {
                _logger.LogError($"Database error occurred in CreateUserAsync.", ex);
                throw new Exception("Database error occurred in CreateUserAsync.", ex);
            }
        }

        public async Task<bool> UpdateUserInfoAsync(UserDTO user)
        {
            await using var conn = _connectionSettings.GetConnection();
            const string query = @"
                UPDATE users
                SET NationalNo = @NationalNo, Password = @Password, Name = @Name
                WHERE UserID = @UserID";

            using var cmd = _connectionSettings.GetCommand(query, conn);
            cmd.Parameters.AddWithValue("@UserID", user.UserId);
            cmd.Parameters.AddWithValue("@NationalNo", user.NationalNo);
            cmd.Parameters.AddWithValue("@Password", user.Password);
            cmd.Parameters.AddWithValue("@Name", user.Name);

            try
            {
                await conn.OpenAsync().ConfigureAwait(false);
                return await cmd.ExecuteNonQueryAsync().ConfigureAwait(false) > 0;
            }
            catch (MySqlException ex)
            {
                _logger.LogError($"Database error occurred in UpdateUserInfoAsync.", ex);
                throw new Exception("Database error occurred in UpdateUserInfoAsync.", ex);
            }
        }

        public async Task<bool> UpdateAllUserInfoAsync(UserDTO user)
        {
            await using var conn = _connectionSettings.GetConnection();
            const string query = @"
                UPDATE users
                SET NationalNo = @NationalNo, Password = @Password, Name = @Name, Role = @Role, AccessRight = @AccessRight
                WHERE UserID = @UserID";

            using var cmd = _connectionSettings.GetCommand(query, conn);
            cmd.Parameters.AddWithValue("@UserID", user.UserId);
            cmd.Parameters.AddWithValue("@NationalNo", user.NationalNo);
            cmd.Parameters.AddWithValue("@Password", user.Password);
            cmd.Parameters.AddWithValue("@Name", user.Name);
            cmd.Parameters.AddWithValue("@Role", user.Role.ToString());
            cmd.Parameters.AddWithValue("@AccessRight", user.AccessRight);

            try
            {
                await conn.OpenAsync().ConfigureAwait(false);
                return await cmd.ExecuteNonQueryAsync().ConfigureAwait(false) > 0;
            }
            catch (MySqlException ex)
            {
                _logger.LogError($"Database error occurred in UpdateAllUserInfoAsync.", ex);
                throw new Exception("Database error occurred in UpdateAllUserInfoAsync.", ex);
            }
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            await using var conn = _connectionSettings.GetConnection();
            const string query = "DELETE FROM users WHERE UserID = @UserID";

            using var cmd = _connectionSettings.GetCommand(query, conn);
            cmd.Parameters.AddWithValue("@UserID", userId);

            try
            {
                await conn.OpenAsync().ConfigureAwait(false);
                return await cmd.ExecuteNonQueryAsync().ConfigureAwait(false) > 0;
            }
            catch (MySqlException ex)
            {
                _logger.LogError("Database error occurred in DeleteUserAsync.", ex);
                throw new Exception("Database error occurred in DeleteUserAsync.", ex);
            }
        }
    }
}