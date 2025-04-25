using BusinessLogicLayer.Services;
using DataAccessLayer.Repositories;
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
        [HttpGet]
        public async Task<IActionResult> Getall()
        {
            var data = await _consumablesreplacement.GetAllConsumablesReplacement();
            return Ok(data);

        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByID(int id)
        {
            var data = await _consumablesreplacement.GetConsumablesReplacementByID(id);
            return Ok(data);

        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _consumablesreplacement.DeleteAddConsumablesReplacement(id);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> Update(Consumablesreplacement consumablesreplacement)
        {
            await _consumablesreplacement.UpdateConsumablesReplacement(consumablesreplacement);
            return Ok();

        }
        [HttpPost]
        public async Task<IActionResult> Add(Consumablesreplacement consumablesreplacement)
        {
            await _consumablesreplacement.AddConsumablesReplacement(consumablesreplacement);
            return Ok();
        }
        [HttpGet("count")]
        public async Task<IActionResult> Count()
        {
            var count = await _consumablesreplacement.CountAllOrdersAsync();
            return Ok(count);
        }
    }
    }
