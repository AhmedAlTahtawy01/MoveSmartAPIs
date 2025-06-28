using BusinessLogicLayer.Services;
using DataAccessLayer.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Move_Smart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsumablePurchaseOrderServiceController : ControllerBase
    {
        private readonly ConsumablespurchaseorderService _consumablespurchaseorderService;

        public ConsumablePurchaseOrderServiceController(ConsumablespurchaseorderService consumablespurchaseorderService)
        {
            _consumablespurchaseorderService = consumablespurchaseorderService;
        }

        [Authorize(Policy = "RequireWorkshopSupervisor")]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var all = await _consumablespurchaseorderService.GetAllConsumablesPurchaseoOrder();
                return Ok(all);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Policy = "RequireWorkshopSupervisor")]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Consumablespurchaseorder order)
        {
            try
            {
                await _consumablespurchaseorderService.AddConsumablePurchaseOrderAsync(order);
                return Ok(new { message = "Consumable purchase order added successfully." });
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
                await _consumablespurchaseorderService.DeleteConsumablePurchaseOrderAsync(id);
                return Ok(new { message = "Consumable purchase order deleted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Policy = "RequireWorkshopSupervisor")]
        [HttpPut]
        public async Task<IActionResult> UpdateOrder([FromBody] Consumablespurchaseorder order)
        {
            try
            {
                await _consumablespurchaseorderService.UpdateConsumablePurchaseOrderAsync(order);
                return Ok(new { message = "Consumable purchase order updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Policy = "RequireWorkshopSupervisor")]
        [HttpGet("{ID}")]
        public async Task<IActionResult> GetByID(int ID)
        {
            try
            {
                var data = await _consumablespurchaseorderService.GetConsumablePurchaseOrderByID(ID);
                if (data == null)
                    return NotFound(new { message = "Consumable purchase order not found." });

                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Policy = "RequireWorkshopSupervisor")]
        [HttpGet("count")]
        public async Task<IActionResult> CountOrders()
        {
            try
            {
                var count = await _consumablespurchaseorderService.CountAllConsumablePurchaseOrdersAsync();
                return Ok( new { count });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
