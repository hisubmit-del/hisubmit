using System.Threading.Tasks;
using HiSubmit.Application.Features.Products.Commands.AddEdit;
using HiSubmit.Application.Features.Products.Commands.Delete;
using HiSubmit.Application.Features.Products.Commands.Enable;
using HiSubmit.Application.Features.Products.Queries.GetAllPaged;
using HiSubmit.Application.Features.Products.Queries.GetProductImage;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Ocsp;

namespace HiSubmit.Server.Controllers.v1.Admin;

public class ProductController : BaseAdminController<ProductController>
{
    /// <summary>
    /// Get All Products
    /// </summary>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <param name="searchString"></param>
    /// <param name="festivalId"></param>
    /// <param name="orderBy"></param>
    /// <returns>Enable 200 OK</returns>
    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll([FromQuery]GetAllProductsQuery query)
    {
        var products = await Mediator.Send(query);
        return Ok(products);
    }

    /// <summary>
    /// Get a Product Image by Id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Enable 200 OK</returns>
    [HttpGet("image/{id}")]
    public async Task<IActionResult> GetProductImageAsync(int id)
    {
        var result = await Mediator.Send(new GetProductImageQuery(id));
        return Ok(result);
    }


    [HttpPost("updateEnable")]
    public async Task<IActionResult> UpdateEnable(EnableProductCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(result);
    }
    // /// <summary>
    // /// Add/Edit a Product
    // /// </summary>
    // /// <param name="request"></param>
    // /// <param name="festivalId"></param>
    // /// <returns>Enable 200 OK</returns>
    // [HttpPost("Update")]
    // public async Task<IActionResult> Post(AddEditProductRequest request, int festivalId)
    // {
    //     return Ok(await Mediator.Send(request));
    // }

    // /// <summary>
    // /// Delete a Product
    // /// </summary>
    // /// <param name="id"></param>
    // /// <returns>Enable 200 OK response</returns>
    // [HttpDelete("Delete/{id}")]
    // public async Task<IActionResult> Delete(int id)
    // {
    //     return Ok(await Mediator.Send(new DeleteProductCommand { Id = id }));
    // }

}