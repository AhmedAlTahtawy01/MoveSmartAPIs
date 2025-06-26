using BusinessLogicLayer.Services;
using DataAccessLayer.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Move_Smart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SparePartPurchaseOrderServiceController : ControllerBase
    {
        private readonly SparePartPurchaseOrderService _sparePartPurchaseOrderService;

        public SparePartPurchaseOrderServiceController(SparePartPurchaseOrderService sparePartPurchaseOrderService)
        {
            _sparePartPurchaseOrderService = sparePartPurchaseOrderService;
        }

        [Authorize(Policy = "RequireWorkshopSupervisor")]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody]Sparepartspurchaseorder partpurchaseorder)
        {
            try
            {
                await _sparePartPurchaseOrderService.AddSparePartsPurchaseOrder(partpurchaseorder);
                return Ok(new { message = "Spare part purchase order added successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Policy = "RequireWorkshopSupervisor")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var data = await _sparePartPurchaseOrderService.GetAllSparePartPurchaseOrder();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Policy = "RequireWorkshopSupervisor")]
        [HttpDelete("{orderId}")]
        public async Task<IActionResult> DeleteSparePartsPurchaseOrder(int orderId)
        {
            try
            {
                await _sparePartPurchaseOrderService.DeleteSparePartsPurchaseOrderAsync(orderId);
                return Ok(new { message = "Spare part purchase order and linked application deleted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Policy = "RequireWorkshopSupervisor")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Upadate(int id, [FromBody] Sparepartspurchaseorder order)
        {
            try
            {
                await _sparePartPurchaseOrderService.UpdateSparePartsPurchaseOrderAsync(id, order);
                return Ok(new { message = "Spare part purchase order and linked application updated successfully." });
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
                var data = await _sparePartPurchaseOrderService.GetSparePartPurchaseOrderByID(ID);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("count")]
        public async Task<IActionResult> CountOrders()
        {
            try
            {
                var count = await _sparePartPurchaseOrderService.CountAllSparePartPurchaseOrdersAsync();
                return Ok(new { message = count });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
