using HiSubmit.Application.Features.Payments.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using HiSubmit.Application.Features.FestivalPaymentItems.Queries.FestivalPaymentStates;
using HiSubmit.Application.Features.FestivalPaymentItems.Queries.GetAll;
using HiSubmit.Application.Features.FestivalPaymentsInformation.Commands.AddEdit;
using HiSubmit.Application.Features.FestivalPaymentsInformation.Queries.GetDetail;
using HiSubmit.Application.Features.Wrapper;
using HiSubmit.Application.Features.Settlements;
using Hisubmit.Client.SharedModels.Contracts.Permission;
using Web.Filters;

namespace Web.Controllers.v1.Festival;

public class FestivalPaymentsController : BaseFestivalController<FestivalPaymentsController>
{
    [HttpGet("SettlementStatements")]
    [FestivalAuthentication(Policy = Permissions.FestivalPayment.CartItem)]
    public async Task<IActionResult> GetSettlementStatements(
        [FromQuery] GetFestivalSettlementStatementsRequest query, int festivalId)
    {
        query.FestivalId = festivalId;
        return Ok(await Mediator.Send(query));
    }

    [HttpPost("SettlementStatements")]
    [FestivalAuthentication(Policy = Permissions.FestivalPayment.CartItem)]
    public async Task<IActionResult> CreateSettlementStatement(
        CreateFestivalSettlementStatementRequest command, int festivalId)
    {
        command.FestivalId = festivalId;
        return Ok(await Mediator.Send(command));
    }

    [HttpPost("SettlementStatements/{statementId:int}/adjustments")]
    [FestivalAuthentication(Policy = Permissions.FestivalPayment.CartItem)]
    public async Task<IActionResult> AddSettlementAdjustment(
        AddSettlementAdjustmentRequest command, int festivalId, int statementId)
    {
        command.FestivalId = festivalId;
        command.StatementId = statementId;
        return Ok(await Mediator.Send(command));
    }

    [HttpPost("SettlementStatements/{statementId:int}/status")]
    [FestivalAuthentication(Policy = Permissions.FestivalPayment.CartItem)]
    public async Task<IActionResult> UpdateSettlementStatus(
        UpdateSettlementStatusRequest command, int festivalId, int statementId)
    {
        command.FestivalId = festivalId;
        command.StatementId = statementId;
        return Ok(await Mediator.Send(command));
    }

    [HttpGet("SettlementStatements/{statementId:int}/export")]
    [FestivalAuthentication(Policy = Permissions.FestivalPayment.CartItem)]
    public async Task<IActionResult> ExportSettlementStatement(
        [FromQuery] ExportFestivalSettlementRequest query,
        int festivalId, int statementId)
    {
        query.FestivalId = festivalId;
        query.StatementId = statementId;
        var result = await Mediator.Send(query);
        if (!result.Succeeded || result.Data is null)
            return Ok(result);
        return File(result.Data.File, result.Data.MimeType, result.Data.FileName);
    }

    /// <summary>
    /// Get  festival Cart item such as product , ticket , submit 
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet("CartItems")]
    [FestivalAuthentication(Policy = Permissions.FestivalPayment.CartItem)]
    public async Task<IActionResult> GetFestivalCartItem([FromQuery] GetAllCartItemQuery query, int festivalId)
    {
        query.FestivalId = festivalId;
        query.Type = GetCartItemQueryType.Festival;
        return Ok(await Mediator.Send(query));
    }

    /// <summary>
    /// update festival payment information (card number, expires ,... or for paypal give email address )
    /// </summary>
    /// <param name="command"></param>
    /// <param name="festivalId"></param>
    /// <returns></returns>
    [HttpPost("UpdatePaymentInformation")]
    [FestivalAuthentication(Policy = Permissions.FestivalPayment.PaymentInformation)]
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
    [FestivalAuthentication(Policy = Permissions.FestivalPayment.PaymentInformation)]
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
    [FestivalAuthentication(Policy = Permissions.FestivalPayment.CartItem)]
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
    [FestivalAuthentication(Policy = Permissions.FestivalPayment.CartItem)]
    public async Task<IActionResult> GetDemandFestival([FromQuery] GetFestivalPaymentStateQuery query, int festivalId)
    {
        query.FestivalId = festivalId;
        return Ok(await Mediator.Send(query));
    }
    
}

