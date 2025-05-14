using DataAccessLayer.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Move_Smart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SparePartController : ControllerBase
    {
        private readonly Sparepart _isparepart;

        public SparePartController(Sparepart isparepart)
        {
            _isparepart = isparepart;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllSparePart()
        {
            var data = await _isparepart.GetAllSparePart();
            return Ok(data);
        }
        [HttpGet("{PartName}")]
        public async Task<IActionResult> GetSparePartByName(int id)
        {
            var data = await _isparepart.GetSparePartByName(id);
            return Ok(data);
        }
        [HttpPost]
        public async Task<IActionResult> AddSparePart([FromBody] Sparepart spare)
        {
            try
            {
                await _isparepart.AddSparePart(spare);
                return Ok("Spare part added successfully.");
            }
            catch (Exception ex)
            {
                // Return only the error message
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateSparePart([FromBody] Sparepart spare)
        {
            try
            {
                await _isparepart.UpdateSparePart(spare);
                return Ok("Spare part updated successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteSparePart(int id)
        {
            try
            {
                await _isparepart.DeleteSparePart(id);
                return Ok("Spare part deleted successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("count")]
        public async Task<IActionResult> Count()
        {
            var count = await _isparepart.CountAllOrdersAsync();
            return Ok(count);
        }
        //[HttpPut("{PartName}")]
        //public async Task<IActionResult> UpdateByNameSparePart(string PartName,[FromBody] Sparepart spare)
        //{
        //    await _isparepart.UpdateByNameSparePart(PartName, spare);
        //    return Ok();
        //}

    }
}
