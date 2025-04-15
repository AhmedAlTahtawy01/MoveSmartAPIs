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
        public async Task<IActionResult> GetSparePartByName(string PartName)
        {
            var data = await _isparepart.GetSparePartByName(PartName);
            return Ok(data);
        }
        [HttpPost]
        public async Task<IActionResult> AddSparePart([FromBody] Sparepart spare)
        {
            await _isparepart.AddSparePart(spare);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> UpdateSparePart([FromBody] Sparepart spare)
        {
            await _isparepart.UpdateSparePart(spare);
            return Ok();
        }
        [HttpDelete]
        [Route("{PartName}")]
        public async Task<IActionResult> DeleteSparePart(string PartName)
        {
            await _isparepart.DeleteSparePart(PartName);
            return Ok();
        }
        [HttpPut("{PartName}")]
        public async Task<IActionResult> UpdateByNameSparePart(string PartName, Sparepart spare)
        {
            await _isparepart.UpdateByNameSparePart(PartName, spare);
            return Ok();
        }

    }
}
