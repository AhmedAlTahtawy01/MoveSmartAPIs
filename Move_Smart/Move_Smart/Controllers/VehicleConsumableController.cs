using DataAccessLayer.Repositories;
using Microsoft.AspNetCore.Authorization;
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
        //[Authorize(Policy = "RequireGeneralSupervisor")]
        [HttpGet]
        public async Task<IActionResult> GetAllVehicleConsumable()
        {
            try
            {
                var data = await _vehicleconsumable.GetAllVehicleConsumable();

                // Check if no data was found and return an appropriate response
                if (data == null || !data.Any())
                {
                    return NotFound(new { message = "No vehicle consumables found." });
                }

                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
        //[Authorize(Policy = "RequireGeneralSupervisor")]
        [HttpGet("id/{id}")]
        public async Task<IActionResult> GetSparePartByID(int id)
        {
            var data = await _vehicleconsumable.GetVehicleConsumableByID(id);
            return Ok(data);
        }
        //[Authorize(Policy = "RequireGeneralSupervisor")]
        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetByName(string name )
        {
            var data = await _vehicleconsumable.GetVehicleConsumableByName(name);
            return Ok(data);
        }
        //[Authorize(Policy = "WorkshopSupervisor")]
        [HttpPost]
        public async Task<IActionResult> AddVehicleConsumable([FromBody] Vehicleconsumable consume)
        {
            try
            {
                await _vehicleconsumable.AddVehicleConsumable(consume);
                return Ok(new { message = "Vehicle consumable added successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        //[Authorize(Policy = "RequireGeneralSupervisor")]
        [HttpGet("count")]
        public async Task<IActionResult> Count()
        {
            var count = await _vehicleconsumable.CountAllOrdersAsync();
            return Ok(new { message = count });
        }
        [Authorize(Policy = "GeneralSupervisor")]
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteVehicleConsumable(int id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new Exception("Consumable ID must be greater than zero.");
                }

                await _vehicleconsumable.DeleteVehicleConsumable(id);
                return Ok(new { message = "Vehicle consumable deleted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        //[Authorize(Policy = "WorkshopSupervisor")]
        [HttpPut]
        public async Task<IActionResult> UpdateSparePart([FromBody] Vehicleconsumable consume)
        {
            try
            {
                // Ensure the consume object is not null
                if (consume == null)
                {
                    throw new Exception("Consumable data cannot be null.");
                }

                // Ensure the ConsumableId is valid
                if (consume.ConsumableId <= 0)
                {
                    throw new Exception("Consumable ID must be provided and greater than zero.");
                }

                // Call the update method
                await _vehicleconsumable.UpdateVehicleConsumable(consume);

                return Ok(new { message = "Vehicle consumable updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        

    }
}

