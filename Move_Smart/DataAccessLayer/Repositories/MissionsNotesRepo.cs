using DataAccessLayer.Repositories;
using DataAccessLayer.Util;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class MissionsNotesDTO
    {
        public int NoteID { get; set; }
        public int ApplicationID { get; set; }

        public MissionsNotesDTO(int noteID, int applicationID)
        {
            NoteID = noteID;
            ApplicationID = applicationID;
        }
    }

    public class MissionsNotesRepo
    {

        private readonly ConnectionSettings _connectionSettings;
        private readonly ILogger<MissionsNotesRepo> _logger;

        public MissionsNotesRepo(ConnectionSettings connectionSettings, ILogger<MissionsNotesRepo> logger)
        {
            _connectionSettings = connectionSettings ?? throw new ArgumentNullException(nameof(connectionSettings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<MissionsNotesDTO>> GetAllMissionsNotesAsync()
        {
            List<MissionsNotesDTO> notesList = new List<MissionsNotesDTO>();

            string query = @"SELECT * FROM MissionsNotes;";

            try
            {
                using (MySqlConnection conn = _connectionSettings.GetConnection())
                {
                    using (MySqlCommand cmd = _connectionSettings.GetCommand(query, conn))
                    {
                        await conn.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                notesList.Add(new MissionsNotesDTO(
                                    Convert.ToInt32(reader["NoteID"]),
                                    Convert.ToInt32(reader["ApplicationID"])
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

            return notesList;
        }

        public async Task<MissionsNotesDTO> GetMissionNoteByNoteIDAsync(int noteID)
        {
            string query = @"SELECT * FROM MissionsNotes
                            WHERE NoteID = @NoteID;";

            try
            {
                using (MySqlConnection conn = _connectionSettings.GetConnection())
                {
                    using (MySqlCommand cmd = _connectionSettings.GetCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("NoteID", noteID);

                        await conn.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new MissionsNotesDTO(
                                    Convert.ToInt32(reader["NoteID"]),
                                    Convert.ToInt32(reader["ApplicationID"])
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

        public async Task<MissionsNotesDTO> GetMissionNoteByApplicationIDAsync(int applicationID)
        {
            string query = @"SELECT * FROM MissionsNotes
                            WHERE ApplicationID = @ApplicationID;";

            try
            {
                using (MySqlConnection conn = _connectionSettings.GetConnection())
                {
                    using (MySqlCommand cmd = _connectionSettings.GetCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("ApplicationID", applicationID);

                        await conn.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new MissionsNotesDTO(
                                    Convert.ToInt32(reader["NoteID"]),
                                    Convert.ToInt32(reader["ApplicationID"])
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

        public async Task<int?> AddNewMissionNoteAsync(MissionsNotesDTO newNote)
        {
            string query = @"INSERT INTO MissionsNotes
                            (ApplicationID)
                            VALUES
                            (@ApplicationID);
                            SELECT LAST_INSERT_ID();";

            try
            {
                using (MySqlConnection conn = _connectionSettings.GetConnection())
                {
                    using (MySqlCommand cmd = _connectionSettings.GetCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("ApplicationID", newNote.ApplicationID);

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

        public async Task<bool> UpdateMissionNoteAsync(MissionsNotesDTO updatedNote)
        {
            string query = @"UPDATE MissionsNotes SET
                            ApplicationID = @ApplicationID,
                            WHERE NoteID = @NoteID;";

            try
            {
                using (MySqlConnection conn = _connectionSettings.GetConnection())
                {
                    using (MySqlCommand cmd = _connectionSettings.GetCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("NoteID", updatedNote.NoteID);
                        cmd.Parameters.AddWithValue("ApplicationID", updatedNote.ApplicationID);

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

        public async Task<bool> DeleteMissionNoteAsync(int noteID)
        {
            string query = @"DELETE FROM MissionsNotes
                            WHERE NoteID = @NoteID;";

            try
            {
                using (MySqlConnection conn = _connectionSettings.GetConnection())
                {
                    using (MySqlCommand cmd = _connectionSettings.GetCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("NoteID", noteID);

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

        public async Task<bool> IsMissionNoteExistsAsync(int noteID)
        {
            string query = @"SELECT Found=1 FROM MissionsNotes
                            WHERE NoteID = @NoteID;";

            try
            {
                using (MySqlConnection conn = _connectionSettings.GetConnection())
                {
                    using (MySqlCommand cmd = _connectionSettings.GetCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("NoteID", noteID);
            
                        await conn.OpenAsync();
                        
                        return cmd.ExecuteScalarAsync() != null;
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
