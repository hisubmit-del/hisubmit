using HiSubmit.Application.Features.Documents.Commands.AddEdit;
using HiSubmit.Application.Features.Documents.Commands.Delete;
using HiSubmit.Application.Features.Documents.Queries.GetAll;
using HiSubmit.Application.Features.Documents.Queries.GetById;
using Hisubmit.Client.SharedModels.Contracts.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Web.Controllers.Utilities.Misc
{
    // [Route("api/[controller]")]
    // [ApiController]
    // public class DocumentsController : BaseApiController<DocumentsController>
    // {
    //     /// <summary>
    //     /// Get All Documents
    //     /// </summary>
    //     /// <param name="pageNumber"></param>
    //     /// <param name="pageSize"></param>
    //     /// <param name="searchString"></param>
    //     /// <returns>Enable 200 OK</returns>
    //     [Authorize(Policy = Permissions.Documents.View)]
    //     [HttpGet]
    //     public async Task<IActionResult> GetAll(int pageNumber, int pageSize, string searchString)
    //     {
    //         var docs = await Mediator.Send(new GetAllDocumentsQuery(pageNumber, pageSize, searchString));
    //         return Ok(docs);
    //     }
    //
    //     /// <summary>
    //     /// Get Document By Id
    //     /// </summary>
    //     /// <param name="id"></param>
    //     /// <returns>Enable 200 Ok</returns>
    //     [Authorize(Policy = Permissions.Documents.View)]
    //     [HttpGet("{id}")]
    //     public async Task<IActionResult> GetById(int id)
    //     {
    //         var document = await Mediator.Send(new GetDocumentByIdQuery { Id = id });
    //         return Ok(document);
    //     }
    //
    //     /// <summary>
    //     /// Add/Edit Document
    //     /// </summary>
    //     /// <param name="request"></param>
    //     /// <returns>Enable 200 OK</returns>
    //     [Authorize(Policy = Permissions.Documents.Create)]
    //     [HttpPost]
    //     public async Task<IActionResult> Post(AddEditDocumentCommand request)
    //     {
    //         return Ok(await Mediator.Send(request));
    //     }
    //
    //     /// <summary>
    //     /// Delete a Document
    //     /// </summary>
    //     /// <param name="id"></param>
    //     /// <returns>Enable 200 OK</returns>
    //     [Authorize(Policy = Permissions.Documents.Delete)]
    //     [HttpDelete("{id}")]
    //     public async Task<IActionResult> Delete(int id)
    //     {
    //         return Ok(await Mediator.Send(new DeleteDocumentCommand { Id = id }));
    //     }
    // }
}