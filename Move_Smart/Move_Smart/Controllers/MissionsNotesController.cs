using BusinessLogicLayer.Services;
using DataAccessLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Move_Smart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MissionsNotesController : ControllerBase
    {
        private readonly MissionsNotesService _service;
        private readonly ILogger<MissionsNotesController> _logger;

        public MissionsNotesController(MissionsNotesService service, ILogger<MissionsNotesController> logger)
        {
            _service = service;
            _logger = logger;
        }


        [Authorize(Policy = "RequireGeneralSupervisor")]
        [HttpGet("All", Name = "GetAllMissionsNotes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<MissionsNotesDTO>>> GetAllMissionsNotes()
        {
            List<MissionsNotesDTO> missionsNotes = await _service.GetAllMissionsNotesAsync();

            if (missionsNotes == null || !missionsNotes.Any())
            {
                return NotFound("No missions notes found.");
            }

            return Ok(missionsNotes);
        }


        [Authorize(Policy = "RequireGeneralSupervisor")]
        [HttpGet("{noteID}", Name = "GetMissionNoteByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<MissionsNotesDTO>> GetMissionNoteByID(int noteID)
        {
            if (noteID <= 0)
            {
                return BadRequest($"Invalid ID [{noteID}]!");
            }

            MissionsNotesDTO? missionNote = await _service.GetMissionNoteByNoteIDAsync(noteID);

            if (missionNote == null)
            {
                return NotFound($"Mission note with ID {noteID} not found.");
            }

            return Ok(missionNote);
        }


        [Authorize(Policy = "RequireHospitalManager")]
        [HttpPost(Name = "AddNewMissionNote")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<MissionsNotesDTO>> AddNewMissionNote(MissionsNotesDTO dto)
        {
            if (dto == null)
            {
                return BadRequest("MissionsNotesDTO cannot be null!");
            }

            if (await _service.AddNewMissionNoteAsync(dto) == null)
            {
                return BadRequest("Failed to add new mission note!");
            }

            return CreatedAtAction("GetMissionNoteByID", new { noteID = dto.NoteID }, dto);
        }


        [Authorize(Policy = "RequireHospitalManager")]
        [HttpPut(Name = "UpdateMissionNote")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<bool>> UpdateMissionNote(MissionsNotesDTO dto)
        {
            if (dto == null)
            {
                return BadRequest("MissionsNotesDTO cannot be null!");
            }

            if (!await _service.IsMissionNoteExistsAsync(dto.NoteID ?? 0))
            {
                return NotFound($"Mission note with ID [{dto.NoteID}] not found!");
            }

            if (!await _service.UpdateMissionNoteAsync(dto))
            {
                return BadRequest($"Failed to update mission note with ID [{dto.NoteID}]!");
            }

            return Ok($"Mission note with ID [{dto.NoteID}] updated successfully.");
        }


        [Authorize(Policy = "RequireHospitalManager")]
        [HttpDelete("{noteID}", Name = "DeleteMissionNote")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<bool>> DeleteMissionNote(int noteID)
        {
            if (noteID <= 0)
            {
                return BadRequest($"Invalid ID [{noteID}]!");
            }
            
            if (!await _service.IsMissionNoteExistsAsync(noteID))
            {
                return NotFound($"Mission note with ID [{noteID}] not found!");
            }
            
            if (!await _service.DeleteMissionNoteAsync(noteID))
            {
                return BadRequest($"Failed to delete mission note with ID [{noteID}]!");
            }
            
            return Ok($"Mission note with ID [{noteID}] deleted successfully.");
        }
    }
}
