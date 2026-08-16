using HiSubmit.Application.Features.Payments.Commands;
using HiSubmit.Application.Features.Payments.Queries;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using HiSubmit.Application.Features.Payments.DiscountsCodes.Queries;
using HiSubmit.Application.Features.Users.Commands.SpecialFee;
using HiSubmit.Application.Interfaces.Services;

namespace HiSubmit.Server.Controllers.v1.Payments;

public class CartController(ICurrentUserService currentUserService) : BaseApiController<CartController>
{
    private readonly ICurrentUserService _currentUserService = currentUserService;

    /// <summary>
    /// Get All Carts if TakeCurrentusercart is true return current user logined carts
    /// </summary>
    /// <param name="query">      
    /// </param>
    /// <returns></returns>
    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAllCarts([FromQuery] GetAllCartsQuery query)
    {
        query.TakeCurrentUserCarts = true;
        return Ok(await Mediator.Send(query));
    }

    /// <summary>
    /// Get cart item with cartId if cartId==null return current user open cart items
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet("GetItems")]
    public async Task<IActionResult> GetItems([FromQuery] GetUserOpenCartItemQuery query)
    {
        return Ok(await Mediator.Send(query));
    }

    /// <summary>
    /// Paid current user opened cart
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("PaidCart")]
    public async Task<IActionResult> PaidCart(PaidCartCommand command)
    {
        return Ok(await Mediator.Send(command));
    }


    [HttpPost("CheckPaidCart")]
    public async Task<IActionResult> CheckPaidCart(PaidCartCommand command)
    {
        return Ok(await Mediator.Send(command));
    }
        

    /// <summary>
    /// update account status to special account
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("SpecialAccountAddToCard")]
    public async Task<IActionResult> SpecialAccount(SpecialFeeCommand command)
    {
        return Ok(await Mediator.Send(command));
    }


    /// <summary>
    /// get special account Fee 
    /// </summary>
    /// <returns></returns>
    [HttpGet("SpecialAccountFee")]
    public async Task<IActionResult> GetSpecialAccountFee()
    {
        return Ok(await Mediator.Send(new GetSiteCommissionQuery()));
    }


    /// <summary>
    /// delete item from shopping cart if cart is dont paid
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpDelete("DeleteItem")]
    public async Task<IActionResult> DeleteItem([FromQuery] DeleteCartItemCommand command)
    {
        return Ok(await Mediator.Send(command));
    }

    //private static PayPalEnvironment GetEnvironment()
    //{
    //    return new SandboxEnvironment(
    //        clientId: "ASrg5BdD5zjbbOVTvFW9QG2brIt1zIoChtDZkg9pQNLB-ud89_rJa_B9TvbMumi75IK1le73P5AMEyuS",
    //        clientSecret: "EMFE42RdlFUNMUs6iHRWO1bnIbk1U0gqxy4DoaydXbL5ywoLhMyg-4vomvNvnvIpcaUZhe7aRJum5z_4");
    //}


    [HttpPost("DownloadCartFactor")]
    public async Task<IActionResult> Download(DownloadCartFactorCommand command)
    {
        return Ok(await Mediator.Send(command));
    }

    [HttpGet("PaidZeroCart")]
    public async Task<IActionResult> PaidZeroCommand([FromQuery]PaidZeroCartCommand command)
    {
        command.UserId = _currentUserService.UserId;
        return Ok(await Mediator.Send(command));
    }

    [HttpPost("CalculateDiscountCode")]
    public async Task<IActionResult> CalculateDiscountCode(CalculateDiscountCodeQuery query)
    {
        return Ok(await Mediator.Send(query));
    }
}
