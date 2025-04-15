//using DataAccessLayer.Repositories;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using System.Threading.Tasks;
//using ZstdSharp.Unsafe;

//namespace Move_Smart.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class SparePartPurchaseOrderController : ControllerBase
//    {
//        public readonly SparePartsPurchaseOrderRepo _sparepartpurchaseorder;
//        public SparePartPurchaseOrderController(SparePartsPurchaseOrderRepo sparepartspurchaseorder)
//        {
//            _sparepartpurchaseorder = sparepartspurchaseorder;
//        }
//        [HttpGet]
//        public async Task<IActionResult> GetAllSparePartPurchaseOrder() 
//        {
//            var data = await _sparepartpurchaseorder.GetAllSparePartsPurchaseOrder();
//            return Ok(data);
//        }
//        [HttpGet("{SparePartPurchaseOrderID}")]
//        public async Task<IActionResult> GetSparePartPurchaseOrderByID(int SparePartPurchaseOrderID)
//        {
//            var data = await _sparepartpurchaseorder.GetSparePartsPurchaseOrderByID(SparePartPurchaseOrderID);
//            return Ok(data);
//        }
//        [HttpPut]
//        public async Task<IActionResult> UpdateSparePartPurchaseOrder(Sparepartspurchaseorder order)
//        {
//             await _sparepartpurchaseorder.UpdateSparePartsPurchaseOrder(order);
//            return Ok();
//        }
//        [HttpDelete]
//        [Route("{ID}")]
//        public async Task<IActionResult> DeleteSparePart(int ID)
//        {
//            await _sparepartpurchaseorder.DeleteSparePartsPurchaseOrder(ID);
//            return Ok();
//        }
//        [HttpPost]
//        public async Task<IActionResult> AddSparePartPurchaseOrder(Sparepartspurchaseorder partpurchaseorder)
//        {
//            await _sparepartpurchaseorder.AddSparePartsPurchaseOrder(partpurchaseorder);
//            return Ok();
//        }
//    }
//}
