using BusinessLogicLayer.Services;
using DataAccessLayer.Repositories;
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
        [HttpGet]
        public async Task<IActionResult>  Get() 
        {
            var all = await _consumablespurchaseorderService.GetAllConsumablesPurchaseoOrder();
            return Ok(all);
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody]Consumablespurchaseorder order)
        {
            await _consumablespurchaseorderService.AddConsumablePurchaseOrderAsync(order);
            return Ok();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            await _consumablespurchaseorderService.DeleteConsumablePurchaseOrderAsync(id);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> UpdateOrder(Consumablespurchaseorder order)
        {
            await _consumablespurchaseorderService.UpdateConsumablePurchaseOrderAsync(order);
            return Ok();
        }
        [HttpGet("{ID}")]
        public async Task<IActionResult> GetByID(int ID)
        {
            var data = await _consumablespurchaseorderService.GetConsumablePurchaseOrderByID(ID);
            return Ok(data);
        }
    }
}
