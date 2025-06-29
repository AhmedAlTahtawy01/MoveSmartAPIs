using BusinessLogicLayer.Services;
using DataAccessLayer.Repositories;
using Microsoft.AspNetCore.Authorization;
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

        //[Authorize(Policy = "RequireWorkshopSupervisor")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var data = await _sparePartWithdrawApplicationService.GetSparepartswithdrawapplicationByID(id);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        //[Authorize(Policy = "RequireWorkshopSupervisor")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var data = await _sparePartWithdrawApplicationService.GetAllSparePartWithdrawApplication();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Policy = "RequireWorkshopSupervisor")]
        [HttpPut]
        public async Task<IActionResult> UpdateSpareParetWithdrawApp([FromBody] Sparepartswithdrawapplication order)
        {
            try
            {
                await _sparePartWithdrawApplicationService.UpdateSparePartsWithdrawApplicationAsync(order);
                return Ok(new { message = "Spare part withdraw application updated successfully." });
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
                await _sparePartWithdrawApplicationService.DeleteSparePartWithdrawAppliactoinOrderAsync(id);
                return Ok(new { message = "Spare part withdraw application deleted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Policy = "RequireWorkshopSupervisor")]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Sparepartswithdrawapplication order)
        {
            try
            {
                await _sparePartWithdrawApplicationService.AddConsumablePurchaseOrderAsync(order);
                return Ok(new { message = "Spare part withdraw application added successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
