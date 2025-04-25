using BusinessLogicLayer.Services;
using DataAccessLayer.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Move_Smart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SparePartWithdrawApplicationServiceController : ControllerBase
    {
        private readonly SparePartWithdrawApplicationService _sparePartWithdrawApplicationService;
        public SparePartWithdrawApplicationServiceController(SparePartWithdrawApplicationService sparePartWithdrawApplicationService)
        {
            _sparePartWithdrawApplicationService = sparePartWithdrawApplicationService;
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var data =await _sparePartWithdrawApplicationService.GetSparepartswithdrawapplicationByID(id);
            return Ok(data);

        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _sparePartWithdrawApplicationService.GetAllSparePartWithdrawApplication();
            return Ok(data);

        }
        [HttpPut]
        public async Task<IActionResult> UpdateSpareParetWithdrawApp([FromBody]Sparepartswithdrawapplication order)
        {
            await _sparePartWithdrawApplicationService.UpdateSparePartsWithdrawApplicationAsync(order);
            return Ok();

        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _sparePartWithdrawApplicationService.DeleteSparePartWithdrawAppliactoinOrderAsync(id);
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> Add(Sparepartswithdrawapplication order)
        {
            await _sparePartWithdrawApplicationService.AddConsumablePurchaseOrderAsync(order);
            return Ok();
        }
    }
}
