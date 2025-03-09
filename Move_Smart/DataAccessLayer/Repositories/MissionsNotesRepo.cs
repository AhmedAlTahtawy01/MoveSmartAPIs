using DataAccessLayer.Util;
using MySqlConnector;
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
        public bool ApprovedByGeneralSupervisor { get; set; }
        public bool ApprovedByGeneralManager { get; set; }

        public MissionsNotesDTO(int noteID, int applicationID, bool approvedByGeneralSupervisor,
            bool approvedByGeneralManager)
        {
            NoteID = noteID;
            ApplicationID = applicationID;
            ApprovedByGeneralSupervisor = approvedByGeneralSupervisor;
            ApprovedByGeneralManager = approvedByGeneralManager;
        }
    }

    public class MissionsNotesRepo
    {
        public static async Task<List<MissionsNotesDTO>> GetAllMissionsNotesAsync()
        {
            List<MissionsNotesDTO> notesList = new List<MissionsNotesDTO>();

            string query = @"SELECT * FROM MissionsNotes
                            ORDER ApprovedByGeneralSupervisor DESC, BY ApprovedByGeneralManager DESC;";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnectionSettings.ConnectionString))
                {
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        await conn.OpenAsync();

                        using (MySqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while(await reader.ReadAsync())
                            {
                                notesList.Add(new MissionsNotesDTO(
                                    Convert.ToInt32(reader["NoteID"]),
                                    Convert.ToInt32(reader["ApplicationID"]),
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

            return notesList;
        }

        public static async Task<List<MissionsNotesDTO>> GetAllApprovedNotesAsync()
        {
            List<MissionsNotesDTO> notesList = new List<MissionsNotesDTO>();

            string query = @"SELECT * FROM MissionsNotes
                            WHERE ApprovedByGeneralSupervisor = 1 AND ApprovedByGeneralManager = 1;";

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
                                notesList.Add(new MissionsNotesDTO(
                                    Convert.ToInt32(reader["NoteID"]),
                                    Convert.ToInt32(reader["ApplicationID"]),
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

            return notesList;
        }

        public static async Task<List<MissionsNotesDTO>> GetAllNonApprovedNotesAsync()
        {
            List<MissionsNotesDTO> notesList = new List<MissionsNotesDTO>();

            string query = @"SELECT * FROM MissionsNotes
                            WHERE ApprovedByGeneralSupervisor = 0 OR ApprovedByGeneralManager = 0
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
                                notesList.Add(new MissionsNotesDTO(
                                    Convert.ToInt32(reader["NoteID"]),
                                    Convert.ToInt32(reader["ApplicationID"]),
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

            return notesList;
        }

        public static async Task<List<MissionsNotesDTO>> GetAllNonApprovedNotesFromGeneralSupervisorsAsync()
        {
            List<MissionsNotesDTO> notesList = new List<MissionsNotesDTO>();

            string query = @"SELECT * FROM MissionsNotes
                            WHERE ApprovedByGeneralSupervisor = 0;";

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
                                notesList.Add(new MissionsNotesDTO(
                                    Convert.ToInt32(reader["NoteID"]),
                                    Convert.ToInt32(reader["ApplicationID"]),
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

            return notesList;
        }

        public static async Task<List<MissionsNotesDTO>> GetAllNonApprovedNotesFromGeneralManagerAsync()
        {
            List<MissionsNotesDTO> notesList = new List<MissionsNotesDTO>();

            string query = @"SELECT * FROM MissionsNotes
                            WHERE ApprovedByGeneralManager = 0;";

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
                                notesList.Add(new MissionsNotesDTO(
                                    Convert.ToInt32(reader["NoteID"]),
                                    Convert.ToInt32(reader["ApplicationID"]),
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

            return notesList;
        }

        public static async Task<MissionsNotesDTO> GetMissionNoteByNoteIDAsync(int noteID)
        {
            string query = @"SELECT * FROM MissionsNotes
                            WHERE NoteID = @NoteID;";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnectionSettings.ConnectionString))
                {
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("NoteID", noteID);

                        await conn.OpenAsync();
                        
                        using (MySqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new MissionsNotesDTO(
                                    Convert.ToInt32(reader["NoteID"]),
                                    Convert.ToInt32(reader["ApplicationID"]),
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

        public static async Task<MissionsNotesDTO> GetMissionNoteByApplicationIDAsync(int applicationID)
        {
            string query = @"SELECT * FROM MissionsNotes
                            WHERE ApplicationID = @ApplicationID;";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnectionSettings.ConnectionString))
                {
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("ApplicationID", applicationID);

                        await conn.OpenAsync();

                        using (MySqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new MissionsNotesDTO(
                                    Convert.ToInt32(reader["NoteID"]),
                                    Convert.ToInt32(reader["ApplicationID"]),
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

        public static async Task<int?> AddNewMissionNoteAsync(MissionsNotesDTO newNote)
        {
            string query = @"INSERT INTO MissionsNotes
                            (ApplicationID, ApprovedByGeneralSupervisor, ApprovedByGeneralManager)
                            VALUES
                            (@ApplicationID, @ApprovedByGeneralSupervisor, @ApprovedByGeneralManager);
                            SELECT LAST_INSERT_ID();";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnectionSettings.ConnectionString))
                {
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("ApplicationID", newNote.ApplicationID);
                        cmd.Parameters.AddWithValue("ApprovedByGeneralSupervisor", newNote.ApprovedByGeneralSupervisor);
                        cmd.Parameters.AddWithValue("ApprovedByGeneralManager", newNote.ApprovedByGeneralManager);

                        await conn.OpenAsync();

                        object? result = await cmd.ExecuteScalarAsync();
                        if(result != null && int.TryParse(result.ToString(), out int id))
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

        public static async Task<bool> UpdateMissionNoteAsync(MissionsNotesDTO updatedNote)
        {
            string query = @"UPDATE MissionsNotes SET
                            ApplicationID = @ApplicationID,
                            ApprovedByGeneralSupervisor = @ApprovedByGeneralSupervisor,
                            ApprovedByGeneralManager = @ApprovedByGeneralManager
                            WHERE NoteID = @NoteID;";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnectionSettings.ConnectionString))
                {
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("NoteID", updatedNote.NoteID);
                        cmd.Parameters.AddWithValue("ApplicationID", updatedNote.ApplicationID);
                        cmd.Parameters.AddWithValue("ApprovedByGeneralSupervisor", updatedNote.ApprovedByGeneralSupervisor);
                        cmd.Parameters.AddWithValue("ApprovedByGeneralManager", updatedNote.ApprovedByGeneralManager);

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

        public static async Task<bool> DeleteMissionNoteAsync(int noteID)
        {
            string query = @"DELETE FROM MissionsNotes
                            WHERE NoteID = @NoteID;";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnectionSettings.ConnectionString))
                {
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
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
    }
}
