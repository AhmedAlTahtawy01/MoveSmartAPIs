using DataAccessLayer.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Move_Smart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsumablePurchaseOrderController : ControllerBase
    {
        private readonly consumablespurchaseorderRepo _repo;
        public ConsumablePurchaseOrderController(consumablespurchaseorderRepo repo)
        {
            _repo = repo;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _repo.GetAllConsumablesPurchaseOrder();
            return Ok(data);
        }
        [HttpGet("{ID}")]
        public async Task<IActionResult> GetByID(int id)
        {
            var data = await _repo.GetConsumablesPurchaseOrderByID(id);
            return Ok(data);
        }
        [HttpDelete("{ID}")]
        public async Task<IActionResult> DeleteOrder(int ID)
        {
            await _repo.DeleteConsumablesPurchaseOrder(ID);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Consumablespurchaseorder order)
        {
            await _repo.UpdateConsumablesPurchaseOrder(order);
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Consumablespurchaseorder order)
        {
            await _repo.AddConsumablesPurchaseOrder(order);
            return Ok();
        }


    }
}
