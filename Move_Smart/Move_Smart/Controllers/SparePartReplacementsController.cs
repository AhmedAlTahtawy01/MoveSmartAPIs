using DataAccessLayer.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Move_Smart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SparePartReplacementsController : ControllerBase
    {
        private readonly SparePartsReplacement _sparePartsReplacement;
        public SparePartReplacementsController(SparePartsReplacement sparePartsReplacement)
        {
            _sparePartsReplacement = sparePartsReplacement;

        }
        [HttpGet]
        public async Task<IActionResult> Getall()
        {
            var data = await _sparePartsReplacement.GetAllSparePartsReplacement();
            return Ok(data);

        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByID(int id)
        {
            var data = await _sparePartsReplacement.GetSparePartsReplacementByID(id);
            return Ok(data);

        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _sparePartsReplacement.DeleteSparePartsReplacement(id);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> Update(Sparepartsreplacement order)
        {
            await _sparePartsReplacement.UpdateSparePartsReplacement(order);
            return Ok();

        }
        [HttpPost]
        public async Task<IActionResult> Add(Sparepartsreplacement order)
        {
            await _sparePartsReplacement.AddSparePartsReplacement(order);
            return Ok();
        }
        [HttpGet("count")]
        public async Task<IActionResult> Count()
        {
            var count = await _sparePartsReplacement.CountAllOrdersAsync();
            return Ok(count);
        }

    }
}
