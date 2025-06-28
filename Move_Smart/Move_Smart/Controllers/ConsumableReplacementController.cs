using BusinessLogicLayer.Services;
using DataAccessLayer.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Move_Smart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsumableReplacementController : ControllerBase
    {
        private readonly ConsumablesReplacementRepo _consumablesreplacement;

        public ConsumableReplacementController(ConsumablesReplacementRepo consumablesreplacement)
        {
            _consumablesreplacement = consumablesreplacement;
        }

        [Authorize(Policy = "RequireWorkshopSupervisor")]
        [HttpGet]
        public async Task<IActionResult> Getall()
        {
            try
            {
                var data = await _consumablesreplacement.GetAllConsumablesReplacement();
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
                var data = await _consumablesreplacement.GetConsumablesReplacementByID(id);
                if (data == null)
                    return NotFound(new { message = "Consumable replacement not found." });

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
                await _consumablesreplacement.DeleteAddConsumablesReplacement(id);
                return Ok(new { message = "Consumable replacement deleted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Policy = "RequireWorkshopSupervisor")]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Consumablesreplacement consumablesreplacement)
        {
            try
            {
                await _consumablesreplacement.UpdateConsumablesReplacement(consumablesreplacement);
                return Ok(new { message = "Consumable replacement updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Policy = "RequireWorkshopSupervisor")]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Consumablesreplacement consumablesreplacement)
        {
            try
            {
                await _consumablesreplacement.AddConsumablesReplacement(consumablesreplacement);
                return Ok(new { message = "Consumable replacement added successfully." });
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
            try
            {
                var count = await _consumablesreplacement.CountAllOrdersAsync();
                return Ok( new { count });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
