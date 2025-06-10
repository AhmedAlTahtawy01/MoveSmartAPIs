using DataAccessLayer.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Move_Smart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SparePartReplacementsController : ControllerBase
    {
        private readonly SparePartsReplacement _sparePartsReplacement;

        public SparePartReplacementsController(SparePartsReplacement sparePartsReplacement)
        {
            _sparePartsReplacement = sparePartsReplacement;
        }

        [Authorize(Policy = "RequireWorkshopSupervisor")]
        [HttpGet]
        public async Task<IActionResult> Getall()
        {
            try
            {
                var data = await _sparePartsReplacement.GetAllSparePartsReplacement();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Policy = "RequireWorkshopSupervisor")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByID(int id)
        {
            try
            {
                var data = await _sparePartsReplacement.GetSparePartsReplacementByID(id);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Policy = "RequireWorkshopSupervisor")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _sparePartsReplacement.DeleteSparePartsReplacement(id);
                return Ok(new { message = "Spare part replacement deleted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Policy = "RequireWorkshopSupervisor")]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Sparepartsreplacement order)
        {
            try
            {
                await _sparePartsReplacement.UpdateSparePartsReplacement(order);
                return Ok(new { message = "Spare part replacement updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Policy = "RequireWorkshopSupervisor")]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Sparepartsreplacement order)
        {
            try
            {
                await _sparePartsReplacement.AddSparePartsReplacement(order);
                return Ok(new { message = "Spare part replacement added successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("count")]
        public async Task<IActionResult> Count()
        {
            try
            {
                var count = await _sparePartsReplacement.CountAllOrdersAsync();
                return Ok(count);   
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
