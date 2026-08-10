using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Interfaces.Services.Identity;
using Hisubmit.Client.SharedModels.Requests.Identity;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;

namespace HiSubmit.Application.Features.Users.Commands.VerifyAccount;

public class VerifyAccountCommand : VerificationCodeRequest, IRequest<IResult>;


public class VerifyAccountCommandHandler(IUserService userService)
    :IRequestHandler<VerifyAccountCommand,IResult>
{
    public async Task<IResult> Handle
        (VerifyAccountCommand request, CancellationToken cancellationToken)
    {
        var user = await userService.GetUserByEmailAddress(request.Email);
        var res = await userService.ConfirmEmailAsync(user, request.Code);
        return res;
    } 
}