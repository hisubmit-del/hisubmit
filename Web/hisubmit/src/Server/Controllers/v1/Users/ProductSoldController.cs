using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HiSubmit.Application.Features.Wrapper;
using HiSubmit.Application.Features.SoldProducts.Queries;
using HiSubmit.Application.Features.SoldProducts.Commands;
using Microsoft.AspNetCore.Authorization;

namespace HiSubmit.Server.Controllers.v1.Users;

public class ProductSoldController : BaseApiController<ProductSoldController>
{
    /// <summary>
    /// Get All Product Sold User
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet("getAll")]
    public async Task<IActionResult> GetAll([FromQuery] GetAllSoldProductQuery query)
    {
        query.RequestAccountType = RequestAccountType.User;
        return Ok(await Mediator.Send(query));
    }

    /// <summary>
    /// Get Product Detail Such as product name
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet("detail")]
    public async Task<IActionResult> GetById([FromQuery] GetSoldProductDetailQuery query)
    {
        return Ok(await Mediator.Send(query));
    }

    /// <summary>
    /// Add Product To Cart item
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("Add")]
    [Authorize]
    public async Task<IActionResult> Add(AddProductSoldCommand command)
    {
        return Ok(await Mediator.Send(command));
    }
}