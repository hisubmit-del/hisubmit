using AutoMapper;
using HiSubmit.Application.Events.FestivalAddUsers;
using HiSubmit.Application.Events.FestivalRegisteredUser;
using HiSubmit.Application.Events.Users;
using HiSubmit.Application.Exceptions;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Application.Interfaces.Services.Identity;
using HiSubmit.Application.Requests.Identity;
using HiSubmit.Application.Responses.Identity;
using HiSubmit.Client.SharedModels.Constants.Role;
using HiSubmit.Client.SharedModels.Wrapper;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core.Tokenizer;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.WebUtilities;

namespace HiSubmit.Application.Features.Users.Commands.Register
{
    public class RegisterUserCommand : RegisterRequest, IRequest<Result<string>>
    {
        public string Origin { get; set; }
        public bool IsFestivalUser { get; set; }
    }

    internal class RegisterUserCommandHandler(
        IUserService userService,
        IMediator mediator,
        IMapper mapper,
        IBaseUrlService baseUrlService,
        IStringLocalizer<RegisterUserCommandHandler> stringLocalizer,
        IUnitOfWork<int> unitOfWork,
        ICurrentUserService currentUserService)
        : IRequestHandler<RegisterUserCommand, Result<string>>
    {
        public async Task<Result<string>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            if (request.RegisterAsFestival)
            {
                var festivalNameExist = await unitOfWork.Repository<Festival>()
                    .Entities.AnyAsync(p=>p.Name==request.FestivalName,cancellationToken);
                if (festivalNameExist)
                {
                    return await Result<string>.FailAsync(stringLocalizer["The name of the festival is repetitive"]);
                }
            }
            if (request.IsFestivalUser && request.FestivalId == null)
            {
                throw new BadRequestException();
            }

            var registerRequest = mapper.Map<RegisterRequest>(request);
            var random = new Random();

            var verificationCode = random.Next(10000, 999999);
            registerRequest.VerificationCode = verificationCode.ToString();

            var result = (Result<RegisterUserResponse>)await userService
                .RegisterAsync(registerRequest, request.Origin);

            if (result.Succeeded)
            {
                await userService.AddToRoleAsync(result.Data.UserId, new List<string>() { RoleConstants.ArtistRole });
                if (request.RegisterAsFestival)
                {
                    //Add To role ProductFestivalId
                    await userService.AddToRoleAsync(result.Data.UserId, new List<string> { RoleConstants.FestivalRole });
                    await mediator.Publish(new FestivalUserRegisteredEvent()
                    {
                        FestivalName = request.FestivalName,
                        UserId = result.Data.UserId
                    }, cancellationToken);
                }

                if (request.IsFestivalUser)
                {
                    if (currentUserService.FestivalId != null)
                        await mediator.Publish(
                            new FestivalAddUserEvent(result.Data.UserId, currentUserService.FestivalId.Value),
                            cancellationToken);
                    await mediator.Publish(new FestivalRegisteredUserEvent()
                    {
                        Email = request.Email,
                        FullName = $"{request.FirstName} {request.LastName}",
                        FestivalId = request.FestivalId.Value,
                        Password = request.Password
                    }, cancellationToken);
                }

                else
                {
                    await mediator.Publish(new UserRegisteredEvent()
                    {
                        Email = request.Email,
                        VerificationUrl = $"{ baseUrlService.GetBaseUrl() }account/confirmEmail?userId={result.Data.UserId }&code={WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(result.Data.VerificationCode))}",
                        FullName = $"{request.FirstName} {request.LastName}"
                    }, cancellationToken);
                }

                return await Result<string>.SuccessAsync(stringLocalizer["Successfully Register user"]);
            }
            var messages = result.Messages.ToList();
            return await Result<string>.FailAsync(messages);
        }
    }
}
