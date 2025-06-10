using BusinessLogicLayer.Services;
using DataAccessLayer.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Move_Smart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsumableWithdrawApplicationServiceController : ControllerBase
    {
        private readonly CosumableWithdawApplicationService _cosumableWithdawApplicationService;

        public ConsumableWithdrawApplicationServiceController(CosumableWithdawApplicationService consumableWithdrawApplicationService)
        {
            _cosumableWithdawApplicationService = consumableWithdrawApplicationService;
        }

        [Authorize(Policy = "RequireWorkshopSupervisor")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var data = await _cosumableWithdawApplicationService.GetAllConsumableWithdrawApplications();
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
                var data = await _cosumableWithdawApplicationService.GetConsumableWithdrawApplicationByID(id);
                if (data == null)
                    return NotFound(new { message = "Consumable withdraw application not found." });

                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Policy = "RequireWorkshopSupervisor")]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Consumableswithdrawapplication order)
        {
            try
            {
                await _cosumableWithdawApplicationService.AddConsumableWithdrawApplicationAsync(order);
                return Ok(new { message = "Consumable withdraw application added successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Policy = "RequireWorkshopSupervisor")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            try
            {
                await _cosumableWithdawApplicationService.DeleteWithdrawOrder(id);
                return Ok(new { message = "Consumable withdraw application deleted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Policy = "RequireWorkshopSupervisor")]
        [HttpPut]
        public async Task<IActionResult> UpdateOrder([FromBody] Consumableswithdrawapplication application)
        {
            try
            {
                await _cosumableWithdawApplicationService.UpdateConsumableWithdrawAsync(application);
                return Ok(new { message = "Consumable withdraw application updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
