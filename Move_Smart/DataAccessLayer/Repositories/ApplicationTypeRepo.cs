using System;
using System.Data;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Data.Common;
using DataAccessLayer.Util;

namespace DataAccessLayer.Repositories
{
    public class ApplicationTypeDTO
    {
        public int TypeId { get; set; }
        public string TypeName { get; set; }

        public ApplicationTypeDTO(int typeId, string typeName)
        {
            this.TypeId = typeId;
            this.TypeName = typeName;
        }
    }

    public class ApplicationTypeRepo
    {
        private ApplicationTypeDTO MapApplicationType(DbDataReader reader)
        {
            return new ApplicationTypeDTO
            (
                reader.GetInt32("TypeID"),
                reader.GetString("TypeName")
            );
        }

        public async Task<List<ApplicationTypeDTO>> GetAllApplicationTypesAsync()
        {
            var applicationTypes = new List<ApplicationTypeDTO>();
            await using var conn = ConnectionSettings.GetConnection();
            const string query = "SELECT TypeID, TypeName FROM applicationtypes";

            using var cmd = ConnectionSettings.GetCommand(query, conn);

            try
            {
                await conn.OpenAsync().ConfigureAwait(false);
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    applicationTypes.Add(MapApplicationType(reader));
                }
            }
            catch (MySqlException ex)
            {
                throw new Exception("Database error occurred in GetAllApplicationTypesAsync.", ex);
            }

            return applicationTypes;

        }

        public async Task<ApplicationTypeDTO> GetApplicationTypeByIdAsync(int typeId)
        {
            await using var conn = ConnectionSettings.GetConnection();
            const string query = "SELECT TypeID, TypeName FROM applicationtypes WHERE TypeID = @typeId";

            using var cmd = ConnectionSettings.GetCommand(query, conn);
            cmd.Parameters.AddWithValue("@typeId", typeId);

            try
            {
                await conn.OpenAsync().ConfigureAwait(false);
                using var reader = await cmd.ExecuteReaderAsync();
                return await reader.ReadAsync() ? MapApplicationType(reader) : null;
            }
            catch (MySqlException ex)
            {
                throw new Exception("Database error occurred in GetApplicationTypeByIdAsync.", ex);
            }
        }

        public async Task<bool> ApplicationTypeExistsAsync(string typeName)
        {
            await using var conn = ConnectionSettings.GetConnection();
            const string query = "SELECT COUNT(*) FROM applicationtypes WHERE TypeName = @typeName";

            using var cmd = ConnectionSettings.GetCommand(query, conn);
            cmd.Parameters.AddWithValue("@typeName", typeName);

            try
            {
                await conn.OpenAsync().ConfigureAwait(false);
                return Convert.ToInt32(await cmd.ExecuteScalarAsync().ConfigureAwait(false)) > 0;
            }
            catch (MySqlException ex)
            {
                throw new Exception("Database error occurred in ApplicationTypeExistsAsync.", ex);
            }
        }

        public async Task<int> CreateApplicationTypeAsync(string typeName)
        {
            await using var conn = ConnectionSettings.GetConnection();
            const string query = "INSERT INTO applicationtypes (TypeName) VALUES (@typeName); SELECT LAST_INSERT_ID();";

            using var cmd = ConnectionSettings.GetCommand(query, conn);
            cmd.Parameters.AddWithValue("@typeName", typeName);

            try
            {
                await conn.OpenAsync().ConfigureAwait(false);
                return Convert.ToInt32(await cmd.ExecuteScalarAsync().ConfigureAwait(false));
            }
            catch (MySqlException ex)
            {
                throw new Exception("Database error occurred in CreateApplicationTypeAsync.", ex);
            }
        }

        public async Task<bool> UpdateApplicationTypeAsync(ApplicationTypeDTO applicationType)
        {

            await using var conn = ConnectionSettings.GetConnection();
            const string query = "UPDATE applicationtypes SET TypeName = @typeName WHERE TypeID = @typeId";

            using var cmd = ConnectionSettings.GetCommand(query, conn);
            cmd.Parameters.AddWithValue("@typeId", applicationType.TypeId);
            cmd.Parameters.AddWithValue("@typeName", applicationType.TypeName);

            try
            {
                await conn.OpenAsync().ConfigureAwait(false);
                return await cmd.ExecuteNonQueryAsync().ConfigureAwait(false) > 0;
            }
            catch (MySqlException ex)
            {
                throw new Exception("Database error occurred in UpdateApplicationTypeAsync.", ex);
            }
        }

        public async Task<bool> DeleteApplicationTypeAsync(ApplicationTypeDTO applicationType)
        {

            await using var conn = ConnectionSettings.GetConnection();
            const string query = "DELETE FROM applicationtypes WHERE TypeID = @typeId";

            using var cmd = ConnectionSettings.GetCommand(query, conn);
            cmd.Parameters.AddWithValue("@typeId", applicationType.TypeId);

            try
            {
                await conn.OpenAsync().ConfigureAwait(false);
                return await cmd.ExecuteNonQueryAsync().ConfigureAwait(false) > 0;
            }
            catch (MySqlException ex)
            {
                throw new Exception("Database error occurred in DeleteApplicationTypeAsync.", ex);
            }

        }
    }
}
