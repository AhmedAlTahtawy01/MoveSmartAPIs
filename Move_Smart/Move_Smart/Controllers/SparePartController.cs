using DataAccessLayer.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Move_Smart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SparePartController : ControllerBase
    {
        private readonly Sparepart _isparepart;

        public SparePartController(Sparepart isparepart)
        {
            _isparepart = isparepart;
        }

        [Authorize(Policy = "RequireWorkshopSupervisor")]
        [HttpGet]
        public async Task<IActionResult> GetAllSparePart()
        {
            var data = await _isparepart.GetAllSparePart();
            return Ok(data);
        }

        [Authorize(Policy = "RequireWorkshopSupervisor")]
        [HttpGet("id/{id}")]
        public async Task<IActionResult> GetByID(int id)
        {
            try
            {
                var data = await _isparepart.GetSparePartByID(id);
                return Ok(data);
            }
            catch (Exception ex)
            {
                // Return only the error message
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Policy = "RequireWorkshopSupervisor")]
        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetByName(string name )
        {
            try
            {
                var data = await _isparepart.GetSparePartByName(name);
                return Ok(data);
            }
            catch (Exception ex)
            {
                // Return only the error message
                return BadRequest(new { message = ex.Message });
            }
            
        }

        [Authorize(Policy = "GeneralSupervisor")]
        [HttpPost]
        public async Task<IActionResult> AddSparePart([FromBody] Sparepart spare)
        {
            try
            {
                await _isparepart.AddSparePart(spare);
                return Ok(new { message = "Spare part added successfully." });
            }
            catch (Exception ex)
            {
                // Return only the error message
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Policy = "GeneralSupervisor")]
        [HttpPut]
        public async Task<IActionResult> UpdateSparePart([FromBody] Sparepart spare)
        {
            try
            {
                await _isparepart.UpdateSparePart(spare);
                return Ok(new { message = "Spare part updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Policy = "GeneralSupervisor")]
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteSparePart(int id)
        {
            try
            {
                await _isparepart.DeleteSparePart(id);
                return Ok(new { message = "Spare part deleted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [Authorize(Policy = "RequireWorkshopSupervisor")]
        [HttpGet("count")]
        public async Task<IActionResult> Count()
        {
            var count = await _isparepart.CountAllOrdersAsync();
            return Ok(new { message = count });
        }
    }
}
