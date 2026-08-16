using HiSubmit.Application.Features.Brands.Commands.AddEdit;
using HiSubmit.Application.Features.Brands.Commands.Delete;
using HiSubmit.Application.Features.Brands.Queries.Export;
using HiSubmit.Application.Features.Brands.Queries.GetAll;
using HiSubmit.Application.Features.Brands.Queries.GetById;
using Hisubmit.Client.SharedModels.Contracts.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Web.Controllers.v1.Catalog;

public class ArtCategoryController : BaseApiController<ArtCategoryController>
{
    /// <summary>
    /// Get All ArtCategories
    /// </summary>
    /// <returns>Enable 200 OK</returns>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var brands = await Mediator.Send(new GetAllArtCategoryQuery());
        return Ok(brands);
    }

    /// <summary>
    /// Get a ArtCategory By Id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Enable 200 Ok</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var brand = await Mediator.Send(new GetBrandByIdQuery() { Id = id });
        return Ok(brand);
    }

    /// <summary>
    /// Create/Update a ArtCategory
    /// </summary>
    /// <param name="command"></param>
    /// <returns>Enable 200 OK</returns>
    [Authorize(Policy = Permissions.ArtCategory.Create)]
    [HttpPost]
    public async Task<IActionResult> Post(AddEditArtCategoryCommand command)
    {
        return Ok(await Mediator.Send(command));
    }

    /// <summary>
    /// Delete a ArtCategory
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Enable 200 OK</returns>
    [Authorize(Policy = Permissions.ArtCategory.Delete)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        return Ok(await Mediator.Send(new DeleteBrandCommand { Id = id }));
    }

    /// <summary>
    /// Search ArtCategories and Export to Excel
    /// </summary>
    /// <param name="searchString"></param>
    /// <returns></returns>
    [Authorize(Policy = Permissions.ArtCategory.Export)]
    [HttpGet("export")]
    public async Task<IActionResult> Export(string searchString = "")
    {
        return Ok(await Mediator.Send(new ExportBrandsQuery(searchString)));
    }
}
