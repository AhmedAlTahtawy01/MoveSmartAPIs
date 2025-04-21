using DataAccessLayer.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Move_Smart.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehicleConsumableController : ControllerBase
    {
        private readonly Vehicleconsumable _vehicleconsumable;
        public VehicleConsumableController(Vehicleconsumable vehicleconsumable)
        {
            _vehicleconsumable = vehicleconsumable;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllVehicleConsumable()
        {
            var data = await _vehicleconsumable.GetAllVehicleConsumable();
            return Ok(data);
        }
        [HttpGet("{ConsumableName}")]
        public async Task<IActionResult> GetSparePartByName(string ConsumableName)
        {
            var data = await _vehicleconsumable.GetVehicleConsumableByName(ConsumableName);
            return Ok(data);
        }
        [HttpPost]
        public async Task<IActionResult> AddVehicleConsumable([FromBody] Vehicleconsumable consume)
        {
            await _vehicleconsumable.AddVehicleConsumable(consume);
            return Ok();
        }
        [HttpDelete]
        [Route("{ConsumableName}")]
        public async Task<IActionResult> DeleteVehicleConsumable(string ConsumableName)
        {
            await _vehicleconsumable.DeleteVehicleConsumable(ConsumableName);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> UpdateSparePart([FromBody] Vehicleconsumable consume)
        {
            await _vehicleconsumable.UpdateVehicleConsumable(consume);
            return Ok();
        }
        [HttpPut("{part}")]
        public async Task<IActionResult> UpdateVehicleConumable([FromRoute]string part, [FromBody] Vehicleconsumable consume)
        {
            await _vehicleconsumable.UpdateConsumableAsynchronously(part, consume);
            return Ok();
        }
    }
}
