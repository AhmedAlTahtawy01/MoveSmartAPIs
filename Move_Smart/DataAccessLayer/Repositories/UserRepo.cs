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
        SuperUser = 0,
        HospitalManager = 1,
        GeneralManager = 2,
        GeneralSupervisor = 3,
        PatrolsSupervisor = 4,
        WorkshopSupervisor = 5,
        AdministrativeSupervisor = 6
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
        private readonly ConnectionsSettings _connectionSettings;
        private readonly ILogger<UserRepo> _logger;

        public UserRepo(ConnectionsSettings connectionSettings, ILogger<UserRepo> logger)
        {
            _connectionSettings = connectionSettings ?? throw new ArgumentNullException(nameof(connectionSettings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private UserDTO MapUser(DbDataReader reader)
        {
            string roleString = reader.GetString(reader.GetOrdinal("Role"));
            if (!Enum.TryParse(roleString, true, out EnUserRole role))
            {
                _logger.LogError($"Invalid Role value in database: {roleString}");
                throw new InvalidOperationException($"Unable to parse role: {roleString}");
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

            const string query = @"
                        SELECT UserID, NationalNo, Password, Name, Role, AccessRight
                        FROM users
                        LIMIT @Offset, @PageSize";
            int offset = (pageNumber - 1) * pageSize;

            return await _connectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                var usersList = new List<UserDTO>();
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync()) usersList.Add(MapUser(reader));
                return usersList;
            }, new MySqlParameter("@Offset", offset), new MySqlParameter("@PageSize", pageSize));
        }

        public async Task<UserDTO?> GetUserByIdAsync(int userId)
        {
            const string query = "SELECT UserID, NationalNo, Password, Name, Role, AccessRight FROM users WHERE UserID = @userId";

            return await _connectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                using var reader = await cmd.ExecuteReaderAsync();
                return await reader.ReadAsync() ? MapUser(reader) : null;
            }, new MySqlParameter("@userId", userId));
        }

        public async Task<UserDTO?> GetUserByNationalNoAsync(string nationalNo)
        {
            const string query = "SELECT UserID, NationalNo, Password, Name, Role, AccessRight FROM users WHERE NationalNo = @nationalNo";

            return await _connectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                using var reader = await cmd.ExecuteReaderAsync();
                return await reader.ReadAsync() ? MapUser(reader) : null;
            }, new MySqlParameter("@nationalNo", nationalNo));
        }

        public async Task<bool> NationalNoExistsAsync(string nationalNo, int excludeUserId = 0)
        {
            string query = "SELECT COUNT(*) FROM users WHERE NationalNo = @nationalNo";
            var parameters = new List<MySqlParameter>
            {
                new MySqlParameter("@nationalNo", nationalNo)
            };

            if (excludeUserId > 0)
            {
                query += " AND UserID != @excludeUserId";
                parameters.Add(new MySqlParameter("@excludeUserId", excludeUserId));
            }

            return await _connectionSettings.ExecuteQueryAsync(query, async cmd =>
            {
                return Convert.ToInt32(await cmd.ExecuteScalarAsync()) > 0;
            }, parameters.ToArray());
        }

        public async Task<int> CreateUserAsync(UserDTO user)
        {
            const string query = @"
                INSERT INTO users (NationalNo, Password, Name, Role, AccessRight)
                VALUES (@NationalNo, @Password, @Name, @Role, @AccessRight);
                SELECT LAST_INSERT_ID();";

            return await _connectionSettings.ExecuteQueryAsync(query, async cmd =>
                Convert.ToInt32(await cmd.ExecuteScalarAsync()),
                new MySqlParameter("@NationalNo", user.NationalNo),
                new MySqlParameter("@Password", user.Password),
                new MySqlParameter("@Name", user.Name),
                new MySqlParameter("@Role", user.Role.ToString()),
                new MySqlParameter("@AccessRight", user.AccessRight));
        }

        public async Task<bool> UpdateUserInfoAsync(UserDTO user)
        {
            const string query = @"
                UPDATE users
                SET NationalNo = @NationalNo, Password = @Password, Name = @Name
                WHERE UserID = @UserID";

            return await _connectionSettings.ExecuteQueryAsync(query, async cmd =>
                await cmd.ExecuteNonQueryAsync() > 0,
                new MySqlParameter("@UserID", user.UserId),
                new MySqlParameter("@NationalNo", user.NationalNo),
                new MySqlParameter("@Password", user.Password),
                new MySqlParameter("@Name", user.Name));
        }

        public async Task<bool> UpdateAllUserInfoAsync(UserDTO user)
        {
            const string query = @"
                UPDATE users
                SET NationalNo = @NationalNo, Password = @Password, Name = @Name, 
                    Role = @Role, AccessRight = @AccessRight
                WHERE UserID = @UserID";

            return await _connectionSettings.ExecuteQueryAsync(query, async cmd =>
                await cmd.ExecuteNonQueryAsync() > 0,
                new MySqlParameter("@UserID", user.UserId),
                new MySqlParameter("@NationalNo", user.NationalNo),
                new MySqlParameter("@Password", user.Password),
                new MySqlParameter("@Name", user.Name),
                new MySqlParameter("@Role", user.Role.ToString()),
                new MySqlParameter("@AccessRight", user.AccessRight));
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            const string query = "DELETE FROM users WHERE UserID = @UserID";

            return await _connectionSettings.ExecuteQueryAsync(query, async cmd =>
                await cmd.ExecuteNonQueryAsync() > 0,
                new MySqlParameter("@UserID", userId));
        }
    }
}