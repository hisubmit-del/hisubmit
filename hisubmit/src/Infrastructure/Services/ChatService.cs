using AutoMapper;
using HiSubmit.Application.Exceptions;
using HiSubmit.Application.Interfaces.Chat;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Application.Interfaces.Services.Identity;
using HiSubmit.Application.Models.Chat;
using HiSubmit.Application.Responses.Identity;
using HiSubmit.Infrastructure.Contexts;
using HiSubmit.Infrastructure.Models.Identity;
using HiSubmit.Client.SharedModels.Constants.Role;
using HiSubmit.Client.SharedModels.Wrapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HiSubmit.Application.Enums;
using HiSubmit.Domain.Enums;

namespace HiSubmit.Infrastructure.Services
{
    public class ChatService : IChatService
    {
        private readonly BlazorHeroContext _context;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IStringLocalizer<ChatService> _localize;

        public ChatService(
            BlazorHeroContext context,
            IMapper mapper,
            IUserService userService,
            IStringLocalizer<ChatService> localize)
        {
            _context = context;
            _mapper = mapper;
            _userService = userService;
            _localize = localize;
        }

        public async Task<Result<IEnumerable<ChatHistoryResponse>>> GetChatHistoryAsync(string userId, string contactId,
            bool forSiteAdmin, int? festivalId = null)
        {
            var response = await _userService.GetAsync(userId);
            if (response.Succeeded)
            {
                var user = response.Data;
                var userChatQuery = _context.ChatHistories
                    .Where(h =>
                        (h.FromUserId == userId && h.ToUserId == contactId) ||
                        (h.FromUserId == contactId && h.ToUserId == userId));

                if (forSiteAdmin)
                {
                    userChatQuery = _context.ChatHistories
                        .Where(h =>
                            ((h.AdminReceiver || h.AdminSender) && h.ToUserId == contactId) ||
                            (h.FromUserId == contactId && (h.AdminReceiver || h.AdminSender)));
                }

                if (festivalId != null && festivalId != 0)
                {
                    userChatQuery = _context.ChatHistories.Where(h =>
                        ((h.FromFestivalId == festivalId || h.ToFestivalId == festivalId) && h.ToUserId == contactId) ||
                        (h.FromUserId == contactId &&
                         (h.FromFestivalId == festivalId || h.ToFestivalId == festivalId)));
                }

                var query = await userChatQuery
                    .OrderBy(a => a.CreatedDate)
                    .Include(a => a.FromUser)
                    .Include(a => a.ToUser)
                    .Select(x => new ChatHistoryResponse
                    {
                        FromUserId = x.FromUserId,
                        FromUserFullName = $"{x.FromUser.FirstName} {x.FromUser.LastName}",
                        Message = x.Message,
                        CreatedDate = x.CreatedDate,
                        AdminSender = x.AdminSender,
                        FromFestivalId = x.FromFestivalId,
                        Id = x.Id,
                        ToUserId = x.ToUserId,
                        ToUserFullName = $"{x.ToUser.FirstName} {x.ToUser.LastName}",
                        ToUserImageURL = x.ToUser.ProfilePictureDataUrl,
                        FromUserImageURL = x.FromUser.ProfilePictureDataUrl
                    }).ToListAsync();
                return await Result<IEnumerable<ChatHistoryResponse>>.SuccessAsync(query);
            }
            else
            {
                throw new ApiException(_localize["User Not Found!"]);
            }
        }

        public async Task<Result<IEnumerable<ChatUserResponse>>> GetAdminChatUsersAsync(string userId)
        {
            var userRoles = await _userService.GetRolesAsync(userId);
            var userIsAdmin =
                userRoles.Data?.UserRoles?.Any(x => x.Selected && x.RoleName == RoleConstants.AdministratorRole) ==
                true;
            var allUsers = await _context.Users
                .Where(user => user.Id != userId && (userIsAdmin || user.IsActive && user.EmailConfirmed))
                .ToListAsync();

            var allFestivals = await _context.Festivals
                .Select(festival => new ChatUserResponse
                {
                    UserName = festival.Name,
                    FirstName = festival.Name,
                    Type = ChatUserType.Festival,
                    ToFestivalId = festival.Id,
                    EmailAddress = festival.Email,
                    ProfilePictureDataUrl = festival.LogoURL,
                }).ToListAsync();
            
            var chatUsers = _mapper.Map<IEnumerable<ChatUserResponse>>(allUsers);
            chatUsers = chatUsers.Union(allFestivals);
            return await Result<IEnumerable<ChatUserResponse>>.SuccessAsync(chatUsers);
        }

        public async Task<IResult> SaveMessageAsync(ChatHistory<IChatUser> message)
        {
            if (string.IsNullOrWhiteSpace(message.ToUserId))
            {
                message.ToUser = await _context.Users.Where(user => user.Id == message.ToUserId).FirstOrDefaultAsync();
            }

            await _context.ChatHistories.AddAsync(_mapper.Map<ChatHistory<BlazorHeroUser>>(message));
            await _context.SaveChangesAsync();
            return await Result.SuccessAsync();
        }


        public async Task<Result<IEnumerable<ChatUserResponse>>> GetFestivalChatUserAsync(int festivalId)
        {
            var festivalSubUsersId = await _context.FestivalSubUser.Where(p => p.FestivalId == festivalId)
                .Select(p => p.UserId).ToListAsync();
            var festivalSubUser = await _context.Users.Where(user => festivalSubUsersId.Any(id => id == user.Id))
                .ToListAsync();
            var subUserChatResponse = _mapper.Map<IEnumerable<ChatUserResponse>>(festivalSubUser);
            foreach (var subUser in subUserChatResponse)
            {
                subUser.Type = ChatUserType.FestivalSubUser;
            }

            var artistsId = await _context.Submits
                .Where(p => p.FestivalId == festivalId && p.SubmitStatus != SubmitStatus.DontPaid)
                .Include(p => p.Project)
                .Select(p => p.Project.UserId)
                .ToListAsync();

            var artistUser =
                await _context.Users.Where(user => artistsId.Any(id => id == user.Id)).ToListAsync();
            var artistChatUsers = _mapper.Map<IEnumerable<ChatUserResponse>>(artistUser);


            var chatUsers = artistChatUsers.Union(subUserChatResponse).ToList();
            chatUsers = AddAdminPrivate(chatUsers).ToList();
            return await Result<IEnumerable<ChatUserResponse>>.SuccessAsync(chatUsers);
        }

        public async Task<Result<IEnumerable<ChatUserResponse>>> GetUserChatUsersAsync(string userId)
        {
            var userFestivals = await _context.FestivalSubUser
                .Where(p => p.UserId == userId)
                .Include(p => p.Festival)
                .Select(p => p.Festival)
                .ToListAsync();


            var submitFestival = await _context.Submits
                .Where(p => p.Project.UserId == userId)
                .Select(p => p.Festival)
                .ToListAsync();

            var chatResponse = userFestivals.Select(festival => new ChatUserResponse()
                {
                    UserName = festival.Name,
                    FirstName = festival.Name,
                    Type = ChatUserType.Festival,
                    FromFestivalId = festival.Id,
                    EmailAddress = festival.Email,
                    ProfilePictureDataUrl = festival.LogoURL,
                })
                .ToList();

            chatResponse.AddRange(submitFestival.Select(festival => new ChatUserResponse()
                {
                    UserName = festival.Name,
                    FirstName = festival.Name,
                    ToFestivalId = festival.Id,
                    Type = ChatUserType.Festival,
                    EmailAddress = festival.Email,
                    ProfilePictureDataUrl = festival.LogoURL,
                })
                .ToList());
            chatResponse = chatResponse.Where(p => p.ToFestivalId != null).DistinctBy(p => p.ToFestivalId).ToList();

            chatResponse = AddAdminPrivate(chatResponse).ToList();

           
            return await Result<IEnumerable<ChatUserResponse>>.SuccessAsync(chatResponse);
        }

        private static IEnumerable<ChatUserResponse> AddAdminPrivate(List<ChatUserResponse> chatResponse)
        {
            chatResponse.Add(new ChatUserResponse()
            {
                AdminReceiver = true,
                FirstName = "Admin",
                UserName = "Admin",
                Type = ChatUserType.Admin
            });

            return chatResponse;
        }
    }
}