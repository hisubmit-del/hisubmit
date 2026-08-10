using System.Threading.Tasks;
using HiSubmit.Application.Features.Submits.Queries.GetAllSubmitCategories;
using HiSubmit.Application.Features.Submits.Queries.GetAllSubmitsQueries;
using HiSubmit.Application.Features.Submits.Queries.GetSubmitDetail;
using HiSubmit.Application.Features.Submits.Queries.GetSubmitFormAnswers;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.v1.Festival;

public class FestivalSubmitsController : BaseFestivalController<FestivalSubmitsController>
{
    /// <summary>
    /// Get All ProductFestivalId submits with Filter
    /// </summary>
    /// <param name="query"></param>
    /// <param name="festivalId"></param>
    /// <returns></returns>
    [HttpGet("Submit")]
    public async Task<IActionResult> GetAll([FromQuery] GetAllSubmitsQuery query, int festivalId)
    {
        query.FestivalId = festivalId;
        return Ok(await Mediator.Send(query));
    }


    /// <summary>
    /// Get All Selected Categories by user on submit to festival 
    /// </summary>
    /// <param name="query"></param>
    /// <param name="festivalId"></param>
    /// <returns></returns>
    [HttpGet("SubmitCategories")]
    public async Task<IActionResult> GetAllSubmitCategories([FromQuery]GetAllSubmitCategoriesQuery query, int festivalId)
    {
        query.FestivalId = festivalId;
        return Ok(await Mediator.Send(query));
    }
    
    /// <summary>
    /// Get All Submit Form Answers
    /// </summary>
    /// <param name="query"></param>
    /// <param name="festivalId"></param>
    /// <returns></returns>
    [HttpGet("SubmitFormAnswers")]
    public async Task<IActionResult> GetAllSubmitForms([FromQuery]GetSubmitFormAnswersQuery query, int festivalId)
    {
        query.FestivalId = festivalId;
        return Ok(await Mediator.Send(query));
    }

    [HttpGet("SubmitDetail")]
    public async Task<IActionResult> GetSubmitDetail([FromQuery] GetSubmitDetailQuery query,int festivalId)
    {
        query.FestivalId = festivalId;
        return Ok(await Mediator.Send(query));
    }
}