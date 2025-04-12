using System;
using System.Data;
using System.Data.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;

namespace DataAccessLayer.Util
{
    public class ConnectionSettings
    {
        private readonly string _connectionString;
        private readonly ILogger<ConnectionSettings> _logger;

        public ConnectionSettings(IConfiguration configuration, ILogger<ConnectionSettings> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(configuration), "Connection string 'DefaultConnection' is missing.");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public MySqlConnection GetConnection() => new MySqlConnection(_connectionString);
        

        public MySqlCommand GetCommand(string query, MySqlConnection conn)
        {
            var command = new MySqlCommand(query, conn);
            command.CommandType = CommandType.Text;
            return command;
        }

        public async Task<T> ExecuteQueryAsync<T>(string query, Func<DbCommand, Task<T>> func, params MySqlParameter[] parameters)
        {
            await using var conn = GetConnection();
            using var cmd = GetCommand(query, conn);
            cmd.Parameters.AddRange(parameters);

            try
            {
                await conn.OpenAsync().ConfigureAwait(false);
                return await func(cmd).ConfigureAwait(false);
            }
            catch (MySqlException ex)
            {
                _logger.LogError(ex, $"Database error occurred in {nameof(func.Method)}.");
                throw new Exception($"Database error occurred in {nameof(func.Method)}.", ex);
            }
        }
    }
}