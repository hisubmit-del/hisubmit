using HiSubmit.Application.Features.Products.Commands.AddEdit;
using HiSubmit.Application.Features.Products.Commands.Delete;
using HiSubmit.Application.Features.Products.Queries.Export;
using HiSubmit.Application.Features.Products.Queries.GetAllPaged;
using HiSubmit.Application.Features.Products.Queries.GetProductImage;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using HiSubmit.Application.Features.Products.Queries.GetById;
using Hisubmit.Client.SharedModels.Contracts.Permission;
using Web.Filters;

namespace Web.Controllers.v1.Catalog;

public class ProductsController : BaseFestivalController<ProductsController>
{
    /// <summary>
    /// Get All Products
    /// </summary>
    /// <param name="festivalId"></param>
    /// <returns>Enable 200 OK</returns>
    
    [HttpPost("GetAll")]
    [FestivalAuthentication(Policy = Permissions.FestivalProducts.View)]
    public async Task<IActionResult> GetAll(GetAllProductsQuery query,int festivalId)
        //(int pageNumber, int pageSize, string searchString, int? festivalId,string orderBy = null)
    {
        query.FestivalId=festivalId;
        var products = 
            await Mediator.Send(query);
        return Ok(products);
    }

    /// <summary>
    /// Get a Product Image by Id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Enable 200 OK</returns>
    [HttpGet("image/{id}")]
    [FestivalAuthentication(Policy = Permissions.FestivalProducts.View)]
    public async Task<IActionResult> GetProductImageAsync(int id)
    {
        var result = await Mediator.Send(new GetProductImageQuery(id));
        return Ok(result);
    }

    /// <summary>
    /// Add/Edit a Product
    /// </summary>
    /// <param name="command"></param>
    /// <param name="festivalId"></param>
    /// <returns>Enable 200 OK</returns>
    [HttpPost("Update")]
    [FestivalAuthentication(Policy = Permissions.FestivalProducts.Edit)]
    public async Task<IActionResult> Post(AddEditProductCommand command, int festivalId)
    {
        command.FestivalId = festivalId;
        return Ok(await Mediator.Send(command));
    }

    /// <summary>
    /// Delete a Product
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Enable 200 OK response</returns>
    [HttpDelete("Delete/{id}")]
    [FestivalAuthentication(Policy = Permissions.FestivalProducts.Edit)]
    public async Task<IActionResult> Delete(int id)
    {
        return Ok(await Mediator.Send(new DeleteProductCommand { Id = id }));
    }

    /// <summary>
    /// Search Products and Export to Excel
    /// </summary>
    /// <param name="searchString"></param>
    /// <returns>Enable 200 OK</returns>
    [HttpGet("export")]
    [FestivalAuthentication(Policy = Permissions.FestivalProducts.View)]
    public async Task<IActionResult> Export(string searchString = "")
    {
        return Ok(await Mediator.Send(new ExportProductsQuery(searchString)));
    }
    
    
    [HttpGet("Get")]
    [FestivalAuthentication(Policy = Permissions.FestivalProducts.View)]
    public async Task<IActionResult> Get([FromQuery] GetProductByIdQuery query)
    {
        return Ok(await Mediator.Send(query));
    }
}
