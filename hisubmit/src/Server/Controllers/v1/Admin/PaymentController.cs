using System.Threading.Tasks;
using HiSubmit.Application.Features.FestivalPaymentItems.Commands.Add;
using HiSubmit.Application.Features.FestivalPaymentItems.Queries.FestivalPaymentStates;
using HiSubmit.Application.Features.FestivalPaymentItems.Queries.GetAll;
using HiSubmit.Application.Features.FestivalPaymentsInformation.Queries.GetAll;
using HiSubmit.Application.Features.FestivalPaymentsInformation.Queries.GetDetail;
using HiSubmit.Application.Features.Payments.Commands;
using HiSubmit.Application.Features.Payments.Queries;
using Microsoft.AspNetCore.Mvc;

namespace HiSubmit.Server.Controllers.v1.Admin;

public class PaymentController : BaseAdminController<PaymentController>
{
   /// <summary>
   /// All cart item such as service fee product ticket and ...
   /// </summary>
   /// <param name="query"></param>
   /// <returns></returns>
   [HttpPost("Items")]
   public async Task<IActionResult> GetAllCartItems(GetAllCartItemQuery query)
   {
      query.Type = GetCartItemQueryType.Admin;
      return Ok(await Mediator.Send(query));
   }


   [HttpPost("Carts")]
   public async Task<IActionResult> GetAllCarts(GetAllCartsQuery query)
   {
      query.Type = GetCartItemQueryType.Admin;
      return Ok(await Mediator.Send(query));
   }

   /// <summary>
   /// Get All ProductFestivalId Payment Information
   /// </summary>
   /// <param name="query"></param>
   /// <returns></returns>
   [HttpGet("AllPaymentsInformation")]
   public async Task<IActionResult> GetFestivalPaymentsInformation(
      [FromQuery] GetAllFestivalPaymentInformationQuery query)
   {
      return Ok(await Mediator.Send(query));
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
   /// Add Payment Factor  for ProductFestivalId income
   /// </summary>
   /// <param name="command"></param>
   /// <returns></returns>
   [HttpPost("AddFestivalPaymentItem")]
   public async Task<IActionResult> AddFestivalPaymentItem(AddFestivalPaymentItemCommand command)
   {
      return Ok(await Mediator.Send(command));
   }

   
   /// <summary>
   /// Get All ProductFestivalId Payment Factors
   /// </summary>
   /// <param name="query"></param>
   /// <returns></returns>
   [HttpGet("AllFestivalPaymentItems")]
   public async Task<IActionResult> GetFestivalPaymentItems([FromQuery] GetAllFestivalPaymentItemQuery query)
   {
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

   [HttpPost("DownloadCartFactor")]
   public async Task<IActionResult> Download(DownloadCartFactorCommand command)
   {
      return Ok(await Mediator.Send(command));
   }
}
