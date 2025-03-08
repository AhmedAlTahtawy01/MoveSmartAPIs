using DataAccessLayer.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Util
{
    internal class ConnectionSettings
    {
        public static string ?_connectionString;
        public static ILogger<UserRepo> _logger;

        public static void Intialize(IConfiguration configuration, ILogger<UserRepo> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }

        public static MySqlCommand GetCommand(string query, MySqlConnection conn)
        {
            var command = new MySqlCommand(query, conn);
            command.CommandType = CommandType.Text;
            return command;
        }

        public static async Task<T> ExecuteQueryAsync<T>(string query, Func<DbCommand, Task<T>> func, params MySqlParameter[] parameters)
        {
            await using var conn = ConnectionSettings.GetConnection();
            using var cmd = ConnectionSettings.GetCommand(query, conn);
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
