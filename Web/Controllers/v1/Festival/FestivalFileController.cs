using HiSubmit.Application.Features.Festivals.Commands.AddEditFestivalFile;
using HiSubmit.Application.Features.Festivals.Commands.DeleteFestivalFile;
using HiSubmit.Application.Features.Festivals.Queries.GetAllFestivalFile;
using HiSubmit.Application.Features.Festivals.Queries.GetFestivalFileDetail;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Contracts.Permission;
using Web.Filters;

namespace Web.Controllers.v1.Festival;

public class FestivalFileController : BaseFestivalController<FestivalFileController>
{
    /// <summary>
    /// Get All ProductFestivalId Attached file
    /// </summary>
    /// <param name="query"></param>
    /// <returns>list of file </returns>
    [HttpGet("GetAll")]
    [FestivalAuthentication(Policy = Permissions.Festival.View)]
    public async Task<IActionResult> GetAll([FromQuery] GetAllFestivalFileQuery query)
    {
        return Ok(await Mediator.Send(query));
    }

    /// <summary>
    /// Get ProductFestivalId File Detail Such as name and type and File
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet("Detail")]
    [FestivalAuthentication(Policy = Permissions.Festival.View)]
    public async Task<IActionResult> GetDetail([FromQuery] GetFestivalFileDetailQuery query)
    {
        return Ok(await Mediator.Send(query));
    }

    /// <summary>
    /// Add Or Edit ProductFestivalId File
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("Update")]
    [FestivalAuthentication(Policy = Permissions.Festival.Edit)]
    public async Task<IActionResult> Update(AddEditFestivalFileCommand command)
    {
        return Ok(await Mediator.Send(command));
    }

    /// <summary>
    /// Delete festival file qith id delete data on database and file on directory
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpDelete("Delete")]
    [FestivalAuthentication(Policy = Permissions.Festival.Edit)]
    public async Task<IActionResult> Delete([FromQuery]DeleteFestivalFileCommand command)
    {
        return Ok(await Mediator.Send(command));
    }
}
