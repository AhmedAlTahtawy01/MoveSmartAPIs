using BusinessLogicLayer.Services;
using DataAccessLayer.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mysqlx.Crud;

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
        [HttpPost]
        public async Task<IActionResult> Add(Sparepartspurchaseorder partpurchaseorder)
        {
            await _sparePartPurchaseOrderService.AddSparePartsPurchaseOrder(partpurchaseorder);
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _sparePartPurchaseOrderService.GetAllSparePartPurchaseOrder();
            return Ok(data);
        }
        [HttpDelete("{orderId}")]
        public async Task<IActionResult> DeleteSparePartsPurchaseOrder(int orderId)
        {
                await _sparePartPurchaseOrderService.DeleteSparePartsPurchaseOrderAsync(orderId);
                return Ok(new { message = "Spare part purchase order and linked application deleted successfully." });
            
            
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Upadate(int id , [FromBody] Sparepartspurchaseorder order)
        {
            await _sparePartPurchaseOrderService.UpdateSparePartsPurchaseOrderAsync(id, order);
            return Ok(new { message = "Spare part purchase order and linked application updated successfully." });
        }
        [HttpGet("{ID}")]
        public async Task<IActionResult> GetByID(int ID)
        {
            var data = await _sparePartPurchaseOrderService.GetSparePartPurchaseOrderByID(ID);
            return Ok(data);
        }
        [HttpGet("count")]
        public async Task<IActionResult> CountOrders()
        {
            var count = await _sparePartPurchaseOrderService.CountAllSparePartPurchaseOrdersAsync();
            return Ok(count);
        }

    }
}
