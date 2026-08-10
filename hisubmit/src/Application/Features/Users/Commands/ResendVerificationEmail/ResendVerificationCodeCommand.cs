using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Interfaces.RenderView;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Application.Interfaces.Services.BackGroundJob;
using HiSubmit.Application.Interfaces.Services.Identity;
using Hisubmit.Client.SharedModels.Requests.Identity;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using HiSubmit.Application.Models.Emails;
using HiSubmit.Application.Requests.Mail;

namespace HiSubmit.Application.Features.Users.Commands.ResendVerificationEmail
{
    public class ResendVerificationCodeCommand : ResendVerificationCodeRequest, IRequest<IResult>;

    public class ResendVerificationCodeCommandHandler(
    IBackGroundJobService backGroundJobService,
        IMailService mailService,
    IRenderViewService renderViewService,
        IUserService userService) : IRequestHandler<ResendVerificationCodeCommand, IResult>
    {
        public async Task<IResult> Handle(ResendVerificationCodeCommand request, CancellationToken cancellationToken)
        {
            var userId = await userService.GetUserByEmailAddress(request.Email);
            var users = await userService.GetUser([userId]);
            if (users is not { Count: > 0 })
                return await Result.FailAsync("An error has occurred.");

            var user = users[userId];
            if (user.EmailConfirmed)
                return await Result.FailAsync("An error has occurred.");

            var model = new ConfirmedEmailModel()
            {
                FullName = users[userId].FullName,
                VerificationCode = users[userId].VerificationCode
            };

            var mainContent =
                await renderViewService.RenderViewToStringAsync("_ConfirmedEmail", model);

            var backJob = backGroundJobService.AddEnqueue(() =>
                mailService.SendAsync(new MailRequest()
                {
                    Body = mainContent,
                    To = users[userId].Email,
                    Subject = "Welcome To Hisubmit.com"
                }));

            return await Result.SuccessAsync();
        }
    }


}
