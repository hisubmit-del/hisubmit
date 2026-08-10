using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Application.Interfaces.Services.Identity;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Domain.Entities.Submitter;
using HiSubmit.Domain.Enums;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiSubmit.Application.Features.Chats.Queries;

public class GetAllContactQuery : IRequest<IResult<List<GetAllContactResponse>>>
{
    public ChatRequestUserType Type { get; set; }
    public string UserId { get; set; }
    public int? FestivalId { get; set; }
}

public class GetAllContactQueryHandler : IRequestHandler<GetAllContactQuery, IResult<List<GetAllContactResponse>>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork<int> _unitOfWork;
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public GetAllContactQueryHandler
        (IUnitOfWork<int> unitOfWork, IMapper mapper, 
            IUserService userService,
            ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
        _userService = userService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IResult<List<GetAllContactResponse>>> Handle(GetAllContactQuery request,
        CancellationToken cancellationToken)
    {
        var contacts = new List<GetAllContactResponse>();
        switch (request.Type)
        {
            case ChatRequestUserType.User:
                contacts = await GetAllUserContact(request.UserId);
                break;
            case ChatRequestUserType.Admin:
                contacts = await GetAllAdminContact();
                break;
            case ChatRequestUserType.Festival:
                contacts = await GetAllFestivalContact(request.FestivalId.Value);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return await Result<List<GetAllContactResponse>>.SuccessAsync(contacts);
    }

    private async Task<List<GetAllContactResponse>> GetAllUserContact(string userId)
    {
        var userFestivals = await _unitOfWork.Repository<FestivalSubUser>()
            .Entities
            .Where(p => p.UserId == userId)
            .Include(p => p.Festival)
            .Select(p => p.Festival)
            .ToListAsync();

        var submitFestival = await _unitOfWork.Repository<Submit>()
            .Entities
            .Where(p => p.Project.UserId == userId)
            .Select(p => p.Festival)
            .ToListAsync();


        var contacts = userFestivals.Select(festival => new GetAllContactResponse
            {
                FestivalId = festival.Id,
                FullName = festival.Name,
                ImageUrl = festival.LogoURL,
                ContactType = ContactType.Festival,
            })
            .ToList();

        contacts.AddRange(submitFestival.Select(festival => new GetAllContactResponse
            {
                FestivalId = festival.Id,
                FullName = festival.Name,
                ImageUrl = festival.LogoURL,
                ContactType = ContactType.Festival,
            })
            .ToList());
        contacts = contacts.Where(p => p.FestivalId != null && p.ContactType == ContactType.Festival)
            .DistinctBy(p => p.FestivalId).ToList();

        contacts.Add(GetAllContactResponse.GetAdminContact());
        return contacts;
    }

    private async Task<List<GetAllContactResponse>> GetAllFestivalContact(int festivalId)
    {
        var contacts = new List<GetAllContactResponse>();
        var festivalSubUsersId = await _unitOfWork.Repository<FestivalSubUser>()
            .Entities
            .Where(p => p.FestivalId == festivalId)
            .Select(p => p.UserId)
            .ToListAsync();

        var festivalSubUser = await _userService.GetAllAsync(festivalSubUsersId);

        contacts.AddRange(festivalSubUser.Data.Select(p => new GetAllContactResponse
        {
            UserId = p.Id,
            FullName = p.FullName,
            ImageUrl = p.ProfilePictureDataUrl,
            ContactType = ContactType.OtherSubUser,
        }));

        var artistsId = await _unitOfWork.Repository<Submit>()
            .Entities
            .Where(p => p.FestivalId == festivalId && p.SubmitStatus != SubmitStatus.DontPaid)
            .Include(p => p.Project)
            .Select(p => p.Project.UserId)
            .ToListAsync();
        
        var artistUser = await _userService.GetAllAsync(artistsId);
        contacts.AddRange(artistUser.Data.Select(p => new GetAllContactResponse
        {
            UserId = p.Id,
            FullName = p.FullName,
            ImageUrl = p.ProfilePictureDataUrl,
            ContactType = ContactType.OtherSubUser,
        }));
        
        contacts.Add(GetAllContactResponse.GetAdminContact());
        return contacts;
    }

    private async Task<List<GetAllContactResponse>> GetAllAdminContact()
    {
        var contacts = new List<GetAllContactResponse>();
        var userId = _currentUserService.UserId;
        var userRoles = await _userService.GetRolesAsync(userId);

        var allUsers = await _userService.GetAllAsync();

        var allFestivals = await _unitOfWork.Repository<Festival>()
            .Entities
            .Select(festival => new GetAllContactResponse
            {
                 FestivalId = festival.Id,
                 FullName = festival.Name,
                 ImageUrl = festival.LogoURL,
                 ContactType = ContactType.Festival,
            }).ToListAsync();
        contacts = allFestivals;
        contacts.AddRange(allUsers.Data.Select(user =>new GetAllContactResponse
        {
            UserId = user.Id,
            FullName = user.FullName,
            ImageUrl = user.ProfilePictureDataUrl,
            ContactType = ContactType.Actors,
        }));

        return contacts;
    }
}

public class GetAllContactResponse
{
    public string FullName { get; set; }
    public string ImageUrl { get; set; }
    public int? RoomId { get; set; }
    public ContactType ContactType { get; set; }
    public string UserId { get; set; }
    public int? FestivalId { get; set; }
    public  int? NotSeenCount { get; set; }


    public static GetAllContactResponse GetAdminContact()
    {
        return new GetAllContactResponse
        {
            ContactType = ContactType.Admin,
            ImageUrl = string.Empty,
            FullName = "Site Admin"
        };
    }
}

public enum ContactType
{
    Admin,
    Actors,
    Referee,
    Festival,
    OtherSubUser
}