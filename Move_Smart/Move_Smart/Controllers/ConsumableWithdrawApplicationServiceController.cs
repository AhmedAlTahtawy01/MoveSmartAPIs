using BusinessLogicLayer.Services;
using DataAccessLayer.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.AccessControl;

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
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _cosumableWithdawApplicationService.GetAllConsumableWithdrawApplications();
            return Ok(data);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByID(int id)
        {
            var data = await _cosumableWithdawApplicationService.GetConsumableWithdrawApplicationByID(id);
            return Ok(data);
        }
        [HttpPost]
        public async Task<IActionResult> Add(Consumableswithdrawapplication order)
        {
            await _cosumableWithdawApplicationService.AddConsumableWithdrawApplicationAsync(order);
            return Ok();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            await _cosumableWithdawApplicationService.DeleteWithdrawOrder(id);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> UpdateOrder(Consumableswithdrawapplication application)
        {
            await _cosumableWithdawApplicationService.UpdateConsumableWithdrawAsync(application);
            return Ok();
        }
    }
}
