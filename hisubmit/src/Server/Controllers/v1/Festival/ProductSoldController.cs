using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HiSubmit.Application.Features.Wrapper;
using HiSubmit.Application.Features.SoldProducts.Queries;

namespace HiSubmit.Server.Controllers.v1.Festival;

public class ProductSoldController : BaseFestivalController<ProductSoldController>
{
    /// <summary>
    /// Get All Product Sold Of festival
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet("getAll")]
    public async Task<IActionResult> GetAll([FromQuery] GetAllSoldProductQuery query, int festivalId)
    {
        query.RequestAccountType = RequestAccountType.Festival;
        query.FestivalId = festivalId;
        return Ok(await Mediator.Send(query));
    }

    /// <summary>
    /// Get Product Detail Such as product name
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet("detail")]
    public async Task<IActionResult> GetById([FromQuery] GetSoldProductDetailQuery query, int festivalId)
    {
        query.FestivalId = festivalId;
        return Ok(await Mediator.Send(query));
    }
}