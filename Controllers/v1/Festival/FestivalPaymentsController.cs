using HiSubmit.Application.Features.Payments.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using HiSubmit.Application.Features.FestivalPaymentItems.Queries.FestivalPaymentStates;
using HiSubmit.Application.Features.FestivalPaymentItems.Queries.GetAll;
using HiSubmit.Application.Features.FestivalPaymentsInformation.Commands.AddEdit;
using HiSubmit.Application.Features.FestivalPaymentsInformation.Queries.GetDetail;
using HiSubmit.Application.Features.Wrapper;

namespace Web.Controllers.v1.Festival;

public class FestivalPaymentsController : BaseFestivalController<FestivalPaymentsController>
{

    /// <summary>
    /// Get  festival Cart item such as product , ticket , submit 
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet("CartItems")]
    public async Task<IActionResult> GetFestivalCartItem([FromQuery] GetAllCartItemQuery query, int festivalId)
    {
        query.FestivalId = festivalId;
        return Ok(await Mediator.Send(query));
    }

    /// <summary>
    /// update festival payment information (card number, expires ,... or for paypal give email address )
    /// </summary>
    /// <param name="command"></param>
    /// <param name="festivalId"></param>
    /// <returns></returns>
    [HttpPost("UpdatePaymentInformation")]
    public async Task<IActionResult> UpdatePaymentInformation(AddEditFestivalPaymentInformationCommand command, int festivalId)
    {
        command.FestivalId = festivalId;
        return Ok(await Mediator.Send(command));
    }

    /// <summary>
    /// Get ProductFestivalId Payment Information such as card number or paypal email address 
    /// </summary>
    /// <param name="query"></param>
    /// <param name="festivalId"></param>
    /// <returns></returns>
    [HttpGet("GetPaymentInformation")]
    public async Task<IActionResult> GetPaymentInformation([FromQuery] GetFestivalPaymentInformationDetailQuery query, int festivalId)
    {
        query.FestivalId = festivalId;
        return Ok(await Mediator.Send(query));
    }
    
    /// <summary>
    /// Get All ProductFestivalId Payment Factors
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet("AllFestivalPaymentItems")]
    public async Task<IActionResult> GetFestivalPaymentItems([FromQuery] GetAllFestivalPaymentItemQuery query, int festivalId)
    {
        query.FestivalId = festivalId;
        query.AccountType = RequestAccountType.Festival;
        return Ok(await Mediator.Send(query));
    }

    /// <summary>
    /// Get ProductFestivalId Debt 
    /// </summary>
    /// <param name="query"></param>
    /// <param name="festivalId"></param>
    /// <returns></returns>
    [HttpGet("DemandFestival")]
    public async Task<IActionResult> GetDemandFestival([FromQuery] GetFestivalPaymentStateQuery query, int festivalId)
    {
        query.FestivalId = festivalId;
        return Ok(await Mediator.Send(query));
    }
    
}

