using HiSubmit.Application.Features.DocumentTypes.Commands.AddEdit;
using HiSubmit.Application.Features.DocumentTypes.Commands.Delete;
using HiSubmit.Application.Features.DocumentTypes.Queries.Export;
using HiSubmit.Application.Features.DocumentTypes.Queries.GetAll;
using HiSubmit.Application.Features.DocumentTypes.Queries.GetById;
using Hisubmit.Client.SharedModels.Contracts.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HiSubmit.Server.Controllers.Utilities.Misc
{
    // [Route("api/[controller]")]
    // [ApiController]
    // public class DocumentTypesController : BaseApiController<DocumentTypesController>
    // {
    //     /// <summary>
    //     /// Get All Document Types
    //     /// </summary>
    //     /// <returns>Enable 200 OK</returns>
    //     [Authorize(Policy = Permissions.DocumentTypes.View)]
    //     [HttpGet]
    //     public async Task<IActionResult> GetAll()
    //     {
    //         var documentTypes = await Mediator.Send(new GetAllDocumentTypesQuery());
    //         return Ok(documentTypes);
    //     }
    //
    //     /// <summary>
    //     /// Get Document ItemType By Id
    //     /// </summary>
    //     /// <param name="id"></param>
    //     /// <returns>Enable 200 Ok</returns>
    //     [Authorize(Policy = Permissions.DocumentTypes.View)]
    //     [HttpGet("{id}")]
    //     public async Task<IActionResult> GetById(int id)
    //     {
    //         var documentType = await Mediator.Send(new GetDocumentTypeByIdQuery { Id = id });
    //         return Ok(documentType);
    //     }
    //
    //     /// <summary>
    //     /// Create/Update a Document ItemType
    //     /// </summary>
    //     /// <param name="request"></param>
    //     /// <returns>Enable 200 OK</returns>
    //     [Authorize(Policy = Permissions.DocumentTypes.Create)]
    //     [HttpPost]
    //     public async Task<IActionResult> Post(AddEditDocumentTypeCommand request)
    //     {
    //         return Ok(await Mediator.Send(request));
    //     }
    //
    //     /// <summary>
    //     /// Delete a Document ItemType
    //     /// </summary>
    //     /// <param name="id"></param>
    //     /// <returns>Enable 200 OK</returns>
    //     [Authorize(Policy = Permissions.DocumentTypes.Delete)]
    //     [HttpDelete("{id}")]
    //     public async Task<IActionResult> Delete(int id)
    //     {
    //         return Ok(await Mediator.Send(new DeleteDocumentTypeCommand { Id = id }));
    //     }
    //
    //     /// <summary>
    //     /// Search Document Types and Export to Excel
    //     /// </summary>
    //     /// <param name="searchString"></param>
    //     /// <returns></returns>
    //     [Authorize(Policy = Permissions.DocumentTypes.Export)]
    //     [HttpGet("export")]
    //     public async Task<IActionResult> Export(string searchString = "")
    //     {
    //         return Ok(await Mediator.Send(new ExportDocumentTypesQuery(searchString)));
    //     }
    // }
}